
//------------------------------------------------------------------------------
// mmAPI.h - Interface declarations for the multimedia API
//------------------------------------------------------------------------------

#pragma once  

#ifdef MM_EXPORTS  
#define MM_API __declspec(dllexport)   
#else  
#define MM_API __declspec(dllimport)   
#endif  

MM_API mmStatus mmLoad(                // load the multimedia interface
   HINTERFACE*          pModule,       // pointer to a HMODULE
   MM_VERSION*          pVersion,      // version control parameter
   MM_LOAD_CONTEXT*     pFlags);       // load context flags

MM_API mmStatus mmRelease(             // release the multimedia interface
   HINTERFACE           pModule);      // HMODULE

MM_API mmStatus mmClientOpen(          // open a multimedia client
   HSESSION*            pSession,      // returned pointer to the new session instance handle
   MM_CLIENT_OPEN*      pOpenParms);   // additional open parameters

MM_API mmStatus mmClientPause(         // place the session into a paused state
   HSESSION             hSession);     // handle to the session instance

MM_API mmStatus mmClientPlay(          // place the session into a play state
   HSESSION             hSession,      // handle to the session instance
   MM_CLIENT_PLAY*      pPlayParms);   // additional play parameters

MM_API mmStatus mmServerOpen(          // open a multimedia server
   HSESSION*            pSession,      // returned pointer to the new session instance handle
   MM_SERVER_OPEN*      pOpenParms);   // additional open parameters

MM_API mmStatus mmClose(               // close the session and all of its resources
   HSESSION             hSession);     // handle to a session instance

MM_API mmStatus mmDictionarySet(       // set a runtime attribute
   HSESSION             hSession,      // handle to a session instance
   char*                pKey,          // key to set
   char*                pValue);       // value of key to set

MM_API mmStatus mmDictionaryGet(       // get a runtime attribute
   HSESSION             hSession,      // handle to a session instance
   char*                pKey,          // key to get
   char*                pValue);       // value of key to get

//------------------------------------------------------------------------------
