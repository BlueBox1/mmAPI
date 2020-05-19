using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MM.SDK
{
   internal static class MMSendMessage
   {
      public const int MM_WINCMD_DEWARP            = 0x00000001;
      public const int MM_WINCMD_PLAY              = 0x00000002;
      public const int MM_WINCMD_PAUSE             = 0x00000004;
      public const int MM_WINCMD_OPEN              = 0x00000005;
      public const int MM_WINCMD_WINDOW            = 0x00000006;
      public const int MM_WINCMD_ZOOM              = 0x00000007;
      public const int MM_WINCMD_SOURCE            = 0x00000008;
 
      public enum MM_WIN_TXT_FLAG
      {
         MM_TITLE = 0x1,
         MM_STATUS = 0x2,
         MM_WDOG = 0x4,
         MM_OSD = 0x8,
      };

      public static void SendMessage(uint id, IntPtr hWnd, uint command, IntPtr data, int dataSize )
      {
         if (hWnd == IntPtr.Zero)
            return;

         IntPtr ptrCopyData = IntPtr.Zero;
         try
         {
            // Create the data structure and fill with data
            MMInterop.COPYDATASTRUCT copyData = new MMInterop.COPYDATASTRUCT();
            copyData.dwData = new IntPtr(command);
            copyData.cbData = dataSize;
            copyData.lpData = data;

            // Allocate memory for the data and copy
            ptrCopyData = Marshal.AllocCoTaskMem(Marshal.SizeOf(copyData));
            Marshal.StructureToPtr(copyData, ptrCopyData, false);

            // Send the message
            IntPtr result = MMInterop.SendMessage(hWnd, MMInterop.WM_COPYDATA, (IntPtr)id, ptrCopyData);
         }
         catch (Exception ex)
         {
            Debug.WriteLine("SendMessage Exception " + ex.ToString());
         }
         finally
         {
            // Free the allocated memory after the contol has been returned
            if (ptrCopyData != IntPtr.Zero)
               Marshal.FreeCoTaskMem(ptrCopyData);
         }
      }
      //public static void PostMessage(uint id, IntPtr hWnd, uint command, IntPtr data, int dataSize)
      //{
      //   if (hWnd == IntPtr.Zero)
      //      return;

      //   IntPtr ptrCopyData = IntPtr.Zero;
      //   try
      //   {
      //      // Create the data structure and fill with data
      //      _MMInterop.COPYDATASTRUCT copyData = new _MMInterop.COPYDATASTRUCT();
      //      copyData.dwData = new IntPtr(command);
      //      copyData.cbData = dataSize;
      //      copyData.lpData = data;

      //      // Allocate memory for the data and copy
      //      ptrCopyData = Marshal.AllocCoTaskMem(Marshal.SizeOf(copyData));
      //      Marshal.StructureToPtr(copyData, ptrCopyData, false);

      //      // Send the message
      //      bool bRet = _MMInterop.PostMessage(hWnd, _MMInterop.WM_COPYDATA, (IntPtr)id, ptrCopyData);
      //      if (!bRet)
      //      {
      //         // An error occured
      //         throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
      //      }
      //   }
      //   catch (Exception ex)
      //   {
      //      Debug.WriteLine("PostMessage Exception " + ex.ToString());
      //   }
      //   finally
      //   {
      //      // Free the allocated memory after the contol has been returned
      //      if (ptrCopyData != IntPtr.Zero)
      //         Marshal.FreeCoTaskMem(ptrCopyData);
      //   }
      //}
      public static void SendWindowText(IntPtr hWnd, string text, MM_WIN_TXT_FLAG flags)
      {
         IntPtr pnt = IntPtr.Zero;
         try
         {  // Copy the string to unmanaged memory.
            pnt = Marshal.StringToHGlobalUni(text);
            MMInterop.SendMessage(hWnd, MMInterop.WM_SETTEXT, (IntPtr)flags, pnt);
         }
         finally
         { // Free the unmanaged memory.
            if (pnt != IntPtr.Zero)
               Marshal.FreeHGlobal(pnt);
         }
      }
   }
}
