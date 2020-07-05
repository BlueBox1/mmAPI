//------------------------------------------------------------------------------
//
// app_encode_rtsp_server
//
// Screen scrape cmdline hWnd, encode and provide over rtsp, URL returned into gURL
//
//------------------------------------------------------------------------------

#include <Windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <iostream>
#include <string>

#include <mmstatus.h>
#include <mmtypes.h>
#include <mmapi.h>

//#define ENCODE_HWND 1
#define ENCODE_SCREEN 1
//#define ENCODE_DESKTOP 1

//------------------------------------------------------------------------------

char gURL[254];
HANDLE gHEvent = NULL;
BOOL WINAPI HandlerRoutine(DWORD dwCtrlType)
{
   SetEvent(gHEvent);
   return TRUE;
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
   }
}

//------------------------------------------------------------------------------

void setupSession(HWND hWnd, MM_SERVER_OPEN* pOpenParms)
{
   memset(pOpenParms, 0, sizeof(MM_SERVER_OPEN));

   pOpenParms->Size = sizeof(MM_SERVER_OPEN);
   pOpenParms->EncodeParams.Size = sizeof(pOpenParms->EncodeParams);
   pOpenParms->PStatusCBKFN = MediaSrcStatusFn;
   pOpenParms->PStatusUserData = NULL;
   pOpenParms->EncodeParams.CODEC = MM_CODEC_AVC;// MM_CODEC_AVC; // MM_CODEC_JPG  
   pOpenParms->EncodeParams.FrameRate = 10;
   pOpenParms->RTSPParams.RTSPPortNumber = 0; // use default provided
   pOpenParms->RTSPParams.BReuseSource = 1; // allow multiple connections to be optimized
   pOpenParms->RTSPParams.PURL = "app_encode_rtsp_server_url";
   pOpenParms->RTSPParams.PUserName = NULL;// "user";
   pOpenParms->RTSPParams.PPassword = NULL;// "pass";

#if ENCODE_HWND
   pOpenParms->OpenFlags = MM_SERVER_REQUEST_VIDEO_TO_RTSP | MM_SERVER_REQUEST_VIDEO_OF_HWND;  
   pOpenParms->HWnd = hWnd;
#elif ENCODE_DESKTOP
   pOpenParms->OpenFlags = MM_SERVER_REQUEST_VIDEO_TO_RTSP | MM_SERVER_REQUEST_VIDEO_OF_HWND;
   pOpenParms->HWnd = GetDesktopWindow();
#elif ENCODE_SCREEN
   pOpenParms->OpenFlags = MM_SERVER_REQUEST_VIDEO_TO_RTSP | MM_SERVER_REQUEST_VIDEO_OF_MONITOR;
   const POINT ptZero = { 0, 0 };
   pOpenParms->HMonitor = MonitorFromPoint(ptZero, MONITOR_DEFAULTTOPRIMARY);
#else
   pOpenParms->OpenFlags = MM_SERVER_REQUEST_VIDEO_TO_RTSP | MM_SERVER_REQUEST_VIDEO_OF_COORDS;
   pOpenParms->Coordinates.Top = 540;
   pOpenParms->Coordinates.Left = 960;
   pOpenParms->Coordinates.Right = 1920;
   pOpenParms->Coordinates.Bottom = 1080;
#endif

   if (pOpenParms->EncodeParams.CODEC == MM_CODEC_AVC)
   {
      pOpenParms->EncodeParams.AVCParams.KeyFrameInterval = 30; // zero represents infinite (default)
      pOpenParms->EncodeParams.AVCParams.Profile = MM_ENC_PROFILE_BASELINE;
      pOpenParms->EncodeParams.AVCParams.TargetUsage = MM_ENC_TARGET_USAGE_BALANCED;
      pOpenParms->EncodeParams.AVCParams.RateMethod = MM_ENC_RATE_METHOD_VBR;
      pOpenParms->EncodeParams.AVCParams.TargetKbps = 5000;
   }
   else if (pOpenParms->EncodeParams.CODEC == MM_CODEC_JPG)
      pOpenParms->EncodeParams.JPGParams.QFactor = 75;
}

//------------------------------------------------------------------------------

int main(int argc, char** argv)
{ 
   // Add a CTRL-C handler routine.
   SetConsoleCtrlHandler(HandlerRoutine, TRUE);
   gHEvent = CreateEvent(NULL, FALSE, FALSE, NULL);

   mmStatus sts;
   MM_LOAD load = { 0, };
   load.Size = sizeof(MM_LOAD);
   sts = mmLoad(&load);
   if (sts == MM_STS_NONE)
   {
      HSESSION hSession;
      MM_SERVER_OPEN openParms;
      HWND hWnd = NULL;

#if ENCODE_HWND
      // has the user provided a hWnd on the command line, if not, use the consoles
      hWnd = (argc < 2) ? GetConsoleWindow() : (HWND)std::stoi(argv[1], NULL, 10);
#endif

      setupSession(hWnd, &openParms);
      sts = mmServerOpen(&hSession, &openParms);
      if (sts == MM_STS_NONE)
      {
         WaitForSingleObject(gHEvent, INFINITE);
         sts = mmClose(hSession);
      }
      mmRelease(load.HInterface);
   }
   CloseHandle(gHEvent);
   printf("%s exiting with code 0x%x\n", argv[0], sts);
   do
   {
      std::cout << '\n' << "press ENTER to continue...";
   } while (std::cin.get() != '\n');
   return sts;
}

//------------------------------------------------------------------------------