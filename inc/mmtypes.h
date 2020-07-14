
//------------------------------------------------------------------------------
// mmTypes.h - Type definitions for the multimedia API
//------------------------------------------------------------------------------

#include <pshpack1.h>

#ifdef _WIN64
typedef __int64 HINTERFACE, *PHINTERFACE;  // MULTIMEDIA handle to the api interface dll
typedef __int64 HSESSION, *PHSESSION;      // MULTIMEDIA session object handle
#else
typedef int HINTERFACE, *PHINTERFACE;      // MULTIMEDIA handle to the api interface dll
typedef int HSESSION, *PHSESSION;          // MULTIMEDIA session object handle
#endif

#define MM_CBKAPI __stdcall                     // callback function return type
#define MM_DATA_PLANES 8                        // maximum number of video data planes
//------------------------------------------------------------------------------

typedef enum _MM_LOAD_CONTEXT                   // mmLoad returned context flags
{
   MM_LOAD_CONTEXT_ACTIVATED = 0x1,             // licence activated
   MM_LOAD_CONTEXT_LOGGING = 0x2,               // logging to registry specified file
   MM_LOAD_CONTEXT_DXSUPPORTED = 0x4,           // directX acceleration supported 
}  MM_LOAD_CONTEXT, *PMM_LOAD_CONTEXT;

//------------------------------------------------------------------------------

enum MM_CLIENT_REQUEST                          // mmClientOpen data request context flags
{
   MM_CLIENT_REQUEST_VIDEO_TO_DISPLAY = 0x1,    // display video in provided hWnd
   MM_CLIENT_REQUEST_AUDIO_TO_MIXER = 0x2,      // play audio to local mixer
   MM_CLIENT_REQUEST_VIDEO_SOURCE = 0x100,      // source video data to provided call back
   MM_CLIENT_REQUEST_AUDIO_SOURCE = 0x1000,     // source audio data to provided call back
   MM_CLIENT_REQUEST_VIDEO_DECODED = 0x200,     // source decoded video data to provided call back
   MM_CLIENT_REQUEST_AUDIO_DECODED = 0x2000,    // source decoded audio data to provided call back
};

//------------------------------------------------------------------------------

enum MM_SERVER_REQUEST                          // mmServerOpen data request context flags
{
   MM_SERVER_REQUEST_VIDEO_TO_RTSP = 0x1,       // transport video over RTSP
   MM_SERVER_REQUEST_VIDEO_OF_HWND = 0x2000,    // encode a window as video
   MM_SERVER_REQUEST_VIDEO_OF_COORDS = 0x4000,  // encode desktop coordinates as video
   MM_SERVER_REQUEST_VIDEO_OF_MONITOR = 0x8000, // encode a screen as video
};

//------------------------------------------------------------------------------

typedef enum _MM_CLIENT_CHILD_REQUEST
{
   MM_CLIENT_CHILD_OPEN = 0x1,
   MM_CLIENT_CHILD_CLOSE = 0x2,
   MM_CLIENT_CHILD_RESET = 0x4,
   MM_CLIENT_CHILD_DEWARP = 0x8,
   MM_CLIENT_CHILD_ZOOM = 0x10,
   MM_CLIENT_CHILD_SOURCE = 0x20,
}  MM_CLIENT_CHILD_REQUEST, *PMM_CLIENT_CHILD_REQUEST;

//------------------------------------------------------------------------------

typedef enum _MM_CLIENT_SOURCE_REQUEST
{
   MM_CLIENT_SOURCE_RENDER = 0x1,
   MM_CLIENT_SOURCE_ASPECT_RATIO = 0x2,
}  MM_CLIENT_SOURCE_REQUEST, *PMM_CLIENT_SOURCE_REQUEST;

//------------------------------------------------------------------------------

typedef enum _MM_CLIENT_SPEED                   // mmClientPlay supported playout speeds
{
   MM_CLIENT_SPEED_UNKNOWN    = 0,
   MM_CLIENT_SPEED_QUARTER    = 1,
   MM_CLIENT_SPEED_HALF       = 2,
   MM_CLIENT_SPEED_ONE        = 3,
   MM_CLIENT_SPEED_TWO        = 4,
   MM_CLIENT_SPEED_FOUR       = 5,
   MM_CLIENT_SPEED_EIGHT      = 6,
   MM_CLIENT_SPEED_SIXTEEN    = 7,
   MM_CLIENT_SPEED_THIRTYTWO  = 8,
}  MM_CLIENT_SPEED, *PMM_CLIENT_SPEED;

//------------------------------------------------------------------------------

typedef enum _MM_CODEC_ENCODE
{
   MM_CODEC_UNKNOWN  = 0,
   MM_CODEC_AVC      = 1,
   MM_CODEC_JPG      = 2,
   MM_CODEC_HEVC     = 3,
} MM_CODEC_ENCODE, *PMM_CODEC_ENCODE;

//------------------------------------------------------------------------------

typedef enum _MM_ENC_PROFILE
{
   MM_ENC_PROFILE_UNKNOWN = 0,

   MM_ENC_PROFILE_BASELINE = 1,                 // Baseline profile
   MM_ENC_PROFILE_MAIN = 2,                     // Main profile
   MM_ENC_PROFILE_EXTENDED = 3,                 // Extended profile
   MM_ENC_PROFILE_HIGH = 4,                     // High profile

} MM_ENC_PROFILE, *PMM_ENC_PROFILE;

//------------------------------------------------------------------------------

typedef enum _MM_ENC_TARGET_USAGE
{
   MM_ENC_TARGET_USAGE_UNKNOWN = 0,

   MM_ENC_TARGET_USAGE_BEST_QUALITY = 1,
   MM_ENC_TARGET_USAGE_BALANCED = 2,
   MM_ENC_TARGET_USAGE_BEST_SPEED = 3,

} MM_ENC_TARGET_USAGE, *PMM_ENC_TARGET_USAGE;

//------------------------------------------------------------------------------

typedef enum _MM_ENC_RATE_METHOD
{
   MM_ENC_RATE_METHOD_UNKNOWN = 0,

   MM_ENC_RATE_METHOD_CBR = 1,
   MM_ENC_RATE_METHOD_VBR = 2,

} MM_ENC_RATE_METHOD, *PMM_ENC_RATE_METHOD;

//------------------------------------------------------------------------------

typedef struct _MM_RECT
{
   int Left;
   int Top;
   int Right;
   int Bottom;
} MM_RECT, *PMM_RECT;

//------------------------------------------------------------------------------

typedef struct _MM_ENC_AVC
{
   MM_ENC_PROFILE       Profile;                // user override MM_ENC_PROFILE
   MM_ENC_TARGET_USAGE  TargetUsage;            // user override MM_ENC_TARGET_USAGE
   MM_ENC_RATE_METHOD   RateMethod;             // user override variable or constant MM_ENC_RATE_METHOD
   unsigned             KeyFrameInterval;       // user override GOP interval, zero for auto
   unsigned             TargetKbps;             // target bit rate in KBs
}  MM_ENC_AVC, *PMM_ENC_AVC;

//------------------------------------------------------------------------------

typedef struct _MM_ENC_JPG
{
   unsigned             QFactor;
}  MM_ENC_JPG, *PMM_ENC_JPG;

//------------------------------------------------------------------------------

enum MM_DATA_CONTEXT
{
   MM_DATA_CONTEXT_UNKNOWN = 0x1,
   MM_DATA_CONTEXT_COMPRESSED_AUDIO = 0x10,
   MM_DATA_CONTEXT_COMPRESSED_VIDEO = 0x100,
   MM_DATA_CONTEXT_COMPRESSED_VIDEO_KEY_FRAME = 0x200,
   MM_DATA_CONTEXT_COMPRESSED_VIDEO_NON_VCL = 0x400,
   MM_DATA_CONTEXT_COMPRESSED_VIDEO_VCL = 0x800,
   MM_DATA_CONTEXT_UNCOMPRESSED_VIDEO = 0x1000,
   MM_DATA_CONTEXT_UNCOMPRESSED_VIDEO_CORRUPTION = 0x2000,
   MM_DATA_CONTEXT_UNCOMPRESSED_AUDIO = 0x10000,
   MM_DATA_CONTEXT_META = 0x100000,
};

//------------------------------------------------------------------------------

typedef struct _MM_VERSION                      // version control structure
{
   unsigned             Major;                  // incompatable external functionality
   unsigned             Minor;                  // additional internal functionality
   unsigned             Micro;                  // bug fixes
}  MM_VERSION, *PMM_VERSION;

//------------------------------------------------------------------------------

typedef struct _MM_LOAD                         // load settings structure
{
   unsigned             Size;                   // initialised to size of this structure
   unsigned             TimeoutMS;              // in - timout in MS before returning any network or command error
   char*                PLogPath;               // in - verbose log file creation directory if non-null

   MM_VERSION           Version;                // out - version of the interface
   MM_LOAD_CONTEXT      OutFlags;               // out - load context flags
   HINTERFACE           HInterface;             // out - handle to the loaded interface
}  MM_LOAD, *PMM_LOAD;

//------------------------------------------------------------------------------

typedef struct _MM_CLIENT_PLAY                  // additional play parameters
{
   unsigned             Size;                   // initialised to size of this structure
   char*                PStartTime;             // see documentation for further details
   char*                PEndTime;               // see documentation for further details
   MM_CLIENT_SPEED      Speed;                  // enumerated type of MM_CLIENT_SPEED when supported
   unsigned             BReverse;               // forward or reverse playout direction when supported
}  MM_CLIENT_PLAY, *PMM_CLIENT_PLAY;

//------------------------------------------------------------------------------

typedef struct _MM_SERVER_ENCODE                // additional encode parameters
{
   unsigned             Size;                   // initialised to size of this structure
   MM_CODEC_ENCODE      CODEC;                  // compression type
   unsigned             FrameRate;              // compression rate in seconds
   MM_ENC_JPG           JPGParams;              // Additional JPG parameters
   MM_ENC_AVC           AVCParams;              // Additional AVC parameters
}  MM_SERVER_ENCODE, *PMM_SERVER_ENCODE;

//------------------------------------------------------------------------------

typedef struct _MM_DATA                         // call back data returned to the client
{
   unsigned             Size;                   // initialised to size of this structure
   unsigned char*       PData[MM_DATA_PLANES];  // pointer to the data
   unsigned    		   Pitch[MM_DATA_PLANES];  // pitch of the data in pixels
   unsigned             Planes;                 // number of data planes 1-8;
   unsigned    		   DataLength;             // total size of data in bytes
   unsigned    		   Width;                  // width of the client data in pixels
   unsigned    		   Height;                 // height of the client data in pixels
   unsigned             ContextFlag;            // context MM_DATA_CONTEXT flag for the client data
   double      		   TimeStamp;              // tame stamp, when available, of the client data 
   unsigned             FourCC;                 // data format pointed to by PData
   void*                PReserved;              // reserved for internal use
}  MM_DATA, *PMM_DATA;

//------------------------------------------------------------------------------

typedef struct _MM_CLIENT_DEWARP
{
   unsigned             Size;                   // initialised to size of this structure
   unsigned             BSessionEnabled;
   unsigned             PanoFactor;
   unsigned             BHorViewMode;
   float                FOV;
   float                XAngle;
   float                YAngle;
   float                XAngleB;
   float                FOVRot;
   float                YCenter;
   float                XCenter;
   float                Radius;
   float                HStretch;
}  MM_CLIENT_DEWARP, *PMM_CLIENT_DEWARP;

//------------------------------------------------------------------------------

typedef struct _MM_CLIENT_SOURCE
{
   unsigned             Size;                   // initialised to size of this structure
   unsigned             Flags;                  // combination of MM_CLIENT_SOURCE_REQUEST flags
   unsigned             BRender;                // do not render decoded frame
   unsigned             BEnforceAR;             // apply letter boxing to enforce source aspect ratio
}  MM_CLIENT_SOURCE, *PMM_CLIENT_SOURCE;

//------------------------------------------------------------------------------

typedef struct _MM_SERVER_OVERLAY
{
   unsigned             Size;                   // initialised to size of this structure
   char*                POverlayText;           // valid pointer to overlay text
   unsigned long        TextColor;              // rgb colour code hash
   char*                PTextFont;              // font name, Arial used if unsupported or invalid
   char*                POverlayImage;          // not implemented, set to NULL
   MM_RECT              Position;               // bounding overlay points as percentages
}  MM_SERVER_OVERLAY, *PMM_SERVER_OVERLAY;

//------------------------------------------------------------------------------

typedef struct _MM_CLIENT_CHILD
{
   unsigned             Size;                   // initialised to size of this structure
   void*                HWnd;                   // window handle to display video
   unsigned             Flags;                  // combination of MM_CLIENT_CHILD context flags
   MM_CLIENT_DEWARP     Dewarp;
   MM_RECT              Zoom;
   MM_CLIENT_SOURCE     Source;
}  MM_CLIENT_CHILD, *PMM_CLIENT_CHILD;

//------------------------------------------------------------------------------
                                       
typedef mmStatus (MM_CBKAPI MM_DATA_CBKFN) (    // client defined data callback function
   HSESSION             hSession,               // handle to the client session
   MM_DATA*             pMediaSample,           // pointer to the multimedia data
   void*                pUserData);             // client supplied data
typedef MM_DATA_CBKFN *PMM_DATA_CBKFN;

//------------------------------------------------------------------------------
                                       
typedef void (MM_CBKAPI MM_STATUS_CBKFN) (      // client defined information callback function
   HSESSION             hSession,               // handle to the client session
   mmStatus             status,                 // status value
   char*                pMessage,               // additional status information
   void*                pUserData);             // client supplied data
typedef MM_STATUS_CBKFN *PMM_STATUS_CBKFN;

//------------------------------------------------------------------------------

typedef struct _MM_CLIENT_OPEN                  // additional open parameters
{
   unsigned          Size;                      // initialised to size of this structure
   MM_DATA_CBKFN*    PDataCBKFN;                // pointer to the user created data callback function
   void*             PDataUserData;             // user data that can be supplied in the data callback
   MM_STATUS_CBKFN*  PStatusCBKFN;              // pointer to the user created status callback function
   void*             PStatusUserData;           // user data that can be supplied in the status callback
   unsigned          OpenFlags;                 // combination of MM_CLIENT_REQUEST context flags
   char*             PURL;                      // file path or network address
   void*             HWnd;                      // window handle to display video
   unsigned          BRepeat;                   // repeat media if EOF is returned
   unsigned          BNetTCP;                   // stream over tcp or udp for networked streams
   unsigned          CacheVideo;                // millisecond video cache of decoded frames for smooth playblayback and late network packets                 
   unsigned          CacheAudio;                // millisecond audio cache of decoded frames for smooth playblayback and late network packets
   void*             PReserved;                 // reserved for internal use
}  MM_CLIENT_OPEN, *PMM_CLIENT_OPEN;

typedef struct _MM_SERVER_RTSP                  // additional open parameters
{
   char*             PURL;                      // User supplied stream identifier
   unsigned          RTSPPortNumber;            // RTSP Server port number or zero for auto
   unsigned          BReuseSource;              // Allow multiple connections to use the same source
   char*             PUserName;                 // Digest authentication RFC2617 or null for none
   char*             PPassword;                 // Digest authentication RFC2617 (base64 encoding)
}  MM_SERVER_RTSP, *PMM_SERVER_RTSP;

typedef struct _MM_SERVER_OPEN                  // additional open parameters
{
   unsigned          Size;                      // initialised to size of this structure
   MM_STATUS_CBKFN*  PStatusCBKFN;              // pointer to the user created status callback function
   void*             PStatusUserData;           // user data that can be supplied in the status callback
   unsigned          OpenFlags;                 // combination of MM_SERVER_REQUEST context flags
   void*             HWnd;                      // window handle to screen scrape see, MM_SERVER_REQUEST
   MM_RECT           Coordinates;               // desktop coordinates to screen scrape, see MM_SERVER_REQUEST
   void*             HMonitor;                  // screen to screen scrape, see MM_SERVER_REQUEST
   MM_SERVER_ENCODE  EncodeParams;              // encode parameters for screen scrape
   MM_SERVER_RTSP    RTSPParams;                // additional RTSP parameters
}  MM_SERVER_OPEN, *PMM_SERVER_OPEN;

//------------------------------------------------------------------------------

#include <poppack.h>