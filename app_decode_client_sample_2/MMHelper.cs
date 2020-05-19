using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;

using MultiMedia;

namespace MM.SDK
{
   public static class MMHelper
   {
      //----------------------  PARAM FUNCTIONS -------------------------//

      public static void InitClientParms(uint ar, MMParameters parms)
      {
         parms.Context = CONTEXT.INTERNAL;
         parms.ChildMarker = 0;
         parms.uintID = 0;

         parms.Open = new Open();
         parms.Play = new Play();
         parms.Window = new WindowForm();
         parms.Dewarp = new Dewarp();
         parms.Zoom = new Zoom();
         parms.Source = new Source();

         parms.Open.URL = "";
         parms.Open.UserName = "";
         parms.Open.PassWord = "";
         parms.Open.Reserved = 0;
         parms.Open.OpenParms.Size = (uint)Marshal.SizeOf(typeof(MM_CLIENT_OPEN));
         parms.Open.OpenParms.PURL = IntPtr.Zero;
         parms.Open.OpenParms.PDataCBKFN = null;
         parms.Open.OpenParms.PDataUserData = IntPtr.Zero;
         parms.Open.OpenParms.PStatusCBKFN = null;
         parms.Open.OpenParms.PStatusUserData = IntPtr.Zero;
         parms.Open.OpenParms.OpenFlags = MM_CLIENT_REQUEST.MM_CLIENT_REQUEST_VIDEO_SOURCE | MM_CLIENT_REQUEST.MM_CLIENT_REQUEST_VIDEO_TO_DISPLAY;
         parms.Open.OpenParms.BRepeat = 1;
         parms.Open.OpenParms.HWnd = IntPtr.Zero;
         parms.Open.OpenParms.BNetTCP = 1;
         parms.Open.OpenParms.CacheVideo = 1000; // 1000 = 1 second
         parms.Open.OpenParms.CacheAudio = 1000; // 1000 = 1 second
         parms.Open.OpenParms.PReserved = IntPtr.Zero;

         parms.Play.StartTime = "";
         parms.Play.EndTime = "";
         parms.Play.PlayParms.Size = (uint)Marshal.SizeOf(typeof(MM_CLIENT_PLAY));
         parms.Play.PlayParms.PStartTime = IntPtr.Zero;
         parms.Play.PlayParms.PEndTime = IntPtr.Zero;
         parms.Play.PlayParms.Speed = MM_CLIENT_SPEED.MM_CLIENT_SPEED_ONE;
         parms.Play.PlayParms.BReverse = 0;

         parms.Window.WindowParms.Size = (uint)Marshal.SizeOf(typeof(MM_WINDOW));
         parms.Window.WindowParms.Placement.Top = 100;
         parms.Window.WindowParms.Placement.Left = 100;
         parms.Window.WindowParms.Placement.Right = 640;
         parms.Window.WindowParms.Placement.Bottom = 480;
         parms.Window.WindowParms.BorderAndTitleBar = false;
         parms.Window.WindowParms.ShowState = SHOWSTATE.SHOW;
         parms.Window.WindowParms.ZOrder = 0;
         parms.Window.WindowParms.TopMost = false;

         parms.Dewarp.DewarpParms.Size = (uint)Marshal.SizeOf(typeof(MM_CLIENT_DEWARP));
         parms.Dewarp.DewarpParms.BSessionEnabled = 0;
         parms.Dewarp.DewarpParms.PanoFactor = 1;
         parms.Dewarp.DewarpParms.FOV = 1.2217304763960306f;
         parms.Dewarp.DewarpParms.XAngle = 0.0f;
         parms.Dewarp.DewarpParms.YAngle = 0.0f;
         parms.Dewarp.DewarpParms.XAngleB = 0.0f;
         parms.Dewarp.DewarpParms.BHorViewMode = 1;
         parms.Dewarp.DewarpParms.FOVRot = 0.0f;
         parms.Dewarp.DewarpParms.XCenter = 0.5f;
         parms.Dewarp.DewarpParms.YCenter = 0.5f;
         parms.Dewarp.DewarpParms.Radius = 0.5f;
         parms.Dewarp.DewarpParms.HStretch = 1.0f;

         parms.Zoom.ZoomParms.Top = 0;
         parms.Zoom.ZoomParms.Left = 0;
         parms.Zoom.ZoomParms.Right = 0;
         parms.Zoom.ZoomParms.Bottom = 0;

         parms.Source.SourceParms.Size = (uint)Marshal.SizeOf(typeof(MM_CLIENT_SOURCE));
         parms.Source.SourceParms.Flags = 0;
         parms.Source.SourceParms.BRender = 1;
         parms.Source.SourceParms.BEnforceAR = ar;
      }
      public static int userToRealX(int x)
      {
         return SystemInformation.VirtualScreen.X + x;
      }
      public static int userToRealY(int y)
      {
         return SystemInformation.VirtualScreen.Y + y;
      }
      public static int realToUserX(int x)
      {
         return Math.Abs(SystemInformation.VirtualScreen.X) + x;
      }
      public static int realToUserY(int y)
      {
         return Math.Abs(SystemInformation.VirtualScreen.Y) + y;
      }

      //----------------------  LOAD FUNCTIONS -------------------------//

      public static string GetMultiMediaPath(string baseDirectory)
      {
         bool _Is64BitProcess = (IntPtr.Size == 8);
         string mmLibPath = null;

         if (Directory.Exists(baseDirectory))
         {
#if DEBUG
            if (_Is64BitProcess)
               mmLibPath = Path.Combine(baseDirectory, @"multimedia\x64\Debug");
            else
               mmLibPath = Path.Combine(baseDirectory, @"multimedia\Win32\Debug");
#else
            if (_Is64BitProcess)
               mmLibPath = Path.Combine(baseDirectory, @"multimedia\x64\Release");
            else
               mmLibPath = Path.Combine(baseDirectory, @"multimedia\Win32\Release");
#endif
            Console.WriteLine("GetMultiMediaPath trying PATH: " + mmLibPath);
            if (Directory.Exists(mmLibPath))
               return mmLibPath;
#if DEBUG
            if (_Is64BitProcess)
               mmLibPath = Path.Combine(baseDirectory, @"bin\x64\Debug");
            else
               mmLibPath = Path.Combine(baseDirectory, @"bin\Win32\Debug");
#else
            if (_Is64BitProcess)
               mmLibPath = Path.Combine(baseDirectory, @"bin\x64\Release");
            else
               mmLibPath = Path.Combine(baseDirectory, @"bin\Win32\Release");
#endif
            Console.WriteLine("GetMultiMediaPath trying PATH: " + mmLibPath);
            if (Directory.Exists(mmLibPath))
               return mmLibPath;
         }
         Console.WriteLine("GetMultiMediaPath trying PATH: " + baseDirectory);
         return baseDirectory;
      }

      public static IntPtr MMLoad(string baseDirectory, out MM_VERSION version, out MM_LOAD_CONTEXT flags)
      {
         string mmLibPath = GetMultiMediaPath(baseDirectory);

         if (!string.IsNullOrEmpty(mmLibPath))
         {
            try
            {
               if (!MMInterop.SetDllDirectory(mmLibPath))
               {
                  Exception e = new System.ComponentModel.Win32Exception();
                  throw new DllNotFoundException("Unable to load library: " + mmLibPath, e);
               }
               else
               {
                  IntPtr hModule = IntPtr.Zero;
                  mmStatus sts = 0;

                  sts = mmMethods.mmLoad(out hModule, out version, out flags);
                  if (sts == mmStatus.MM_STS_NONE)
                  {
                     bool activated = false;
                     if ((flags & MM_LOAD_CONTEXT.MM_LOAD_CONTEXT_ACTIVATED) == MM_LOAD_CONTEXT.MM_LOAD_CONTEXT_ACTIVATED)
                        activated = true;

                     String loadString = String.Format("mmAPI v{0}.{1}.{2} activated={3} flags={4}",
                        version.Major.ToString().PadLeft(2, '0'), version.Minor.ToString().PadLeft(2, '0'), version.Micro.ToString().PadLeft(2, '0'),
                        activated, flags);

                     Console.WriteLine(loadString);
                     return hModule;
                  }
               }
            }
            catch (Exception e)
            {
               Console.WriteLine("GetMultiMediaPath returned PATH: " + mmLibPath);
               Console.WriteLine(e.ToString());
            }
         }

         flags = 0;
         version.Major = 0;
         version.Minor = 0;
         version.Micro = 0;
         return IntPtr.Zero;
      }

      public static void MMRelease(IntPtr hModule)
      {
         // free managed resources
         if (hModule != IntPtr.Zero)
            mmMethods.mmRelease(hModule);
      }

      //----------------------  CALLBACK FUNCTIONS -------------------------//

      public static mmStatus DataCallbackFN(IntPtr hSession, IntPtr pMediaSample, IntPtr pUserData)
      {
         Task.Run(() => DataCallbackFNEx(hSession, pMediaSample, pUserData));
         return mmStatus.MM_STS_NONE;
      }
      private static void DataCallbackFNEx(IntPtr hSession, IntPtr pMediaSample, IntPtr pUserData)
      {
      }

      public static void StatusCallbackFN(IntPtr hSession, uint status, IntPtr pMessage, IntPtr pUserData)
      {
         Task.Run(() => StatusCallbackFNEx(hSession, status, pMessage, pUserData));
      }
      private static void StatusCallbackFNEx(IntPtr hSession, uint status, IntPtr pMessage, IntPtr pUserData)
      {
      }

      //----------------------  MISC. FUNCTIONS -------------------------//

      public static bool GrantAccessDir(string dirPath)
      {
         DirectoryInfo dInfo = new DirectoryInfo(dirPath);
         DirectorySecurity dSecurity = dInfo.GetAccessControl();
         dSecurity.AddAccessRule(new FileSystemAccessRule(
             new SecurityIdentifier(WellKnownSidType.WorldSid, null),
             FileSystemRights.FullControl,
             InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
             PropagationFlags.NoPropagateInherit,
             AccessControlType.Allow));

         dInfo.SetAccessControl(dSecurity);
         return true;
      }
      public static bool GrantAccessFile(string filePath)
      {
         FileInfo dInfo = new FileInfo(filePath);
         FileSecurity fSecurity = dInfo.GetAccessControl();

         // Add the FileSystemAccessRule to the security settings.
         fSecurity.AddAccessRule(new FileSystemAccessRule(Environment.UserName, FileSystemRights.FullControl, AccessControlType.Allow));

         // Set the new access settings.
         dInfo.SetAccessControl(fSecurity);
         return true;
      }
   }
}
