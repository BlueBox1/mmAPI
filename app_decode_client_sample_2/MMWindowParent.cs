using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

using MultiMedia;

namespace MM.SDK
{
   public partial class WindowParent : Window
   {
      private bool _closing = false;
      private bool _parentClosed = false;
      private System.Timers.Timer _watchDog;
      private readonly object _listChildLock = new object();
      private List<WindowChild> _children;
      private MMInterface _interface = null; 

      public WindowParent(int argbTheme) : base(argbTheme)
      {
         // ready a list for any children that may simply appear..
         _children = new List<WindowChild>();
         //7 second watchdog for reconnect on errors
         _watchDog = new System.Timers.Timer(7000);
         _watchDog.Elapsed += new ElapsedEventHandler(OnTimedEvent);

         // this is the call order:
         this.HandleCreated += new EventHandler(this.Window_Handle);
         //this.Activated += new EventHandler(this.Window_Active); ;
         this.Load += new EventHandler(this.Window_Load);
         this.Shown += new EventHandler(this.Window_Shown);
         //this.Paint += new System.Windows.Forms.PaintEventHandler(this.Window_Paint);
      }
      public void SetInterface(MMInterface __interface)
      {
         _interface = __interface;
      }
      public IntPtr GetSessionHandle()
      {
         if (_interface != null)
            return _interface._hSession;

         return IntPtr.Zero;
      }
      private void Window_Handle(object sender, EventArgs e)
      {
         // the value of the Handle property is a Windows HWND
         // if the handle has not yet been created, referencing this property will force the handle to be created
         // we need this when internal window is returned and MMSession accesses this public member straight away
         _HWnd = this.Handle;
      }
      private void Window_Load(object sender, EventArgs e)
      {
         // allow simple inter process communication via SendMessage now we have the hWnd
         MMInterop.CHANGEFILTERSTRUCT changeFilter = new MMInterop.CHANGEFILTERSTRUCT();
         changeFilter.size = (uint)Marshal.SizeOf(changeFilter);
         changeFilter.info = 0;
         if (!MMInterop.ChangeWindowMessageFilterEx
            (this.Handle, MMInterop.WM_COPYDATA,
             MMInterop.ChangeWindowMessageFilterExAction.Allow, ref changeFilter))
         {
            int error = Marshal.GetLastWin32Error();
            Debug.WriteLine(String.Format(String.Format("ChangeWindowMessageFilterEx error {0} occurred.", error)));
            Debug.Assert(false);
         }

         this.Hide(); // the function below will show upon completion of setting itself up, avoids flashing window on startup
         // set the window position first, important as auto GPU selection depends on this placement
         if (updateWindowMethod(_interface._parms.Window.WindowParms))
            _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_DICT, mmSessionDictionaryKeys.CLI_RESET, IntPtr.Zero, 0));
      }
      private void Window_Shown(object sender, EventArgs e)
      {
         // using this event allows all load event messages / painting to flush before starting
         // passing null into _interface will pick up the local data structure set from command line arguments
         RestartSession(IntPtr.Zero, 0);
      }
      public bool CloseWindow(IntPtr childMarker, bool bChildUserCalled)
      {
         if (childMarker != IntPtr.Zero) // close child
         {
            WindowChild child = GetChildSession((uint)childMarker); // get before we remove it from the list
            if (child != null) // dispose the child window our way to allow us the difference between the user closing it manually
            {
               child.Hide();
               RemChildSession((uint)childMarker, false);
               child.WindowChildClose();

               if (!bChildUserCalled) // important, close it our way
                  MMInterop.SendMessage(child.GetHWND(), MMInterop.WM_CLOSE, (IntPtr)child._id, IntPtr.Zero);
            }

            if (_children.Count > 0 || !_parentClosed)
               return false; // sessions still rendering
         }
         else // close parent
         {
            this.Hide();
            if (_children.Count > 0) // do not close the session
            {
               // turn rendering off and hide this window
               _interface._parms.Source.SourceParms.Flags |= MM_CLIENT_SOURCE_REQUEST.MM_CLIENT_SOURCE_RENDER;
               _interface._parms.Source.SourceParms.BRender = 0;
               _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_DICT, mmSessionDictionaryKeys.CLI_SOURCE, IntPtr.Zero, 0));
               // optomize resources
               _interface._parms.Window.WindowParms.ShowState = SHOWSTATE.HIDE;
               _interface._parms.Window.WindowParms.Placement.Right = 100;
               _interface._parms.Window.WindowParms.Placement.Bottom = 100;
               updateWindowMethod(_interface._parms.Window.WindowParms);
               _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_DICT, mmSessionDictionaryKeys.CLI_RESET, IntPtr.Zero, 0));
               _parentClosed = true;
               return false; // children still rendering, keep the parent message loop
            }
         }

         _closing = true;
         // all done close program
         _interface.Stop();
         // wait for timer to end, crude but safe
         PaintSessionStatus("");
         while (_watchDog.Enabled != false)
            System.Threading.Thread.Sleep(100);
         _watchDog.Close();
         // close parent window for good
         return true;
      }
      protected override void WndProc(ref Message m)
      {
         if (m.Msg == MMInterop.WM_SIZE)
         {
            base.WndProc(ref m); // update the OS first

            if (!_updatingWindow)
               _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_DICT, mmSessionDictionaryKeys.CLI_RESET, IntPtr.Zero, 0));

            PaintStatus(_statusMessage); // this window only is resizing
         }
         else if (m.Msg == MMInterop.WM_CLOSE)
         {
            if (!CloseWindow(m.WParam, false))
               return;

            // whoever was last to close, parent or child, its now safe to close the parent
            m.WParam = IntPtr.Zero;
            _HWnd = IntPtr.Zero;
            base.WndProc(ref m);
         }
         else if (m.Msg == MMInterop.WM_SETTEXT &&  (m.WParam != IntPtr.Zero)) // our interpretation of WM_SETTEXT for sync's sake (could use WM_COPYDATA)
         {
            try
            {
               if (((MMSendMessage.MM_WIN_TXT_FLAG)m.WParam & MMSendMessage.MM_WIN_TXT_FLAG.MM_STATUS) == MMSendMessage.MM_WIN_TXT_FLAG.MM_STATUS)
                  PaintSessionStatus(MMInterop.PtrToString(m.LParam)); // window status text
               if (((MMSendMessage.MM_WIN_TXT_FLAG)m.WParam & MMSendMessage.MM_WIN_TXT_FLAG.MM_TITLE) == MMSendMessage.MM_WIN_TXT_FLAG.MM_TITLE)
                  SetSessionWindowText(MMInterop.PtrToString(m.LParam)); // window title text
               if (((MMSendMessage.MM_WIN_TXT_FLAG)m.WParam & MMSendMessage.MM_WIN_TXT_FLAG.MM_WDOG) == MMSendMessage.MM_WIN_TXT_FLAG.MM_WDOG)
                  _watchDog.Enabled = true; // re-connect - this is either open or no network frames message for example
               if (((MMSendMessage.MM_WIN_TXT_FLAG)m.WParam & MMSendMessage.MM_WIN_TXT_FLAG.MM_OSD) == MMSendMessage.MM_WIN_TXT_FLAG.MM_OSD)
                  SetSessionOSDText(MMInterop.PtrToString(m.LParam)); // window osd text

            } catch { Debug.Assert(false); }

         }
         else if (m.Msg == MMInterop.WM_COPYDATA)
         {
            try
            {
               MMInterop.COPYDATASTRUCT copyData = (MMInterop.COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(MMInterop.COPYDATASTRUCT));
               int command = (int)copyData.dwData;
               //Debug.WriteLine("MMInterop.WM_COPYDATA command {0}", command);

               if (command == MMSendMessage.MM_WINCMD_DEWARP)
               {
                  MM_CLIENT_DEWARP dewarp = (MM_CLIENT_DEWARP)Marshal.PtrToStructure(copyData.lpData, typeof(MM_CLIENT_DEWARP));
                  if((copyData.cbData == 0) || (dewarp.Size != (uint)Marshal.SizeOf(typeof(MM_CLIENT_DEWARP))))
                  {
                     Debug.Assert(false);
                     return;
                  }
                  else if (m.WParam != IntPtr.Zero)
                  {
                     WindowChild child = GetChildSession((uint)m.WParam);
                     if (child != null)
                     {
                        child._child.Parms.Dewarp = dewarp;
                        ChildSessionTask(child, MMSendMessage.MM_WINCMD_DEWARP);
                     }
                     else
                        Debug.Assert(false);
                  }
                  else
                     _interface.InvokeMMTask(new MM_TASK (MM_TASK_ITEM.MM_DICT, mmSessionDictionaryKeys.CLI_DEWARP, copyData.lpData, copyData.cbData));
               }
               else if (command == MMSendMessage.MM_WINCMD_ZOOM)
               {
                  MM_RECT zoom = (MM_RECT)Marshal.PtrToStructure(copyData.lpData, typeof(MM_RECT));
                  if (copyData.cbData == 0)
                  {
                     Debug.Assert(false);
                     return;
                  }
                  else if (m.WParam != IntPtr.Zero)
                  {
                     WindowChild child = GetChildSession((uint)m.WParam);
                     if (child != null)
                     {
                        child._child.Parms.Zoom = zoom;
                        ChildSessionTask(child, MMSendMessage.MM_WINCMD_ZOOM);
                     }
                     else
                        Debug.Assert(false);
                  }
                  else
                     _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_DICT, mmSessionDictionaryKeys.CLI_ZOOM, copyData.lpData, copyData.cbData));
               }
               else if (command == MMSendMessage.MM_WINCMD_SOURCE)
               {
                  MM_CLIENT_SOURCE source = (MM_CLIENT_SOURCE)Marshal.PtrToStructure(copyData.lpData, typeof(MM_CLIENT_SOURCE));
                  if ((copyData.cbData == 0) || (source.Size != (uint)Marshal.SizeOf(typeof(MM_CLIENT_SOURCE))))
                  {
                     Debug.Assert(false);
                     return;
                  }
                  else if (m.WParam != IntPtr.Zero)
                  {
                     WindowChild child = GetChildSession((uint)m.WParam);
                     if (child != null)
                     {
                        // only update what the user is requesting..
                        if ((source.Flags & MM_CLIENT_SOURCE_REQUEST.MM_CLIENT_SOURCE_RENDER) == MM_CLIENT_SOURCE_REQUEST.MM_CLIENT_SOURCE_RENDER)
                           child._child.Parms.Source.BRender = source.BRender;
                        if ((source.Flags & MM_CLIENT_SOURCE_REQUEST.MM_CLIENT_SOURCE_ASPECT_RATIO) == MM_CLIENT_SOURCE_REQUEST.MM_CLIENT_SOURCE_ASPECT_RATIO)
                           child._child.Parms.Source.BEnforceAR = source.BEnforceAR;

                        child._child.Parms.Source.Flags = source.Flags;
                        ChildSessionTask(child, MMSendMessage.MM_WINCMD_SOURCE);
                     }
                     else
                        Debug.Assert(false);
                  }
                  else
                     _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_DICT, mmSessionDictionaryKeys.CLI_SOURCE, copyData.lpData, copyData.cbData));
               }
               else if (command == MMSendMessage.MM_WINCMD_PAUSE)
               {
                  if (m.WParam != IntPtr.Zero)
                  {
                     WindowChild child = GetChildSession((uint)m.WParam);
                     if (child != null)
                     {
                        child._child.Parms.Source.Flags |= MM_CLIENT_SOURCE_REQUEST.MM_CLIENT_SOURCE_RENDER;
                        child._child.Parms.Source.BRender = 0;
                        ChildSessionTask(child, MMSendMessage.MM_WINCMD_SOURCE);
                     }
                     else
                        Debug.Assert(false);
                  }
                  else
                     _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_PAUSE, 0, IntPtr.Zero, 0));
               }
               else if (command == MMSendMessage.MM_WINCMD_PLAY)
               {
                  if (copyData.cbData == 0)
                  {
                     Debug.Assert(false);
                     return;
                  }
                  else if (m.WParam != IntPtr.Zero)
                  {
                     WindowChild child = GetChildSession((uint)m.WParam);
                     if (child != null)
                     {
                        child._child.Parms.Source.Flags |= MM_CLIENT_SOURCE_REQUEST.MM_CLIENT_SOURCE_RENDER;
                        child._child.Parms.Source.BRender = 1;
                        ChildSessionTask(child, MMSendMessage.MM_WINCMD_SOURCE);
                     }
                     else
                        Debug.Assert(false);
                  }
                  else
                     _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_PLAY, 0, copyData.lpData, copyData.cbData));
               }
               else if (command == MMSendMessage.MM_WINCMD_OPEN)
               {
                  if (copyData.cbData != 0 && m.WParam == IntPtr.Zero)
                     RestartSession(copyData.lpData, copyData.cbData);
                  else
                     Debug.Assert(false);
               }
               else if (command == MMSendMessage.MM_WINCMD_WINDOW)
               {
                  MM_WINDOW window = (MM_WINDOW)Marshal.PtrToStructure(copyData.lpData, typeof(MM_WINDOW));
                  if (window.Size == (Marshal.SizeOf(typeof(MM_WINDOW))))
                  {
                     if (m.WParam != IntPtr.Zero && copyData.cbData != 0)
                     {
                        // create and update the window form on this thread, no calls into mmAPI
                        WindowChild child = GetChildSession((uint)m.WParam);
                        if (child == null) // new child case
                        {
                           child = new WindowChild((uint)m.WParam, _interface, _interface._parms.Open.URL, _ARGBTheme, this);
                           AddChildSession(child);
                           child.SetWindowText(_interface._parms.Open.URL);
                           // drop through, Show() in updateWindowMethod will invoke the childs Window_Handle event call etc
                           // Note: do not use childs Window_Load like we do the parent, Show() below in update keeps us in Sync
                        }
                        if (child.updateWindowMethod(window))
                           child.WindowChildResize();
                     }
                     else if (updateWindowMethod(window))
                           _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_DICT, mmSessionDictionaryKeys.CLI_RESET, IntPtr.Zero, 0));
                  }
                  else
                     Debug.Assert(false);
               }
               else
                  Debug.WriteLine(String.Format("WM_COPYDATA - Unrecognized command = {0}.", command));
            }
            catch (Exception e)
            {
               Debug.WriteLine("{0} Exception caught - WndProc", e);
               Debug.WriteLine(e.ToString());
            }
         }
         else
            base.WndProc(ref m);
      }

      private void RestartSession(IntPtr lpData, int dataSize)
      {
         // lpData and dataSize exist if we are called from the MM_WINCMD_OPEN message (think carousel)
         uint bybassWindowTxtStatus = 0;
         if(lpData != IntPtr.Zero)
            bybassWindowTxtStatus = 0x80000000;
         _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_CLOSE, 0, IntPtr.Zero, 0));
         _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_OPEN, (mmSessionDictionaryKeys)bybassWindowTxtStatus, lpData, dataSize));
         _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_PLAY, 0, IntPtr.Zero, 0));

         // set any source parameters
         if (_interface._parms.Source.SourceParms.Flags != 0)
            _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_DICT, mmSessionDictionaryKeys.CLI_SOURCE, IntPtr.Zero, 0));
         // set any dewarp params
         if (_interface._parms.Dewarp.DewarpParms.BSessionEnabled == 1)
            _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_DICT, mmSessionDictionaryKeys.CLI_DEWARP, IntPtr.Zero, 0));
         // set any zoom rect params
         if (_interface._parms.Zoom.ZoomParms.Right > 0 || _interface._parms.Zoom.ZoomParms.Bottom > 0)
            _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_DICT, mmSessionDictionaryKeys.CLI_ZOOM, IntPtr.Zero, 0));

         lock (_listChildLock)
         {
            foreach (var child in _children)
               child.WindowChildClose();

            foreach (var child in _children)
               child.WindowChildOpen();
         }
      }
      private void OnTimedEvent(object source, ElapsedEventArgs e)
      {
         Debug.WriteLine($"WATCHDOG {0}\n", e.SignalTime);
          
         if (!_closing && _statusMessage.Length > 0) // keep trying to re-open
            RestartSession(IntPtr.Zero, 0);
         else
            _watchDog.Enabled = false;
      }

      private void PaintSessionStatus(string status)
      {
         PaintStatus(status);
         lock (_listChildLock)
            foreach ( var child in _children)
               child.PaintChildStatus(status);
      }
      private void SetSessionWindowText(string txt)
      {
         SetWindowText(txt);
         lock (_listChildLock)
            foreach (var child in _children)
               child.SetWindowText(txt);
      }
      private void SetSessionOSDText(string txt)
      {
         SetOSDText(txt, this.Width, this.Height);
         lock (_listChildLock)
            foreach (var child in _children)
               child.SetOSDText(txt, this.Width, this.Height);
      }

      ///////////////////////////////////////////////////////////
      // Child window helper functions called with _openChildLock
      ///////////////////////////////////////////////////////////

      private void ChildSessionTask(WindowChild child, int task)
      {
         switch (task)
         {
            case MMSendMessage.MM_WINCMD_ZOOM:
               child.WindowChildZoom();
               break;
            case MMSendMessage.MM_WINCMD_DEWARP:
               child.WindowChildDewarp();
               break;
            case MMInterop.WM_CLOSE:
               child.WindowChildClose();
               break;
            case MMSendMessage.MM_WINCMD_SOURCE:
               child.WindowChildSource();
               break;
            case MMSendMessage.MM_WINCMD_PLAY:
            case MMSendMessage.MM_WINCMD_PAUSE:
            case MMSendMessage.MM_WINCMD_OPEN:
            default:
               Debug.Assert(false); // opens itself via window load event
               break;
                  
         }
      }
      private WindowChild GetChildSession(uint id)
      {
         lock (_listChildLock)
         {
            foreach (var c in _children)
            {
               if (c._id == id)
                  return c;
            }
            return null;
         }
      }
      private void AddChildSession(WindowChild child)
      {
         lock (_listChildLock)
         {
            _children.Add(child);
         }
      }
      private void RemChildSession(uint id, bool bAll)
      {
         lock (_listChildLock)
         {
            for (int i = 0; i < _children.Count; i++)
            {
               if (bAll || _children[i]._id == id)
               {
                  _children.Remove(_children[i]);
                  if (!bAll) return;
               }
            }
         }
      }
   }
}