using System;
using System.Runtime.InteropServices;

using MultiMedia;
using System.Text;

namespace MM.SDK
{
   public static class MMInterop
   {
      public const int HWND_TOP = 0;
      public const int HWND_BOTTOM = 1;
      public const int HWND_TOPMOST = -1;
      public const int HWND_NOTOPMOST = -2;
      
      public const int SWP_NOSIZE = 0x0001;
      public const int SWP_NOMOVE = 0x0002;
      public const int SWP_NOZORDER = 0x0004;
      public const int SWP_SHOWWINDOW = 0x0040;
      public const int SWP_NOACTIVATE = 0x0010;
      public const int SWP_HIDEWINDOW = 0X0080;

      public const int SW_HIDE = 0;
      public const int SW_MAXIMIZE = 3;
      public const int SW_MINIMIZE = 6;
      public const int SW_RESTORE = 9;
      public const int SW_SHOW = 5;

      public const uint GW_OWNER = 4;
      public const uint GW_HWNDPREV = 3;
      public const uint GW_HWNDLAST = 1;
      
      public const int GWL_STYLE = -16;
      public const int WS_SIZEBOX = 0x40000;

      public const int WM_COPYDATA = 0x004A;
      public const int WM_SETTEXT = 0x000C;
      public const int WM_CLOSE = 0x0010;
      public const int WM_SIZE = 0x0005;
      public const int WM_LBUTTONDOWN = 0x0201;
      public const int WM_WINDOWPOSCHANGED = 0x0047;

      [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
      public static extern bool SetDllDirectory(string lpFileName);

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      public static extern IntPtr GetWindow([In] IntPtr hWnd, [In] uint uCmd);

      [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      public static extern IntPtr GetConsoleWindow();

      [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool ShowWindow([In] IntPtr hWnd, [In] Int32 nCmdShow);

      [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool SetForegroundWindow([In] IntPtr hWnd);

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool SetWindowPos([In] IntPtr hWnd, [In] int hWndInsertAfter, [In] int X, [In] int Y, [In] int cx, [In] int cy, [In] int uFlags);

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool InvalidateRect([In] IntPtr hWnd, [In] IntPtr lpRect, [In] bool bErase );

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool GetWindowRect([In] IntPtr hWnd, out MM_RECT lpRect);

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool GetClientRect([In] IntPtr hWnd, out MM_RECT lpRect);

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool IsIconic(IntPtr hWnd);

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      public static extern bool IsWindowVisible(IntPtr hWnd);

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      public static extern bool IsWindow(IntPtr hWnd);

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      public static extern bool CloseWindow(IntPtr hWnd);

      [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
      public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

      public delegate bool ConsoleEventDelegate(int eventType);
      [DllImport("Kernel32", SetLastError = true)]
      public static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate handler, bool add);

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
      [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

      // Send Massage
      public static IntPtr HWND_BROADCAST = new IntPtr(0xffff);
      [StructLayout(LayoutKind.Sequential)]
      public struct COPYDATASTRUCT
      {
         public IntPtr dwData;
         public int cbData;
         public IntPtr lpData;
      }

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Auto)]
      public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

      [return: MarshalAs(UnmanagedType.Bool)]
      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Auto)]
      public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

      public enum MessageFilterInfo : uint
      {
         None = 0,
         AlreadyAllowed = 1,
         AlreadyDisAllowed = 2,
         AllowedHigher = 3
      }
      public enum ChangeWindowMessageFilterExAction : uint
      {
         Reset = 0,
         Allow = 1,
         DisAllow = 2
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct CHANGEFILTERSTRUCT
      {
         public uint size;
         public MessageFilterInfo info;
      }
      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      public static extern bool ChangeWindowMessageFilterEx(IntPtr hWnd, uint msg,
      ChangeWindowMessageFilterExAction action, ref CHANGEFILTERSTRUCT changeInfo);
      // Send Massage End

      /// <summary>
      /// API
      /// </summary>
      /// 

      public static int StringArrayToPtr(IntPtr ptr, string[] array)
      {
         int size = 0;
         for (int i = 0; i < array.Length; i++)
         {
            // Note. not UTF8
            byte[] chars = System.Text.Encoding.Unicode.GetBytes(array[i] + '\0'); 
            if (ptr != IntPtr.Zero) // incase we are just after the size
            {
               Marshal.Copy(chars, 0, ptr, chars.Length);
               ptr = new IntPtr(ptr.ToInt64() + chars.Length);
            }
            size += chars.Length;
         }
         return size;
      }
      public static int  StringToPtr(IntPtr ptr, string array)
      {
         // Note. not UTF8
         byte[] chars = System.Text.Encoding.Unicode.GetBytes(array + '\0');
         Marshal.Copy(chars, 0, ptr, chars.Length);
         return chars.Length;
      }
      public static string PtrToString(IntPtr unicodePtr)
      {
         byte[] unicodeBytes; byte[] utf8bytes;
         unicodeBytes = Encoding.Unicode.GetBytes(Marshal.PtrToStringUni(unicodePtr));
         utf8bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, unicodeBytes);
         char[] utfChars = new char[Encoding.UTF8.GetCharCount(utf8bytes, 0, utf8bytes.Length)];
         Encoding.UTF8.GetChars(utf8bytes, 0, utf8bytes.Length, utfChars, 0);
        return new string(utfChars);
      }
      public static void SetTopMostWindow(IntPtr hWnd)
      {
         SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);
      }
   
      public static void ShowConsoleWindow(int cmd)
      {
         IntPtr hWndConsole = GetConsoleWindow();
         ShowWindow(hWndConsole, cmd);
      }

      public static bool GetWindowZOrder(IntPtr hWnd, out int zOrder)
      {
         if (hWnd != IntPtr.Zero)
         {
            var lowestHwnd = GetWindow(hWnd, GW_HWNDLAST);
            var z = 0;
            var hwndTmp = lowestHwnd;
            while (hwndTmp != IntPtr.Zero)
            {
               if (hWnd == hwndTmp)
               {
                  zOrder = z;
                  return true;
               }
               hwndTmp = GetWindow(hwndTmp, GW_HWNDPREV);
               z++;
            }
         }
         zOrder = int.MinValue;
         return false;
      }
   }
}