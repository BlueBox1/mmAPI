//------------------------------------------------------------------------------
//
// app_decode_client_sample_1d
//
// Request callback data for the source and decoded frames
//
//------------------------------------------------------------------------------

//#define USING_OPEN_CV

#include <Windows.h>
#include <stdio.h>

#if defined(_M_X64) && defined (USING_OPEN_CV)
#include "opencv2/core/core.hpp"
#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"
using namespace cv;
using namespace std;
#endif

#include <mmstatus.h>
#include <mmtypes.h>
#include <mmapi.h>

//------------------------------------------------------------------------------

TCHAR gSamplePath[MAX_PATH];
TCHAR* gPSamplePath = NULL;
BOOL gPlay = TRUE;
UINT gUserCount = 3;
UINT gUserInterval = 3;
UINT gSamples = 0;
UINT gSampled = 0;
void* gPBuffer = NULL;
mmStatus gError = MM_STS_NONE;

BOOL WINAPI HandlerRoutine(DWORD dwCtrlType)
{
   gPlay = FALSE;
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
      gPlay = FALSE;
   }
}

//------------------------------------------------------------------------------

MM_DATA_CBKFN MediaSrcDataFn;
mmStatus MM_CBKAPI MediaSrcDataFn(HSESSION hSession, MM_DATA* pData, void* pUserData)
{
   printf("\ntime stamp %f\n", pData->TimeStamp);
   printf("fourcc 0x%x\n", pData->FourCC);
   printf("flag 0x%x\n", pData->ContextFlag);
   printf("length %d\n", pData->DataLength);
   printf("height %d\n", pData->Height);
   printf("width %d\n", pData->Width);
   printf("pitch %d\n", pData->Pitch[0]);
   printf("data 0x%p\n", pData->PData[0]);

   // ignore uncompressed frames and corrupt compressed frames
   if(!(pData->ContextFlag & MM_DATA_CONTEXT_UNCOMPRESSED_VIDEO) || 
       (pData->ContextFlag & MM_DATA_CONTEXT_UNCOMPRESSED_VIDEO_CORRUPTION))
      return MM_STS_NONE;

   gSamples++;
   if (gSamples % gUserInterval != 0)
      return MM_STS_NONE;

   gSampled++;
   if (gSampled > gUserCount)
   {
      gPlay = FALSE;
      return MM_STS_NONE;
   }

   if (gPBuffer)
      free(gPBuffer);

   gPBuffer = malloc(pData->Width * pData->Height * 3 / 2);
   if (!gPBuffer)
      return MM_STS_NONE;

   BYTE* pict = (BYTE*)gPBuffer;

   switch (pData->FourCC)
   {
      case MAKEFOURCC('n', 'v', '1', '2'):
      case MAKEFOURCC('N', 'V', '1', '2'):
      case MAKEFOURCC('n', 'v', '2', '1'):
      case MAKEFOURCC('N', 'V', '2', '1'):
      {
         BYTE* Y = pData->PData[0];
         BYTE* UV = pData->PData[1];

         for (unsigned y = 0; y < pData->Height; y++) // y plane
         {
            memcpy(pict, Y, pData->Width);
            pict += pData->Width;
            Y += pData->Pitch[0];
         }
         for (unsigned y = 0; y < pData->Height / 2; y++) // uv plane (interleaved)
         {
            memcpy(pict, UV, pData->Width);
            pict += pData->Width;
            UV += pData->Pitch[1];
         }
         break;
      }
      case MAKEFOURCC('y', 'v', '1', '2'):
      case MAKEFOURCC('Y', 'V', '1', '2'):
      {
         BYTE* Y = pData->PData[0];
         BYTE* U = pData->PData[1];
         BYTE* V = pData->PData[2]; 

         for (unsigned y = 0; y < pData->Height; y++) // y plane
         {
            memcpy(pict, Y, pData->Width);
            pict += pData->Width;
            Y += pData->Pitch[0];
         }
         for (unsigned y = 0; y < pData->Height / 2; y++) // v plane
         {
            memcpy(pict, V, pData->Width / 2);
            pict += pData->Width / 2;
            V += pData->Pitch[2];
         }
         for (unsigned y = 0; y <pData->Height / 2; y++) // u plane
         {
            memcpy(pict, U, pData->Width / 2);
            pict += pData->Width / 2;
            U += pData->Pitch[1];
         }
         break;
      }
      case MAKEFOURCC('i', '4', '2', '0'):
      case MAKEFOURCC('I', '4', '2', '0'):
      case MAKEFOURCC('i', 'y', 'u', 'v'):
      case MAKEFOURCC('I', 'Y', 'U', 'V'):
      {
         BYTE* Y = pData->PData[0];
         BYTE* U = pData->PData[1];
         BYTE* V = pData->PData[2];

         for (unsigned y = 0; y < pData->Height; y++) // y plane
         {
            memcpy(pict, Y, pData->Width);
            pict += pData->Width;
            Y += pData->Pitch[0];
         }
         for (unsigned y = 0; y <pData->Height / 2; y++) // u plane
         {
            memcpy(pict, U, pData->Width / 2);
            pict += pData->Width / 2;
            U += pData->Pitch[1];
         }
         for (unsigned y = 0; y < pData->Height / 2; y++) // v plane
         {
            memcpy(pict, V, pData->Width / 2);
            pict += pData->Width / 2;
            V += pData->Pitch[2];
         }
         break;
      }
      default:
         printf("fourcc not processed %d\n", pData->FourCC);
         gPlay = FALSE;
         gError = MM_STS_SRC_ERROR_INVALID_DATA;
         return MM_STS_NONE;
   }

#if defined(_M_X64) && defined (USING_OPEN_CV)

   ColorConversionCodes cs;

   switch (pData->FourCC)
   {
   case MAKEFOURCC('n', 'v', '1', '2'):
   case MAKEFOURCC('N', 'V', '1', '2'):
   case MAKEFOURCC('n', 'v', '2', '1'):
   case MAKEFOURCC('N', 'V', '2', '1'):
   {
      cs = COLOR_YUV2BGR_NV12;
      break;
   }
   case MAKEFOURCC('y', 'v', '1', '2'):
   case MAKEFOURCC('Y', 'V', '1', '2'):
   {
      cs = COLOR_YUV2BGR_YV12;
      break;
   }
   case MAKEFOURCC('i', '4', '2', '0'):
   case MAKEFOURCC('I', '4', '2', '0'):
   case MAKEFOURCC('i', 'y', 'u', 'v'):
   case MAKEFOURCC('I', 'Y', 'U', 'V'):
   {
      cs = COLOR_YUV2BGR_I420;
      break;
   }
   default:
      return MM_STS_NONE;
   }

   sprintf(gSamplePath, "%s_%d.bmp", gPSamplePath, gSampled);
   cv::Mat picYV12 = cv::Mat(pData->Height * 3 / 2, pData->Width, CV_8UC1, gPBuffer);
   cv::Mat picBGR;
   cv::cvtColor(picYV12, picBGR, cs);
   cv::imwrite(gSamplePath, picBGR);

#endif
   return MM_STS_NONE;
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
   openParms.PDataCBKFN = MediaSrcDataFn;
   openParms.PDataUserData = NULL;
   openParms.PStatusCBKFN = MediaSrcStatusFn;
   openParms.PStatusUserData = (void*)hWnd;
   openParms.OpenFlags = MM_CLIENT_REQUEST_VIDEO_SOURCE | MM_CLIENT_REQUEST_VIDEO_DECODED;
   openParms.PURL = pURL;
   openParms.HWnd = hWnd;
   openParms.BRepeat = 1;
   openParms.BNetTCP = 1;
   openParms.CacheVideo = cache; // 1000 = 1 second
   
   sts = mmClientOpen(&hSession, &openParms);
   if (sts == MM_STS_NONE)
   {
      MM_CLIENT_PLAY playParms;
      memset(&playParms, 0, sizeof(MM_CLIENT_PLAY));

      playParms.Size = sizeof(MM_CLIENT_PLAY);
      //playParms.StartTime = "19961108T143720.25Z";
      //playParms.EndTime = "19961108T143750.25Z";
      //playParms.StartTime = "5.33";
      //playParms.EndTime = "15.33";
      playParms.PStartTime = pStartTime;
      playParms.PEndTime = pEndTime;
      playParms.Speed = MM_CLIENT_SPEED_ONE;
     
      sts = mmClientPlay(hSession, &playParms);
      if (sts == MM_STS_NONE)
         *pSession = hSession;
   }
   return sts;
}

//------------------------------------------------------------------------------

int main(int argc, char** argv)
{
   HINTERFACE hInterface;
   MM_VERSION version;
   MM_LOAD_CONTEXT flags;

   if (argc < 5) { 
      printf("Usage: %s <url> <saveCount> <saveInterval> <saveName>", argv[0]);
      printf("\nPress ENTER key to Continue\n");
      getchar();
      return 1;
   }
   
   // Add a CTRL-C handler routine.
   SetConsoleCtrlHandler(HandlerRoutine, TRUE);
   gPSamplePath = argv[4];
   gUserCount = atoi(argv[2]);
   gUserInterval = atoi(argv[3]);

   gError = mmLoad(&hInterface, &version, &flags);
   if (gError == MM_STS_NONE)
   {
      HSESSION hSession;

      gError = startSesssion(NULL, argv[1], 0, 0, 1000, &hSession);
      if (gError == MM_STS_NONE)
      {
         while (gPlay)
         {
            Sleep(1000);
         }
         mmClose(hSession);
      }
      mmRelease(hInterface);
   }

   printf("%s exiting with code 0x%x\n", argv[0], gError);
   return gError;
}

//------------------------------------------------------------------------------
