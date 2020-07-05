using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading.Tasks;

using MultiMedia;
using MM.SDK;
using System.Text;
using System.IO;

namespace app_decode_client_sample_2
{
   static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      /// 
      [STAThread]
      static void Main()
      {
         mmStatus sts = 0;
         MM_LOAD load = new MM_LOAD();

         var currentDirectory = System.Environment.CurrentDirectory;

         // colour theme
         int argbTheme = 0x00FFFF;
         int timeoutMS = 10000;
         IntPtr logPath = IntPtr.Zero;
         MMParameters _Parms = new MMParameters();
         MMHelper.InitClientParms(1, _Parms);
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         string[] args = Environment.GetCommandLineArgs();

         try
         {
            // simple cmd line parser
            Dictionary<string, string> retval = args.ToDictionary(
                  k => k.Split(new char[] { '=' }, 2)[0].ToLower(),
                  v => v.Split(new char[] { '=' }, 2).Count() > 1
                  ? v.Split(new char[] { '=' }, 2)[1]
                  : null);

            int tmp;
            float tmpf;
            foreach (var arg in retval)
            {
               if (arg.Key == "-url")
                  _Parms.Open.URL = arg.Value;
               else if (arg.Key == "-user")
                  _Parms.Open.UserName = arg.Value;
               else if (arg.Key == "-pass")
                  _Parms.Open.PassWord = arg.Value;
               else if (arg.Key == "-x")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Window.WindowParms.Placement.Left = tmp;
               }
               else if (arg.Key == "-y")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Window.WindowParms.Placement.Top = tmp;
               }
               else if (arg.Key == "-w")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Window.WindowParms.Placement.Right = tmp;
               }
               else if (arg.Key == "-h")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Window.WindowParms.Placement.Bottom = tmp;
               }
               else if (arg.Key == "-tcp")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Open.OpenParms.BNetTCP = (uint)tmp;
               }
               else if (arg.Key == "-start_time")
                  _Parms.Play.StartTime = arg.Value;
               else if (arg.Key == "-end_time")
                  _Parms.Play.EndTime = arg.Value;
               else if (arg.Key == "-cache_video")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Open.OpenParms.CacheVideo = (uint)tmp;
               }
               else if (arg.Key == "-speed")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Play.PlayParms.Speed = (MM_CLIENT_SPEED)tmp;
               }
               else if (arg.Key == "-repeat")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Open.OpenParms.BRepeat = (uint)tmp;
               }
               else if (arg.Key == "-reverse")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Play.PlayParms.BReverse = (uint)tmp;
               }
               else if (arg.Key == "-borders")
               {
                  if (int.TryParse(arg.Value, out tmp))
                  {
                     if (tmp != 0)
                        _Parms.Window.WindowParms.BorderAndTitleBar = true;
                  }
               }
               else if (arg.Key == "-window")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Window.WindowParms.ShowState = (SHOWSTATE)tmp;
               }
               else if (arg.Key == "-net_tcp")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Open.OpenParms.BNetTCP = (uint)tmp;
               }
               else if (arg.Key == "-aspect_ratio")
               {
                  if (int.TryParse(arg.Value, out tmp))
                  {
                     _Parms.Source.SourceParms.BEnforceAR = (uint)tmp;
                     _Parms.Source.SourceParms.Flags |= MM_CLIENT_SOURCE_REQUEST.MM_CLIENT_SOURCE_ASPECT_RATIO;
                  }
               }
               else if (arg.Key == "-argb")
               {
                  if (int.TryParse(arg.Value, out tmp))
                  {
                     argbTheme = tmp;
                  }
               }
               else if (arg.Key == "-timeout")
               {
                  if (int.TryParse(arg.Value, out tmp))
                  {
                     timeoutMS = tmp;
                  }
               }
               else if (arg.Key == "-logpath")
               {
                  // account for UTF8 encoded data
                  int len = Encoding.UTF8.GetByteCount(arg.Value);
                  byte[] utf8Bytes = new byte[len + 1];
                  Encoding.UTF8.GetBytes(arg.Value, 0, arg.Value.Length, utf8Bytes, 0);
                  load.PLogPath = Marshal.AllocHGlobal(utf8Bytes.Length);
                  Marshal.Copy(utf8Bytes, 0, load.PLogPath, utf8Bytes.Length);
               }
               else if (arg.Key == "-se")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Dewarp.DewarpParms.BSessionEnabled = (uint)tmp;
               }
               else if (arg.Key == "-pf")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Dewarp.DewarpParms.PanoFactor = (uint)tmp;
               }
               else if (arg.Key == "-fov")
               {
                  if (float.TryParse(arg.Value, out tmpf))
                     _Parms.Dewarp.DewarpParms.FOV = tmpf;
               }
               else if (arg.Key == "-xa")
               {
                  if (float.TryParse(arg.Value, out tmpf))
                     _Parms.Dewarp.DewarpParms.XAngle = tmpf;
               }
               else if (arg.Key == "-ya")
               {
                  if (float.TryParse(arg.Value, out tmpf))
                     _Parms.Dewarp.DewarpParms.YAngle = tmpf;
               }
               else if (arg.Key == "-xab")
               {
                  if (float.TryParse(arg.Value, out tmpf))
                     _Parms.Dewarp.DewarpParms.XAngleB = tmpf;
               }
               else if (arg.Key == "-hv")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Dewarp.DewarpParms.BHorViewMode = (uint)tmp;
               }
               else if (arg.Key == "-fovr")
               {
                  if (float.TryParse(arg.Value, out tmpf))
                     _Parms.Dewarp.DewarpParms.FOVRot = tmpf;
               }
               else if (arg.Key == "-xc")
               {
                  if (float.TryParse(arg.Value, out tmpf))
                     _Parms.Dewarp.DewarpParms.XCenter = tmpf;
               }
               else if (arg.Key == "-yc")
               {
                  if (float.TryParse(arg.Value, out tmpf))
                     _Parms.Dewarp.DewarpParms.YCenter = tmpf;
               }
               else if (arg.Key == "-ra")
               {
                  if (float.TryParse(arg.Value, out tmpf))
                     _Parms.Dewarp.DewarpParms.Radius = tmpf;
               }
               else if (arg.Key == "-hs")
               {
                  if (float.TryParse(arg.Value, out tmpf))
                     _Parms.Dewarp.DewarpParms.HStretch = tmpf;
               }
               else if (arg.Key == "-zt")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Zoom.ZoomParms.Top = tmp;
               }
               else if (arg.Key == "-zl")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Zoom.ZoomParms.Left = tmp;
               }
               else if (arg.Key == "-zr")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Zoom.ZoomParms.Right = tmp;
               }
               else if (arg.Key == "-zb")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Zoom.ZoomParms.Bottom = tmp;
               }
               else if (arg.Key == "-reserved")
               {
                  if (int.TryParse(arg.Value, out tmp))
                     _Parms.Open.Reserved = (uint)tmp;
               }
            }

            if (_Parms.Open.URL == null)
               throw new System.ArgumentException("You must provide -url=[url]");
         }
         catch
         {
            string msg = "usage: -url=[url] | -user[username] | -pass[password] | -x=[x] | -y=[y] | -w=[w] | -h=[h] \nExample: -url=stream:rtsp:\\\\127.0.0.1:554\\\\rsc";
            Debug.WriteLine(msg);
            return;
         }

         load.TimeoutMS = (uint)timeoutMS;
         if (MMHelper.MMLoad(ref load, Environment.CurrentDirectory))
         {
            // Create our window           
            WindowParent window = new WindowParent(argbTheme);

            // Specify the default call back functions found within the Window class
            GCHandle _GCHandle = GCHandle.Alloc(window);
            _Parms.Open.OpenParms.PStatusUserData = GCHandle.ToIntPtr(_GCHandle);
            _Parms.Open.OpenParms.PStatusCBKFN = new MM_STATUS_CBKFN(StatusCallbackFN);
            // Interested in..
            _Parms.Open.OpenParms.OpenFlags = MM_CLIENT_REQUEST.MM_CLIENT_REQUEST_VIDEO_SOURCE | MM_CLIENT_REQUEST.MM_CLIENT_REQUEST_VIDEO_TO_DISPLAY;
            
            // Create a synchronized interface for the window to use
            MMInterface _interface = new MMInterface(_Parms, window);
            window.SetInterface(_interface);
            // Execute - NOTE [STAThread] above Main..
            Application.Run(window);

            mmMethods.mmRelease(load.HInterface);
            if (_GCHandle.IsAllocated)
               _GCHandle.Free();
         }
         else
         {
            string msg = "Load Failed: 0x" + sts.ToString("X");
            Debug.WriteLine(msg);
         }
      }

      private static void StatusCallbackFN(IntPtr hSession, uint status, IntPtr pMessage, IntPtr pUserData)
      {
         string message;
         WindowParent window;
         try
         {
            // take a copy on the stack
            message = Marshal.PtrToStringAnsi(pMessage);
            window = (WindowParent)((GCHandle.FromIntPtr((IntPtr)(pUserData))).Target);
         }
         catch (Exception e)
         {
            Console.WriteLine("{0} Exception caught.", e);
            return;
         }
         Task.Run(() => StatusCallbackFNEx(hSession, status, window, message));
      }
      private static void StatusCallbackFNEx(IntPtr hSession, uint status, WindowParent window, string message)
      {
         try
         {
            string msg = "";
            if ((((uint)status & (uint)mmStatusBase.MM_STS_SRC_INFO_BASE) == (uint)mmStatusBase.MM_STS_SRC_INFO_BASE) ||
               (((uint)status & (uint)mmStatusBase.MM_STS_LIB_INFO_BASE) == (uint)mmStatusBase.MM_STS_LIB_INFO_BASE))
            {
               msg = $"INFO - Session 0x{hSession:X} Status 0x{status:X} - {message}";
               if (status == (uint)mmStatus.MM_STS_SRC_INFO_NO_NETWORK_FRAME_TIMEOUT)
               {
                  // network frames not detected, try to re-connect
                  window.PaintSessionStatus(msg); 
                  window._watchDog.Enabled = true;
               }
            }
            else if ((((uint)status & (uint)mmStatusBase.MM_STS_SRC_WARNING_BASE) == (uint)mmStatusBase.MM_STS_SRC_WARNING_BASE) ||
                     (((uint)status & (uint)mmStatusBase.MM_STS_LIB_WARNING_BASE) == (uint)mmStatusBase.MM_STS_LIB_WARNING_BASE))
            {
               msg = $"WARNING - Session 0x{hSession:X} Status 0x{status:X} - {message}";
               if (((uint)status & (uint)mmStatusBase.MM_STS_SRC_WARNING_BASE + 0x100000) == (uint)mmStatusBase.MM_STS_SRC_WARNING_BASE + 0x100000)
               {
               }
            }
            else if ((((uint)status & (uint)mmStatusBase.MM_STS_SRC_ERROR_BASE) == (uint)mmStatusBase.MM_STS_SRC_ERROR_BASE) ||
                     (((uint)status & (uint)mmStatusBase.MM_STS_LIB_ERROR_BASE) == (uint)mmStatusBase.MM_STS_LIB_ERROR_BASE))
            {
               msg = $"ERROR - Session 0x{hSession:X} Status 0x{status:X} - {message}";
               window.PaintSessionStatus(msg);
               window._watchDog.Enabled = true;
            }
            Debug.WriteLine(msg);
         }
         catch (Exception e)
         {
            Debug.WriteLine("{0} Exception caught.", e);
         }
      }
   }
}