using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

using MultiMedia;

namespace MM.SDK
{
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public class Child
   {
      public MM_CLIENT_CHILD Parms;
   }
   public partial class WindowChild : Window
   {
      private MMInterface _interface;
      public Child _child = new Child();
      private MM_CLIENT_CHILD_REQUEST _userStateChild = 0;
      private MM_CLIENT_SOURCE_REQUEST _userStateSource = 0;
      public uint _id = 0;
      private string _url = null;
      WindowParent _parent = null;
      int _childSize = 0;

      public WindowChild(uint id, MMInterface mmInterface, string url, int argbTheme, WindowParent parent) : base(argbTheme)
      {
         Debug.Assert(_id == 0);
         _id = id;
         _url = url;
         _interface = mmInterface;
         _parent = parent;

         _childSize = Marshal.SizeOf(typeof(MM_CLIENT_CHILD));

         _child.Parms.Size = (uint)_childSize;
         _child.Parms.HWnd = IntPtr.Zero;
         _child.Parms.Flags = 0;

         _child.Parms.Source.Size = (uint)Marshal.SizeOf(typeof(MM_CLIENT_SOURCE));
         _child.Parms.Source.Flags = 0;

         this.HandleCreated += new EventHandler(this.Window_Handle);
         //this.Load += new EventHandler(this.Window_Load);
         this.Shown += new EventHandler(this.Window_Shown);
      }
      private void Window_Handle(object sender, EventArgs e)
      {
         _HWnd = _child.Parms.HWnd = this.Handle;
      }
      private void Window_Shown(object sender, EventArgs e)
      {
         // using this event allows all load event messages / painting to flush before starting
         WindowChildOpen();
      }
      private void WindowChildCmd(MM_CLIENT_CHILD_REQUEST request)
      {
         _userStateChild |= request;
         _child.Parms.Flags = request;

         if (_child.Parms.HWnd == IntPtr.Zero || _child.Parms.Flags == 0)
         {
            Debug.Assert(false);
            return;
         }

         IntPtr lpChild = Marshal.AllocHGlobal(_childSize);
         try
         {
            // Copy the struct to unmanaged memory.
            Marshal.StructureToPtr(_child.Parms, lpChild, false);
            _interface.InvokeMMTask(new MM_TASK(MM_TASK_ITEM.MM_DICT, mmSessionDictionaryKeys.CLI_CHILD, lpChild, _childSize));
         }
         finally
         {
            Marshal.FreeHGlobal(lpChild);
         }

         Debug.WriteLine("WindowChildCmd {0} child.flags {1} source flags {2}", _child.Parms.Flags, _child.Parms.Source.Flags, _id);
      }
      public void WindowChildOpen()
      {       
         _child.Parms.Source.Flags |= _userStateSource;
         WindowChildCmd(MM_CLIENT_CHILD_REQUEST.MM_CLIENT_CHILD_OPEN | _userStateChild);
      }
      public void WindowChildDewarp()
      {
         WindowChildCmd(MM_CLIENT_CHILD_REQUEST.MM_CLIENT_CHILD_DEWARP);
      }
      public void WindowChildZoom()
      {
         //mmStatus sts = mmStatus.MM_STS_NONE;
         WindowChildCmd(MM_CLIENT_CHILD_REQUEST.MM_CLIENT_CHILD_ZOOM);
      }
      public void WindowChildClose()
      {
         WindowChildCmd(MM_CLIENT_CHILD_REQUEST.MM_CLIENT_CHILD_CLOSE);
      }
      public void WindowChildSource()
      {
         _userStateSource |= _child.Parms.Source.Flags;
         WindowChildCmd(MM_CLIENT_CHILD_REQUEST.MM_CLIENT_CHILD_SOURCE);
      }
      public void WindowChildResize()
      {
         WindowChildCmd(MM_CLIENT_CHILD_REQUEST.MM_CLIENT_CHILD_RESET);
      }
      public void PaintChildStatus(string statusMessage)
      {
         PaintStatus(statusMessage);
      }
      protected override void WndProc(ref Message m)
      {
         if (m.Msg == MMInterop.WM_SIZE)
         {
            base.WndProc(ref m); // update the OS first
            
            if (!_updatingWindow)
               WindowChildResize();

            PaintStatus(_statusMessage);
         }
         else if (m.Msg == MMInterop.WM_CLOSE)
         {
            // did user manually or parent programatically call close()..
            if (m.WParam == IntPtr.Zero) // user called
            {
               _parent.CloseWindow((IntPtr)_id, true);
               _child.Parms.HWnd = IntPtr.Zero; // stop any furter calls
               _HWnd = IntPtr.Zero;
            }
            base.WndProc(ref m);
         }
         else
            base.WndProc(ref m);
      }
   }
}
