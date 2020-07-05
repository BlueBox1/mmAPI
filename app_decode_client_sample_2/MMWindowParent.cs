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
      public System.Timers.Timer _watchDog;
      private bool _closing = false;
      private MMInterface _interface = null; 

      public WindowParent(int argbTheme) : base(argbTheme)
      {
         // ready a list for any children that may simply appear..
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
      public MM_TASK_ITEM GetSessionState()
      {
         if (_interface != null)
            return _interface._state;

         return MM_TASK_ITEM.MM_NONE;
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
         this.Hide();
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

            m.WParam = IntPtr.Zero;
            _HWnd = IntPtr.Zero;
            base.WndProc(ref m);
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
      }
      private void OnTimedEvent(object source, ElapsedEventArgs e)
      {
         Debug.WriteLine($"WATCHDOG {0}\n", e.SignalTime);

         if (!_closing && GetSessionState() != MM_TASK_ITEM.MM_OPEN && _statusMessage.Length > 0) // keep trying to re-open
            RestartSession(IntPtr.Zero, 0);
         else
            _watchDog.Enabled = false;
      }

      public void PaintSessionStatus(string status)
      {
         PaintStatus(status);
      }
      public void SetSessionWindowText(string txt)
      {
         SetWindowText(txt);
      }
      public void SetSessionOSDText(string txt)
      {
         SetOSDText(txt, this.Width, this.Height);
      }
   }
}