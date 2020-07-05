//------------------------------------------------------------------------------
//
// app_decode_client_sample_1a
//
// Extensive command line parsing example, see displayHelp()
// Does not register for source data callback  
//------------------------------------------------------------------------------

#include <Windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <iostream>

#include <mmstatus.h>
#include <mmtypes.h>
#include <mmapi.h>

#include "cmdlne.h"

#define IDT_TIMER 0x7066

//------------------------------------------------------------------------------

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
   switch (message)
   {
      case WM_CLOSE:
         DestroyWindow(hWnd);
         break;
      case WM_DESTROY:
         PostQuitMessage(0);
         break;
      case WM_SIZE:
         mmDictionarySet((HSESSION)GetProp(hWnd, "hSession"), "CLI_RESET", NULL);
         break;
         // Other cases
         //...
      default:
         return DefWindowProc(hWnd, message, wParam, lParam);
   }
   return 0;
}

//------------------------------------------------------------------------------

HWND CreateDisplayWindow(char* pClassName, int x, int y, int w, int h, BOOL borders)
{
   HWND hWnd = NULL;
   WNDCLASS wc;
   ZeroMemory(&wc, sizeof(wc));
   wc.lpfnWndProc = (WNDPROC)WndProc;
   wc.hInstance = GetModuleHandle(NULL);
   wc.hbrBackground = (HBRUSH)GetStockObject(BLACK_BRUSH);
   wc.hCursor = LoadCursor(NULL, IDC_ARROW);
   wc.lpszMenuName = "SampleMainMenu";
   wc.lpszClassName = pClassName;

   if (RegisterClass(&wc))
   {
      // we need to create the window early, as the DirectX function, CreateDevice requires the hWnd
      hWnd = CreateWindow(pClassName, "Sample Media Client",
         WS_OVERLAPPEDWINDOW, x, y, w, h,
         0, 0, wc.hInstance, 0);
      
      if (!borders && hWnd)
      {
         LONG lStyle = GetWindowLong(hWnd, GWL_STYLE);
         lStyle &= ~(WS_CAPTION | WS_THICKFRAME | WS_MINIMIZE | WS_MAXIMIZE | WS_SYSMENU);
         SetWindowLong(hWnd, GWL_STYLE, lStyle);

         LONG lExStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
         lExStyle &= ~(WS_EX_DLGMODALFRAME | WS_EX_CLIENTEDGE | WS_EX_STATICEDGE);
         SetWindowLong(hWnd, GWL_EXSTYLE, lExStyle);

         // finally, to get your window to redraw with the changed styles
         SetWindowPos(hWnd, NULL, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOOWNERZORDER);
      }
   }
   return hWnd;
}

//------------------------------------------------------------------------------

MM_STATUS_CBKFN MediaSrcStatusFn;
void MM_CBKAPI MediaSrcStatusFn(HSESSION hSession, mmStatus sts, char* pMessage, void* pUserData)
{
   if (((sts & MM_STS_SRC_INFO_BASE) == MM_STS_SRC_INFO_BASE) ||
      ((sts & MM_STS_LIB_INFO_BASE) == MM_STS_LIB_INFO_BASE))
   {
      printf("Session 0x%x INFO 0x%x - %s", (unsigned)hSession, sts, pMessage);
   }
   else if (((sts & MM_STS_SRC_WARNING_BASE) == MM_STS_SRC_WARNING_BASE) ||
      ((sts & MM_STS_LIB_WARNING_BASE) == MM_STS_LIB_WARNING_BASE))
   {
      printf("Session 0x%x WARNING 0x%x - %s", (unsigned)hSession, sts, pMessage);
   }
   else if (((sts & MM_STS_SRC_ERROR_BASE) == MM_STS_SRC_ERROR_BASE) ||
      ((sts & MM_STS_LIB_ERROR_BASE) == MM_STS_LIB_ERROR_BASE))
   {
      printf("Session 0x%x ERROR 0x%x - %s", (unsigned)hSession, sts, pMessage);
      PostMessage((HWND)pUserData, WM_CLOSE, 0, 0 );
   }
}

//------------------------------------------------------------------------------

MM_DATA_CBKFN MediaSrcDataFn;
mmStatus MM_CBKAPI MediaSrcDataFn(HSESSION hSession, MM_DATA* pMediaSample, void* pUserData)
{
   printf("Media source sample time stamp %f\n", pMediaSample->TimeStamp);
   return MM_STS_NONE;
}

//------------------------------------------------------------------------------

mmStatus startSesssion(MM_CLIENT_OPEN* pOpenParms, 
      MM_CLIENT_PLAY* pPlayParms, HSESSION* pSession)
{
   mmStatus sts;
   HSESSION hSession;
   *pSession = NULL;

   sts = mmClientOpen(&hSession, pOpenParms);
   if (sts == MM_STS_NONE)
   {    
      sts = mmClientPlay(hSession, pPlayParms);
      if (sts == MM_STS_NONE)
         *pSession = hSession;
   }
   return sts;
}

//------------------------------------------------------------------------------

int main(int argc, char** argv)
{
   if (argc < 2) {
      displayHelp();
      return 1;
   }

   MM_CLIENT_OPEN openParms;
   MM_CLIENT_PLAY playParms;

   if (!parseDefaultsandCmdLine(argc, argv, &openParms, &playParms)) {
      displayHelp();
      return 1;
   }

   openParms.HWnd = CreateDisplayWindow("SampleClassName", gx, gy, gw, gh, gBorders);
   if (!openParms.HWnd)
      return GetLastError();

   openParms.PStatusCBKFN = MediaSrcStatusFn;
   openParms.PStatusUserData = (void*)openParms.HWnd;
   openParms.PDataCBKFN = MediaSrcDataFn;
   openParms.PDataUserData = NULL;
   openParms.OpenFlags = MM_CLIENT_REQUEST_VIDEO_TO_DISPLAY;
       
   mmStatus sts;
   MM_LOAD load = { 0, };
   load.Size = sizeof(MM_LOAD);
   sts = mmLoad(&load);
   if (sts == MM_STS_NONE)
   {
      HSESSION hSession;

      sts = startSesssion(&openParms, &playParms, &hSession);
      if (sts == MM_STS_NONE)
      {
         MSG msg = { 0 };
         BOOL bRet;

         SetProp((HWND)openParms.HWnd, "hSession", (HANDLE)hSession);
         SetWindowText((HWND)openParms.HWnd, openParms.PURL);
         ShowWindow((HWND)openParms.HWnd, SW_SHOWNORMAL);
         UpdateWindow((HWND)openParms.HWnd);
         if(gReserved) // used for internal testing
            SetTimer((HWND)openParms.HWnd, IDT_TIMER, 5000, NULL);
         else
            ShowWindow(GetConsoleWindow(), SW_MINIMIZE);

         while ((bRet = GetMessage(&msg, (HWND)openParms.HWnd, 0, 0)) != 0)
         {
            if (bRet == -1 || msg.message == WM_TIMER)
            {
               break;
            }
            else
            {
               TranslateMessage(&msg);
               DispatchMessage(&msg);
            }
         }
         RemoveProp((HWND)openParms.HWnd, "hSession");
         sts = mmClose(hSession);
      }
      mmRelease(load.HInterface);
   }
   
   printf("%s exiting with code 0x%x\n", argv[0], sts);
   do
   {
      std::cout << '\n' << "press ENTER to continue...";
   } while (!gReserved && std::cin.get() != '\n');
   return sts;
}

//------------------------------------------------------------------------------