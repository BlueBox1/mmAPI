//------------------------------------------------------------------------------
//
// app_decode_client_sample_1
//
// Test PLAY || DEWARP || ZOOM || CHILD parameters
//
// React to key press events to display functionality 
// PLAY functionality relies on video with a valid duration, ie not an elemental video stream such as .h264
//
//------------------------------------------------------------------------------

#include <Windows.h>
#include <stdio.h>
#include <string>

#include <mmstatus.h>
#include <mmtypes.h>
#include <mmapi.h>

//------------------------------------------------------------------------------

MM_CLIENT_PLAY gPlayParms = { 0, };
HSESSION gHSession = NULL;
int gSpeed = MM_CLIENT_SPEED_ONE;
MM_CLIENT_DEWARP gDewarp = { 0, };
HWND ghWnd[255];
unsigned ghWndCounter = 0;
int gX, gY, gW, gH;

#define DECODE_ROUTE_AUTO 1
#define DECODE_ROUTE_FFMPEG 4
#define DECODE_ROUTE_INTEL 8
#define DECODE_ROUTE_AMD 16
#define DECODE_ROUTE_NVIDIA 32
#define DECODE_ROUTE_VCM 64
#define DECODE_ROUTE_INTEL_SOFT 128

#define SAMPLE_MODE_DEWARP 1 // else arrow keys reflect PLAY possabilities

HWND CreateDisplayWindow(char* pClassName, int x, int y, int w, int h);

//------------------------------------------------------------------------------

void SetDewarp(HWND hWnd)
{
   if (hWnd != ghWnd[0])
   {
      MM_CLIENT_CHILD child;
      child.Size = sizeof(MM_CLIENT_CHILD);
      child.HWnd = hWnd;
      child.Flags = MM_CLIENT_CHILD_DEWARP;
      child.Dewarp = gDewarp;
      mmDictionarySet(gHSession, "CLI_CHILD", (char*)&child);
   }
   else
      mmDictionarySet(gHSession, "CLI_DEWARP", (char*)&gDewarp);
}

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
   switch (message)
   {
   case WM_CLOSE:
      DestroyWindow(hWnd);
      break;
   case WM_DESTROY:
      PostQuitMessage(0);
      ghWndCounter--;
      break;
   case WM_SIZE:
   {
      if (!gHSession)
         break;
      if (hWnd == ghWnd[0])
         mmDictionarySet(gHSession, "cli_reset", 0);
      else
      {
         MM_CLIENT_CHILD child;
         child.Size = sizeof(MM_CLIENT_CHILD);
         child.HWnd = hWnd;
         child.Flags = MM_CLIENT_CHILD_RESET;
         mmDictionarySet(gHSession, "CLI_CHILD", (char*)&child);
      }
      break;
   }
   case WM_KEYDOWN:
   {
      switch (wParam)
      {
      case VK_NUMPAD1:
      {
         char buffer[255];
         SYSTEMTIME st;
         GetSystemTime(&st);
         sprintf(buffer, "%d%d", st.wSecond, st.wMilliseconds);
         hWnd = CreateDisplayWindow(buffer, gX, gY, gW, gH);
         ShowWindow(hWnd, SW_SHOWNORMAL);

         MM_CLIENT_CHILD child;
         child.Size = sizeof(MM_CLIENT_CHILD);
         child.HWnd = hWnd;
         child.Flags = MM_CLIENT_CHILD_OPEN;
         mmDictionarySet(gHSession, "CLI_CHILD", (char*)&child);
         break;
      }
      case VK_NUMPAD2:
      {
         if (gDewarp.BSessionEnabled)
            gDewarp.BSessionEnabled = 0;
         else
            gDewarp.BSessionEnabled = 1;

         SetDewarp(hWnd);
         break;
      }
      case VK_NUMPAD3:
      {
         MM_RECT rect;
         rect = { 20,20,60,60 };

         if (hWnd != ghWnd[0])
         {
            MM_CLIENT_CHILD child;
            child.Size = sizeof(MM_CLIENT_CHILD);
            child.HWnd = hWnd;
            child.Flags = MM_CLIENT_CHILD_ZOOM;
            child.Zoom = rect;
            mmDictionarySet(gHSession, "CLI_CHILD", (char*)&child);
         }
         else
            mmDictionarySet(gHSession, "CLI_ZOOM", (char*)&rect);

         break;
      }
      case VK_NUMPAD4:
      {
         MM_RECT rect;
         rect = { 0,0,100,100 };

         if (hWnd != ghWnd[0])
         {
            MM_CLIENT_CHILD child;
            child.Size = sizeof(MM_CLIENT_CHILD);
            child.HWnd = hWnd;
            child.Flags = MM_CLIENT_CHILD_ZOOM;
            child.Zoom = rect;
            mmDictionarySet(gHSession, "CLI_CHILD", (char*)&child);
         }
         else
            mmDictionarySet(gHSession, "CLI_ZOOM", (char*)&rect);

         break;
      }
      case VK_NUMPAD5:
      {
         if (hWnd != ghWnd[0])
         {
            MM_CLIENT_CHILD child;
            child.Size = sizeof(MM_CLIENT_CHILD);
            child.HWnd = hWnd;
            child.Flags = MM_CLIENT_CHILD_CLOSE;
            mmDictionarySet(gHSession, "CLI_CHILD", (char*)&child);
         }

         DestroyWindow(hWnd);
         break;
      }
      case VK_NUMPAD6:
      {
         if (hWnd != ghWnd[0])
         {
            MM_CLIENT_CHILD child;
            child.Size = sizeof(MM_CLIENT_CHILD);
            child.HWnd = hWnd;
            child.Flags = MM_CLIENT_CHILD_SOURCE;

            child.Source.Size = sizeof(MM_CLIENT_SOURCE);
            child.Source.Flags = MM_CLIENT_SOURCE_RENDER;
            child.Source.BRender = 1;

            mmDictionarySet(gHSession, "CLI_CHILD", (char*)&child);
         }
         else
         {
            MM_CLIENT_SOURCE source;
            source.Size = sizeof(MM_CLIENT_SOURCE);
            source.Flags = MM_CLIENT_SOURCE_RENDER;
            source.BRender = 1;

            mmDictionarySet(gHSession, "CLI_SOURCE", (char*)&source);
         }
         break;
      }
      case VK_NUMPAD7:
      {
         if (hWnd != ghWnd[0])
         {
            MM_CLIENT_CHILD child;
            child.Size = sizeof(MM_CLIENT_CHILD);
            child.HWnd = hWnd;
            child.Flags = MM_CLIENT_CHILD_SOURCE;

            child.Source.Size = sizeof(MM_CLIENT_SOURCE);
            child.Source.Flags = MM_CLIENT_SOURCE_RENDER;
            child.Source.BRender = 0;

            mmDictionarySet(gHSession, "CLI_CHILD", (char*)&child);
         }
         else
         {
            MM_CLIENT_SOURCE source;
            source.Size = sizeof(MM_CLIENT_SOURCE);
            source.Flags = MM_CLIENT_SOURCE_RENDER;
            source.BRender = 0;

            mmDictionarySet(gHSession, "CLI_SOURCE", (char*)&source);
         }
         break;
      }
      case VK_NUMPAD8:
      {
         if (hWnd != ghWnd[0])
         {
            MM_CLIENT_CHILD child;
            child.Size = sizeof(MM_CLIENT_CHILD);
            child.HWnd = hWnd;
            child.Flags = MM_CLIENT_CHILD_SOURCE;

            child.Source.Size = sizeof(MM_CLIENT_SOURCE);
            child.Source.Flags = MM_CLIENT_SOURCE_ASPECT_RATIO;
            child.Source.BEnforceAR = 1;

            mmDictionarySet(gHSession, "CLI_CHILD", (char*)&child);
         }
         else
         {
            MM_CLIENT_SOURCE source;
            source.Size = sizeof(MM_CLIENT_SOURCE);
            source.Flags = MM_CLIENT_SOURCE_ASPECT_RATIO;
            source.BEnforceAR = 1;

            mmDictionarySet(gHSession, "CLI_SOURCE", (char*)&source);
         }
         break;
      }
      case VK_NUMPAD9:
      {
         if (hWnd != ghWnd[0])
         {
            MM_CLIENT_CHILD child;
            child.Size = sizeof(MM_CLIENT_CHILD);
            child.HWnd = hWnd;
            child.Flags = MM_CLIENT_CHILD_SOURCE;

            child.Source.Size = sizeof(MM_CLIENT_SOURCE);
            child.Source.Flags = MM_CLIENT_SOURCE_ASPECT_RATIO;
            child.Source.BEnforceAR = 0;

            mmDictionarySet(gHSession, "CLI_CHILD", (char*)&child);
         }
         else
         {
            MM_CLIENT_SOURCE source;
            source.Size = sizeof(MM_CLIENT_SOURCE);
            source.Flags = MM_CLIENT_SOURCE_ASPECT_RATIO;
            source.BEnforceAR = 0;

            mmDictionarySet(gHSession, "CLI_SOURCE", (char*)&source);
         }
         break;
      }
      case VK_UP:
      {
#if SAMPLE_MODE_DEWARP
         gDewarp.YAngle += 0.05f;
         SetDewarp(hWnd);
#else
         gSpeed++;
         if (gSpeed > MM_CLIENT_SPEED_THIRTYTWO)
            gSpeed = MM_CLIENT_SPEED_QUARTER;

         // continue current stream position
         gPlayParms.PStartTime = NULL;
         //gPlayParms.Speed = (MM_CLIENT_SPEED)gSpeed;
         mmClientPlay(gHSession, &gPlayParms);
#endif
         break;
      }
      case VK_DOWN:
      {
#if SAMPLE_MODE_DEWARP
         gDewarp.YAngle -= 0.05f;
         SetDewarp(hWnd);
#else
         mmClientPause(gHSession);
#endif
         break;
      }
      case VK_LEFT:
      {
#if SAMPLE_MODE_DEWARP
         gDewarp.XAngle -= 0.05f;
         SetDewarp(hWnd);
#else
         // potential new start time, internal buffers flushed
         gPlayParms.PStartTime = "20.00";
         mmClientPlay(gHSession, &gPlayParms);
#endif
         break;
      }
      case VK_RIGHT:
      {
#if SAMPLE_MODE_DEWARP
         gDewarp.XAngle += 0.05f;
         SetDewarp(hWnd);
#else
         // continue current stream position
         gPlayParms.PStartTime = NULL;
         gPlayParms.Speed = (MM_CLIENT_SPEED)gSpeed;
         mmClientPlay(gHSession, &gPlayParms);
#endif
         break;
      }
      case VK_F1:
      {
         gDewarp.FOV += 0.05f;
         SetDewarp(hWnd);
         break;
      }
      case VK_F2:
      {
         gDewarp.FOV -= 0.05f;
         SetDewarp(hWnd);
         break;
      }
      case VK_F3:
      {
         gDewarp.HStretch += 0.05f;
         SetDewarp(hWnd);
         break;
      }
      case VK_F4:
      {
         gDewarp.HStretch -= 0.05f;
         SetDewarp(hWnd);
         break;
      }
      case VK_F5:
      {
         gDewarp.FOVRot += 0.05f;
         SetDewarp(hWnd);
         break;
      }
      case VK_F6:
      {
         gDewarp.FOVRot -= 0.05f;
         SetDewarp(hWnd);
         break;
      }
      case VK_SPACE:
      {       
         break;
      }
      }
   }
   default:
      return DefWindowProc(hWnd, message, wParam, lParam);
   }
   return 0;
}

//------------------------------------------------------------------------------

HWND CreateDisplayWindow(char* pClassName, int x, int y, int w, int h)
{
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
      ghWnd[ghWndCounter++] = CreateWindow(pClassName, "Sample Media Client",
         WS_OVERLAPPEDWINDOW, x, y, w, h,
         0, 0, wc.hInstance, 0);

      return ghWnd[ghWndCounter - 1];
   }
   return NULL;
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
      PostMessage((HWND)pUserData, WM_CLOSE, 0, 0);
   }
}

//------------------------------------------------------------------------------

mmStatus startSesssion(HWND hWnd, char* pURL, char* pStartTime, char* pEndTime, unsigned cache, HSESSION* pSession)
{
   mmStatus sts;
   HSESSION hSession;
   MM_CLIENT_OPEN openParms;
   memset(&openParms, 0, sizeof(MM_CLIENT_OPEN));
   *pSession = NULL;

   openParms.Size = sizeof(MM_CLIENT_OPEN);
   openParms.PDataCBKFN = NULL;
   openParms.PDataUserData = NULL;
   openParms.PStatusCBKFN = MediaSrcStatusFn;
   openParms.PStatusUserData = (void*)hWnd;
   openParms.OpenFlags = MM_CLIENT_REQUEST_VIDEO_TO_DISPLAY;
   openParms.PURL = pURL;
   openParms.HWnd = hWnd;
   openParms.BRepeat = 1;
   openParms.BNetTCP = 1;
   openParms.CacheVideo = cache; // 1000 = 1 second
   
   // force specific decode route in favour of auto
   int forceDecode = DECODE_ROUTE_AUTO;// DECODE_ROUTE_FFMPEG;// DECODE_ROUTE_AUTO;
   openParms.PReserved = &forceDecode;

   sts = mmClientOpen(&hSession, &openParms);
   if (sts == MM_STS_NONE)
   {
      memset(&gPlayParms, 0, sizeof(MM_CLIENT_PLAY));

      gPlayParms.Size = sizeof(MM_CLIENT_PLAY);
      //gPlayParms.StartTime = "19961108T143720.25Z";
      //gPlayParms.EndTime = "19961108T143750.25Z";
      //gPlayParms.StartTime = "5.33";
      //gPlayParms.EndTime = "15.33";
      gPlayParms.PStartTime = pStartTime;
      gPlayParms.PEndTime = pEndTime;
      gPlayParms.Speed = MM_CLIENT_SPEED_ONE;
      gPlayParms.BReverse = 0;
     
      sts = mmClientPlay(hSession, &gPlayParms);
      if (sts == MM_STS_NONE)
         *pSession = hSession;
   }
   return sts;
}

//------------------------------------------------------------------------------

int main(int argc, char** argv)
{
   if (argc < 6) { 
      printf("Usage: %s <url> <x> <y> <w> <h>\n", argv[0]);
      printf("\nPress ENTER key to Continue\n");
      getchar();
      return 1;
   }

   gX = std::stoi(argv[2], NULL, 10);
   gY = std::stoi(argv[3], NULL, 10);
   gW = std::stoi(argv[4], NULL, 10);
   gH = std::stoi(argv[5], NULL, 10);

   HWND hWnd = CreateDisplayWindow("SampleClassName", gX, gY, gW, gH);
   ShowWindow(hWnd, SW_SHOWNORMAL);
#if !_DEBUG
   ShowWindow(GetConsoleWindow(), SW_MINIMIZE);
#endif

   mmStatus sts;
   MM_LOAD load = { 0, };
   load.Size = sizeof(MM_LOAD);
   sts = mmLoad(&load);
   if (sts == MM_STS_NONE)
   {
      sts = startSesssion(hWnd, argv[1], 0, 0, 1000, &gHSession);
      if (sts == MM_STS_NONE)
      {
         MSG msg = { 0 };

         ShowWindow(hWnd, SW_SHOWNORMAL);
         UpdateWindow(hWnd);

#if SAMPLE_MODE_DEWARP
         gDewarp.Size = sizeof(MM_CLIENT_DEWARP);
         gDewarp.BSessionEnabled = 0;
         gDewarp.BHorViewMode = 1; // 1=ceiling - 2=floor/table - 0=wall 
         gDewarp.PanoFactor = 1; // 1= 90 - 2= 180 - 4=360
         gDewarp.XCenter = 0.5f;
         gDewarp.YCenter = 0.5f;
         gDewarp.Radius = 0.36f;
         // keyboard options
         gDewarp.FOVRot = 0.0f;
         gDewarp.HStretch = 1.0f;
         gDewarp.XAngle = 0.0f;
         gDewarp.YAngle = 1.0f;
         gDewarp.FOV = 1.2f;

         mmDictionarySet(gHSession,"CLI_DEWARP", (char*)&gDewarp);
#endif

         while (ghWndCounter)
         {
            Sleep(100); // do not overload thread
            int n = 0;
            while (ghWnd[n])
            {
               if (PeekMessage(&msg, ghWnd[n], 0, 0, PM_REMOVE))
               {
                  TranslateMessage(&msg);
                  DispatchMessage(&msg);
               }
               n++;
            }
         }
         sts = mmClose(gHSession);
      }
      mmRelease(load.HInterface);
   }
   printf("%s exiting with code 0x%x\n", argv[0], sts);
   
   return sts;
}

//------------------------------------------------------------------------------
