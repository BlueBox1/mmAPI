//------------------------------------------------------------------------------
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------


#include <Windows.h>
#include <stdio.h>
#include <mmstatus.h>
#include <mmtypes.h>
#include <mmapi.h>

#include "cmdlne.h"
#include "parse.h"

char gURL[256];
char gStartTime[256];
char gEndTime[256];
LONG gx = CW_USEDEFAULT;
LONG gy = CW_USEDEFAULT;
LONG gw = 1280;
LONG gh = 720;
BOOL gBorders = FALSE;
unsigned gReserved = 0;

//------------------------------------------------------------------------------

void displayHelp()
{
   printf("\n");
   printf("Usage:\n\n");
   printf("Application parameters:\n");
   printf("\t-url=<path-to-src>  - file path or network address.\n");
   printf("\t-repeat=n           - repeat media if EOF is returned, 1==ON 0==OFF.\n");
   printf("\t-net_tcp=n          - stream over tcp or udp for networked streams, 1==ON 0==OFF.\n");
   printf("\t-cache_video=n      - milliseconds cache used for the smooth playback of networked streams.\n");
   printf("\t-start_time=n       - see documentation for further details.\n");
   printf("\t-end_time=n         - see documentation for further details.\n");
   printf("\t-speed=n            - enumerated type of MM_CLIENT_SPEED when supported.\n");
   printf("\t-reverse=n          - forward or reverse playout direction when supported, 1==ON 0==OFF.\n");
   printf("\t-x=n                - x position of the window.\n");
   printf("\t-y=n                - y position of the window.\n");
   printf("\t-w=n                - width of the window.\n");
   printf("\t-h=n                - height of the window.\n");
   printf("\t-borders=n          - show borders and title bar, 1==ON 0==OFF.\n");

   printf("\nPress ENTER key to Continue\n");
   getchar();
}

//------------------------------------------------------------------------------

void SetDefaults(MM_CLIENT_OPEN* pOpenParms, MM_CLIENT_PLAY* pPlayParms)
{
   memset(pOpenParms, 0, sizeof(MM_CLIENT_OPEN));

   pOpenParms->Size = sizeof(MM_CLIENT_OPEN);
   pOpenParms->BRepeat = 1;
   pOpenParms->BNetTCP = 0;
   pOpenParms->CacheVideo = 1000; // 1 second

   memset(pPlayParms, 0, sizeof(MM_CLIENT_PLAY));

   pPlayParms->Size = sizeof(MM_CLIENT_PLAY);
   pPlayParms->PStartTime = NULL;
   pPlayParms->PEndTime = NULL;
   pPlayParms->Speed = MM_CLIENT_SPEED_ONE;
   pPlayParms->BReverse = 0;
}

//------------------------------------------------------------------------------

BOOL parseDefaultsandCmdLine(int argc, char *argv[], MM_CLIENT_OPEN* pOpenParms, MM_CLIENT_PLAY* pPlayParms)
{
   char *temp;
   BOOL bSuppliedURL = FALSE;
   SetDefaults(pOpenParms, pPlayParms);

   for (int n = 0; n < argc; n++)
   {
      printf("argv[%d] = %s\n", n, argv[n]);
   }

   if (checkCmdLineFlag(argc, (const char **)argv, "help"))
   {
      displayHelp();
      return FALSE;
   }

   if (checkCmdLineFlag(argc, (const char **)argv, "url"))
   {
      getCmdLineArgumentString(argc, (const char **)argv, "url", &temp);
      strcpy_s(gURL, sizeof(gURL), temp);
      pOpenParms->PURL = gURL;
      bSuppliedURL = TRUE;
   }
   if (checkCmdLineFlag(argc, (const char **)argv, "start_time"))
   {
      getCmdLineArgumentString(argc, (const char **)argv, "start_time", &temp);
      strcpy_s(gStartTime, sizeof(gStartTime), temp);
      pPlayParms->PStartTime = gStartTime;
   }
   if (checkCmdLineFlag(argc, (const char **)argv, "end_time"))
   {
      getCmdLineArgumentString(argc, (const char **)argv, "end_time", &temp);
      strcpy_s(gEndTime, sizeof(gEndTime), temp);
      pPlayParms->PEndTime = gEndTime;
   }

   if (checkCmdLineFlag(argc, (const char **)argv, "repeat"))
      pOpenParms->BRepeat = getCmdLineArgumentInt(argc, (const char **)argv, "repeat");
   if (checkCmdLineFlag(argc, (const char **)argv, "net_tcp"))
      pOpenParms->BNetTCP = getCmdLineArgumentInt(argc, (const char **)argv, "net_tcp");
   if (checkCmdLineFlag(argc, (const char **)argv, "reverse"))
      pPlayParms->BReverse = getCmdLineArgumentInt(argc, (const char **)argv, "reverse");
   if (checkCmdLineFlag(argc, (const char **)argv, "borders"))
      gBorders = getCmdLineArgumentInt(argc, (const char **)argv, "borders");
   if (checkCmdLineFlag(argc, (const char **)argv, "cache_video"))
      pOpenParms->CacheVideo = getCmdLineArgumentInt(argc, (const char **)argv, "cache_video");
   if (checkCmdLineFlag(argc, (const char **)argv, "speed"))
      pPlayParms->Speed = (MM_CLIENT_SPEED)getCmdLineArgumentInt(argc, (const char **)argv, "speed");
   if (checkCmdLineFlag(argc, (const char **)argv, "x"))
      gx = getCmdLineArgumentInt(argc, (const char **)argv, "x");
   if (checkCmdLineFlag(argc, (const char **)argv, "y"))
      gy = getCmdLineArgumentInt(argc, (const char **)argv, "y");
   if (checkCmdLineFlag(argc, (const char **)argv, "w"))
      gw = getCmdLineArgumentInt(argc, (const char **)argv, "w");
   if (checkCmdLineFlag(argc, (const char **)argv, "h"))
      gh = getCmdLineArgumentInt(argc, (const char **)argv, "h");
   if (checkCmdLineFlag(argc, (const char **)argv, "reserved"))
   {
      gReserved = getCmdLineArgumentInt(argc, (const char **)argv, "reserved");
      pOpenParms->PReserved = &gReserved;
   }

   return bSuppliedURL;
}

//------------------------------------------------------------------------------