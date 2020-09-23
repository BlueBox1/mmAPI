using System;
using System.Runtime.InteropServices;

namespace MultiMedia
{
   //------------------------------------------------------------------------------

   static class Constants
   {
      public const uint MM_DATA_PLANES = 8;        // maximum number of video data planes
   }

   //------------------------------------------------------------------------------

   public enum MM_LOAD_CONTEXT                     // mmLoad returned context flags
   {
      MM_LOAD_CONTEXT_ACTIVATED = 0x1,             // licence is activated
      MM_LOAD_CONTEXT_LOGGING = 0x2,               // logging to file is specified
      MM_LOAD_CONTEXT_DXSUPPORTED = 0x4,           // directX acceleration is supported 
   };

   //------------------------------------------------------------------------------

   public enum MM_CLIENT_REQUEST                   // mmClientOpen data request context flags
   {
      MM_CLIENT_REQUEST_VIDEO_TO_DISPLAY = 0x1,    // display video in provided hWnd
      MM_CLIENT_REQUEST_AUDIO_TO_MIXER = 0x2,      // play audio to local mixer
      MM_CLIENT_REQUEST_VIDEO_SOURCE = 0x100,      // source video data to provided call back
      MM_CLIENT_REQUEST_AUDIO_SOURCE = 0x1000,     // source audio data to provided call back
      MM_CLIENT_REQUEST_VIDEO_DECODED = 0x200,     // source decoded video data to provided call back
      MM_CLIENT_REQUEST_AUDIO_DECODED = 0x2000,    // source decoded audio data to provided call back
   }

   //------------------------------------------------------------------------------

   public enum MM_SERVER_REQUEST                   // mmServerOpen data request context flags
   {
      MM_SERVER_REQUEST_VIDEO_TO_RTSP = 0x1,       // transport video over RTSP
      MM_SERVER_REQUEST_VIDEO_OF_HWND = 0x2000,    // encode a window as video
      MM_SERVER_REQUEST_VIDEO_OF_COORDS = 0x4000,  // encode desktop coordinates as video
      MM_SERVER_REQUEST_VIDEO_OF_MONITOR = 0x8000, // encode a screen as video
   }

   //------------------------------------------------------------------------------

   public enum MM_CLIENT_CHILD_REQUEST
   {
      MM_CLIENT_CHILD_OPEN = 0x1,
      MM_CLIENT_CHILD_CLOSE = 0x2,
      MM_CLIENT_CHILD_RESET = 0x4,
      MM_CLIENT_CHILD_DEWARP = 0x8,
      MM_CLIENT_CHILD_ZOOM = 0x10,
      MM_CLIENT_CHILD_SOURCE = 0x20,
   }

   //------------------------------------------------------------------------------

   public enum MM_CLIENT_SOURCE_REQUEST
   {
      MM_CLIENT_SOURCE_RENDER = 0x1,
      MM_CLIENT_SOURCE_ASPECT_RATIO = 0x2,
   }

   //------------------------------------------------------------------------------

   public enum MM_CLIENT_SPEED                     // mmClientPlay supported playout speeds
   {
      MM_CLIENT_SPEED_UNKNOWN = 0,
      MM_CLIENT_SPEED_QUARTER = 1,
      MM_CLIENT_SPEED_HALF = 2,
      MM_CLIENT_SPEED_ONE = 3,
      MM_CLIENT_SPEED_TWO = 4,
      MM_CLIENT_SPEED_FOUR = 5,
      MM_CLIENT_SPEED_EIGHT = 6,
      MM_CLIENT_SPEED_SIXTEEN = 7,
      MM_CLIENT_SPEED_THIRTYTWO = 8,
   }

   //------------------------------------------------------------------------------

   public enum MM_CODEC_ENCODE
   {
      MM_CODEC_UNKNOWN = 0,
      MM_CODEC_AVC = 1,
      MM_CODEC_JPG = 2,
      MM_CODEC_HEVC = 3,
   }

   //------------------------------------------------------------------------------
   public enum MM_ENC_PROFILE
   {
      MM_ENC_PROFILE_UNKNOWN = 0,
      MM_ENC_PROFILE_BASELINE = 1,      // Baseline profile
      MM_ENC_PROFILE_MAIN = 2,          // Main profile
      MM_ENC_PROFILE_EXTENDED = 3,      // Extended profile
      MM_ENC_PROFILE_HIGH = 4,          // High profile
   }

   //------------------------------------------------------------------------------

   public enum MM_ENC_TARGET_USAGE
   {
      MM_ENC_TARGET_USAGE_UNKNOWN = 0,
      MM_ENC_TARGET_USAGE_BEST_QUALITY = 1,
      MM_ENC_TARGET_USAGE_BALANCED = 2,
      MM_ENC_TARGET_USAGE_BEST_SPEED = 3,
   }

   //------------------------------------------------------------------------------

   public enum MM_ENC_RATE_METHOD
   {
      MM_ENC_RATE_METHOD_UNKNOWN = 0,
      MM_ENC_RATE_METHOD_CBR = 1,
      MM_ENC_RATE_METHOD_VBR = 2,
   }

   //------------------------------------------------------------------------------

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_ENC_AVC
   {
      public MM_ENC_PROFILE Profile;            // user override MM_ENC_PROFILE
      public MM_ENC_TARGET_USAGE TargetUsage;   // user override MM_ENC_TARGET_USAGE
      public MM_ENC_RATE_METHOD RateMethod;     // user override variable or constant MM_ENC_RATE_METHOD
      public uint KeyFrameInterval;             // user override GOP interval, zero for auto
      public uint TargetKbps;                   // target bit rate in KBs
   }

   //------------------------------------------------------------------------------

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_ENC_JPG
   {
      public uint QFactor;
   }

   //------------------------------------------------------------------------------

   public enum MM_DATA_CONTEXT
   {
      MM_DATA_CONTEXT_UNKNOWN = 0x1,
      MM_DATA_CONTEXT_COMPRESSED_AUDIO = 0x10,
      MM_DATA_CONTEXT_COMPRESSED_VIDEO = 0x100,
      MM_DATA_CONTEXT_COMPRESSED_VIDEO_KEY_FRAME = 0x200,
      MM_DATA_CONTEXT_COMPRESSED_VIDEO_NON_VCL = 0x400,
      MM_DATA_CONTEXT_UNCOMPRESSED_VIDEO = 0x1000,
      MM_DATA_CONTEXT_UNCOMPRESSED_VIDEO_CORRUPTION = 0x2000,
      MM_DATA_CONTEXT_UNCOMPRESSED_AUDIO = 0x10000,
      MM_DATA_CONTEXT_META = 0x100000,
   }

   //------------------------------------------------------------------------------

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_VERSION                        // version control structure
   {
      public uint Major;                           // incompatable external functionality
      public uint Minor;                           // additional public functionality
      public uint Micro;                           // patch build
   }

   //------------------------------------------------------------------------------
   
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_LOAD                     // load settings structure
   {
      public uint Size;                      // initialised to size of this structure
      public uint TimeoutMS;                 // in - timout in MS before returning any network or command error
      public IntPtr PLogPath;                // in - verbose log file creation directory if non-null

      public MM_VERSION Version;             // out - version of the interface
      public MM_LOAD_CONTEXT OutFlags;       // out - load context flags
      public IntPtr HInterface;              // out - handle to the loaded interface
   }

   //------------------------------------------------------------------------------

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_CLIENT_PLAY             // additional play parameters
   {
      public uint Size;                     // initialised to size of this structure
      public IntPtr PStartTime;             // see documentation for further details
      public IntPtr PEndTime;               // see documentation for further details
      public MM_CLIENT_SPEED Speed;         // enumerated type of MM_CLIENT_SPEED when supported
      public uint BReverse;                 // forward or reverse playout direction when supported
   }

   //------------------------------------------------------------------------------

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_SERVER_ENCODE
   {
      public uint Size;                    // initialised to size of this structure
      public MM_CODEC_ENCODE CODEC;        // compression type
      public uint FrameRate;               // compression rate in seconds
      public MM_ENC_JPG JPGParams;         // additional JPG parameters
      public MM_ENC_AVC AVCParams;         // additional AVC parameters
   }

   //------------------------------------------------------------------------------
    
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_DATA                   // call back data returned to the client
   {
      public uint       Size;              // initialised to size of this structure
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      public IntPtr[]   PData;             // pointer to the data
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      public uint[]     Pitch;             // pitch of the data in pixels
      public uint       Planes;            // number of data planes 1-8;
      public uint       DataLength;        // total size of data in bytes
      public uint       Width;             // width of the client data in pixels
      public uint       Height;            // height of the client data in pixels
      public uint       ContextFlag;       // context MM_DATA_CONTEXT flag for the client data
      public double     TimeStamp;         // tame stamp, when available, of the client data 
      public uint       FourCC;            // data format pointed to by PData
      public IntPtr     PReserved;         // reserved for internal use
   }

   //------------------------------------------------------------------------------

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_CLIENT_DEWARP
   {
      public uint  Size;                  // initialised to size of this structure
      public uint  BSessionEnabled;
      public uint  PanoFactor;
      public uint  BHorViewMode;
      public float FOV;
      public float XAngle;
      public float YAngle;
      public float XAngleB;
      public float FOVRot;
      public float YCenter;
      public float XCenter;
      public float Radius;
      public float HStretch;
   };

   //------------------------------------------------------------------------------
   
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_CLIENT_SOURCE
   {
      public uint Size;                      // initialised to size of this structure
      public MM_CLIENT_SOURCE_REQUEST Flags; // combination of MM_CLIENT_SOURCE_REQUEST flags
      public uint BRender;                   // do not render decoded frame
      public uint BEnforceAR;                // apply letter boxing to enforce source aspect ratio
   };

   //------------------------------------------------------------------------------

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_SERVER_OVERLAY
   {
      public uint    Size;                   // initialised to size of this structure
      public IntPtr  POverlayText;           // valid pointer to overlay text
      public uint    TextColor;              // rgb colour code hash
      public IntPtr  PTextFont;              // font name, Arial used if unsupported or invalid
      public IntPtr  POverlayImage;          // not implemented, set to NULL
      public MM_RECT Position;               // bounding overlay points as percentages
   };
      
   //------------------------------------------------------------------------------

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_CLIENT_CHILD
   {
      public uint                      Size;    // initialised to size of this structure
      public IntPtr                    HWnd;    // window handle to display video
      public MM_CLIENT_CHILD_REQUEST   Flags;   // combination of MM_CLIENT_CHILD context flags
      public MM_CLIENT_DEWARP          Dewarp;
      public MM_RECT                   Zoom;
      public MM_CLIENT_SOURCE          Source;
   };

   //------------------------------------------------------------------------------

   public delegate mmStatus MM_DATA_CBKFN(IntPtr hSession, IntPtr pMediaSample, IntPtr pUserData);

   //------------------------------------------------------------------------------

   public delegate void MM_STATUS_CBKFN(IntPtr hSession, uint status, IntPtr pMessage, IntPtr pUserData);

   //------------------------------------------------------------------------------

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_CLIENT_OPEN              // additional open parameters
   {
      public uint Size;                      // initialised to size of this structure
      public MM_DATA_CBKFN PDataCBKFN;       // pointer to the user created data callback function
      public IntPtr PDataUserData;           // user data that can be supplied in the data callback
      public MM_STATUS_CBKFN PStatusCBKFN;   // pointer to the user created status callback function
      public IntPtr PStatusUserData;         // user data that can be supplied in the status callback
      public MM_CLIENT_REQUEST OpenFlags;    // combination of MM_CLIENT_REQUEST context flags
      public IntPtr PURL;                    // file path or network address
      public IntPtr HWnd;                    // window handle to display video
      public uint BRepeat;                   // repeat media if EOF is returned
      public uint BNetTCP;                   // stream over tcp or udp for networked streams
      public uint CacheVideo;                // millisecond video cache of decoded frames for smooth playblayback and late network packets                 
      public uint CacheAudio;                // millisecond audio cache of decoded frames for smooth playblayback and late network packets
      public IntPtr PReserved;               // reserved for internal use
   }

   //------------------------------------------------------------------------------

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_RECT                     // additional play parameters, Note, not a 'long' as this is 64 bits wide in C#
   {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;
   }

   //------------------------------------------------------------------------------

   public struct MM_SERVER_RTSP                 // additional open parameters
   {
      public IntPtr PURL;                       // user supplied stream identifier
      public uint   RTSPPortNumber;             // RTSP Server port number or zero for auto
      public uint   BReuseSource;               // allow multiple connections to use the same source 
      public IntPtr PUserName;                  // digest authentication RFC2617 or null for none
      public IntPtr PPassword;                  // digest authentication RFC2617 (base64 encoding)
   }

   //------------------------------------------------------------------------------

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_SERVER_OPEN                 // additional open parameters
   {
      public uint Size;                         // initialised to size of this structure
      public MM_STATUS_CBKFN PStatusCBKFN;      // pointer to the user created status callback function
      public IntPtr PStatusUserData;            // user data that can be supplied in the status callback
      public MM_SERVER_REQUEST OpenFlags;       // combination of MM_SERVER_REQUEST_ context flags
      public IntPtr HWnd;                       // window handle to screen scrape
      public MM_RECT Coordinates;               // if HWnd is NULL, desktop coordinates to screen scrape
      public IntPtr HMonitor;                   // screen to screen scrape, see MM_SERVER_REQUEST
      public MM_SERVER_ENCODE EncodeParams;     // encode parameters for screen scrape
      public MM_SERVER_RTSP RTSPParams;         // additional RTSP parameters
   }
      
   //------------------------------------------------------------------------------

   internal static class NativeMethods
   {
      [DllImport("multimedia.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal static extern mmStatus mmLoad(ref MM_LOAD pLoad);

      [DllImport("multimedia.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal static extern mmStatus mmRelease(IntPtr hInterface);

      [DllImport("multimedia.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal static extern mmStatus mmClientOpen(out IntPtr pSession, ref MM_CLIENT_OPEN pOpenParms);
       
      [DllImport("multimedia.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal static extern mmStatus mmClientPause(IntPtr hSession);

      [DllImport("multimedia.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal static extern mmStatus mmClientPlay(IntPtr hSession, ref MM_CLIENT_PLAY pPlayParms);

      [DllImport("multimedia.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal static extern mmStatus mmServerOpen(out IntPtr pSession, ref MM_SERVER_OPEN pOpenParms);

      [DllImport("multimedia.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal static extern mmStatus mmClose(IntPtr hSession);

      [DllImport("multimedia.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal static extern mmStatus mmDictionarySet(IntPtr hSession, IntPtr pKey, IntPtr pValue);

      [DllImport("multimedia.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal static extern mmStatus mmDictionaryGet(IntPtr hSession, IntPtr pKey, IntPtr pValue);
   }
    
    //------------------------------------------------------------------------------

   public class mmMethods
   {
      public static mmStatus mmLoad(ref MM_LOAD pLoad)
      {
         return NativeMethods.mmLoad(ref pLoad);
      }
      public static mmStatus mmRelease(IntPtr hInterface)
      {
         return NativeMethods.mmRelease(hInterface);
      }
      public static mmStatus mmClientOpen(out IntPtr pSession, ref MM_CLIENT_OPEN pOpenParms)
      {
         return NativeMethods.mmClientOpen(out pSession, ref pOpenParms);
      }
      public static mmStatus mmClientPause(IntPtr hSession)
      {
         return NativeMethods.mmClientPause(hSession);
      }
      public static mmStatus mmClientPlay(IntPtr hSession, ref MM_CLIENT_PLAY pPlayParms)
      {
         return NativeMethods.mmClientPlay(hSession, ref pPlayParms);
      }
      public static mmStatus mmServerOpen(out IntPtr pSession, ref MM_SERVER_OPEN pOpenParms)
      {
         return NativeMethods.mmServerOpen(out pSession, ref pOpenParms);
      }
      public static mmStatus mmClose(IntPtr hSession)
      {
         return NativeMethods.mmClose(hSession);
      }
      public static mmStatus mmDictionarySet(IntPtr hSession, mmSessionDictionaryKeys key, string value)
      {
         IntPtr pKey = Marshal.StringToHGlobalAnsi(key.ToString());
         IntPtr pValue = Marshal.StringToHGlobalAnsi(value.ToString());
         mmStatus sts = NativeMethods.mmDictionarySet(hSession, pKey, pValue);
         Marshal.FreeHGlobal(pKey);
         Marshal.FreeHGlobal(pValue);
         return sts;
      }
      public static mmStatus mmDictionarySet(IntPtr hSession, mmSessionDictionaryKeys key, IntPtr pValue)
      {
         IntPtr pKey = Marshal.StringToHGlobalAnsi(key.ToString());
         mmStatus sts = NativeMethods.mmDictionarySet(hSession, pKey, pValue);
         Marshal.FreeHGlobal(pKey);
         return sts;
      }
      public static mmStatus mmDictionaryGet(IntPtr hSession, mmSessionDictionaryKeys key, string value)
      {
         IntPtr pKey = Marshal.StringToHGlobalAnsi(key.ToString());
         IntPtr pValue = Marshal.StringToHGlobalAnsi(value.ToString());
         mmStatus sts = NativeMethods.mmDictionaryGet(hSession, pKey, pValue);
         Marshal.FreeHGlobal(pKey);
         Marshal.FreeHGlobal(pValue);
         return sts;
      }
   }

   public enum mmStatusBase
   {
      MM_STS_SRC_ERROR_BASE   = 0x11000000,
      MM_STS_SRC_WARNING_BASE = 0x12000000,
      MM_STS_SRC_INFO_BASE    = 0x14000000,

      MM_STS_LIB_ERROR_BASE   = 0x21000000,
      MM_STS_LIB_WARNING_BASE = 0x22000000,
      MM_STS_LIB_INFO_BASE    = 0x24000000,
   }
   public enum mmStatus
   {
      MM_STS_NONE                                  = 0,

      MM_STS_SRC_ERROR_UNKNOWN                     = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0001,
      MM_STS_SRC_ERROR_UNINITIALISED               = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0002,
      MM_STS_SRC_ERROR_INVALID_HWND                = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0003,
      MM_STS_SRC_ERROR_SOURCE_NOT_FOUND            = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0004,
      MM_STS_SRC_ERROR_FAILED_SUB_SESSION_INIT     = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0005,
      MM_STS_SRC_ERROR_NO_SUB_SESSIONS             = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0006,
      MM_STS_SRC_ERROR_INVALID_STATE               = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0007,
      MM_STS_SRC_ERROR_UNDEFINED_BEHAVIOR          = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0008,
      MM_STS_SRC_ERROR_INCOMPATIBLE_API            = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0009,
      MM_STS_SRC_ERROR_INVALID_FLAGS               = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x000a,
      MM_STS_SRC_ERROR_INVALID_HANDLE              = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x000b,
      MM_STS_SRC_ERROR_INVALID_POINTER             = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x000c,
      MM_STS_SRC_ERROR_INVALID_THREAD_CALL         = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x000d,
      MM_STS_SRC_ERROR_UNKNOWN_SERVER_ERROR        = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x000e,
      MM_STS_SRC_ERROR_INVALID_CODEC               = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x000f,
      MM_STS_SRC_ERROR_UNSUPPORTED                 = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0010,
      MM_STS_SRC_ERROR_LICENCE                     = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0011,
      MM_STS_SRC_ERROR_URL_FORMAT                  = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0012,
      MM_STS_SRC_ERROR_INVALID_DATA                = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0013,
      MM_STS_SRC_ERROR_PROTOCOL_NOT_FOUND          = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0014,
      MM_STS_SRC_ERROR_VIDEO_NOT_FOUND             = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0015,
      MM_STS_SRC_ERROR_PARSE_HEADER                = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0016,
      MM_STS_SRC_ERROR_PARSING                     = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0017,
      MM_STS_SRC_ERROR_INVALID_PARAMETER           = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0018,
      MM_STS_SRC_ERROR_ADAPTER_NOT_FOUND           = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0019,
      MM_STS_SRC_ERROR_INVALID_TIME_FORMAT         = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0020,
      MM_STS_SRC_ERROR_UNABLE_TO_HOOK_WINDOW       = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0021,
      MM_STS_SRC_ERROR_NETWORK_TIMEOUT             = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0022,
      MM_STS_SRC_ERROR_NETWORK_UNAUTHORIZED        = mmStatusBase.MM_STS_SRC_ERROR_BASE + 0x0023,

      MM_STS_SRC_WARNING_UNKNOWN                   = mmStatusBase.MM_STS_SRC_WARNING_BASE + 0x0001,
      MM_STS_SRC_WARNING_SOURCE_BUFFER_TOO_SMALL   = mmStatusBase.MM_STS_SRC_WARNING_BASE + 0x0002,
      MM_STS_SRC_WARNING_INVALID_CODEC_RECEIVED    = mmStatusBase.MM_STS_SRC_WARNING_BASE + 0x0003,

      MM_STS_SRC_INFO_UNKNOWN                      = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0001,
      MM_STS_SRC_INFO_RTCP_BYE_RECEIVED            = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0002,
      MM_STS_SRC_INFO_RTSP_CLOSING_STREAM          = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0003,
      MM_STS_SRC_INFO_RTSP_SDP_RECEIVED            = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0004,
      MM_STS_SRC_INFO_RTSP_PORT_CONNECTED          = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0005,
      MM_STS_SRC_INFO_RTSP_PORT_INITIATED          = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0006,
      MM_STS_SRC_INFO_RTP_SOCKET_SIZES             = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0007,
      MM_STS_SRC_INFO_SOURCE_MEDIUM_UNKNOWN        = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0008,
      MM_STS_SRC_INFO_SOURCE_RTSP_SERVER_URL       = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0009,
      MM_STS_SRC_INFO_SOURCE_URL_OPEN              = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x000a,
      MM_STS_SRC_INFO_SOURCE_URL_VIDEO             = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x000b,
      MM_STS_SRC_INFO_SOURCE_EOF                   = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x000c,
      MM_STS_SRC_INFO_SOURCE_URL_CLOSE             = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x000d,
      MM_STS_SRC_INFO_SOURCE_URL_PAUSE             = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x000e,
      MM_STS_SRC_INFO_SOURCE_URL_PLAY              = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x000f,
      MM_STS_SRC_INFO_FFMPEG                       = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0010,
      MM_STS_SRC_INFO_NO_NETWORK_FRAME_TIMEOUT     = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0011,
      MM_STS_SRC_INFO_RTSP_START_STREAMING         = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0012,
      MM_STS_SRC_INFO_RTSP_PAUSE_STREAMING         = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0013,
      MM_STS_SRC_INFO_RTSP_SEEK_STREAMING          = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0014,
      MM_STS_SRC_INFO_RTSP_STOP_STREAMING          = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0015,
      MM_STS_SRC_INFO_SOURCE_URL_VIDEO_EX1         = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0016,
      MM_STS_SRC_INFO_SOURCE_RTSP_VIDEO            = mmStatusBase.MM_STS_SRC_INFO_BASE + 0x0017,

      MM_STS_LIB_ERROR_UNKNOWN                     = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x0001,
      MM_STS_LIB_ERROR_UNINITIALISED               = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x0002,
      MM_STS_LIB_ERROR_NOT_ENOUGH_MEMORY           = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x0003,
      MM_STS_LIB_ERROR_BUFFER_NOT_VALID            = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x0004,
      MM_STS_LIB_ERROR_INVALID_STATE               = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x0005,
      MM_STS_LIB_ERROR_INVALID_INDEX               = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x0006,
      MM_STS_LIB_ERROR_INVALID_HWND                = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x0007,
      MM_STS_LIB_ERROR_INVALID_FILEPATH            = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x0008,
      MM_STS_LIB_ERROR_INVALID_MODE                = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x0009,
      MM_STS_LIB_ERROR_INVALID_POINTER             = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x000a,
      MM_STS_LIB_ERROR_SCREEN_SCRAPE               = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x000b,
      MM_STS_LIB_ERROR_DECODING_HEADER             = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x000c,
      MM_STS_LIB_ERROR_INCOMPATIBLE_API            = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x000d,
      MM_STS_LIB_ERROR_ALREADY_INITIALISED         = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x000e,
      MM_STS_LIB_ERROR_UNDEFINED_BEHAVIOR          = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x000f,
      MM_STS_LIB_ERROR_UNSUPPORTED_CODEC           = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x0010,
      MM_STS_LIB_ERROR_HARDWARE_ABORTED            = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x0011,
      MM_STS_LIB_ERROR_VIDEO_PARMS_CHANGED         = mmStatusBase.MM_STS_LIB_ERROR_BASE + 0x0012,

      MM_STS_LIB_WARNING_UNKNOWN                   = mmStatusBase.MM_STS_LIB_WARNING_BASE + 0x0001,
      MM_STS_LIB_WARNING_INPUT_DATA_DROPPED        = mmStatusBase.MM_STS_LIB_WARNING_BASE + 0x0002,
      MM_STS_LIB_WARNING_NO_AVAILABLE_SURFACE      = mmStatusBase.MM_STS_LIB_WARNING_BASE + 0x0003,
      MM_STS_LIB_WARNING_RESTARTING_HARDWARE       = mmStatusBase.MM_STS_LIB_WARNING_BASE + 0x0004,

      MM_STS_LIB_INFO_UNKNOWN                      = mmStatusBase.MM_STS_LIB_INFO_BASE + 0x0001,
      MM_STS_LIB_INFO_MORE_INPUT_DATA              = mmStatusBase.MM_STS_LIB_INFO_BASE + 0x0002,
   }

   public enum mmSessionDictionaryKeys
   {
      NO_OP = 0,

      /// <summary>
      /// key "svr_force_idr_frame"  value "on" | "off"
      /// Sessions opened with mmServerOpen can override the original requested 
      /// KeyFrameInterval(GOP) value by forcing all encoded frames to be of IDR 
      /// type.When this feature is turned off, the originally specified 
      /// KeyFrameInterval is again used.
      /// 
      /// Example:
      ///     mmDictionarySe(hSession,  "svr_force_idr_frame", "on");
      ///     Sleep(1000)
      ///     mmDictionarySe(hSession,  "svr_force_idr_frame", "off");
      /// </summary>
      SVR_FORCE_IDR_FRAME = 1,

      /// <summary>
      /// key "svr_repeat_vca_frame"  value "n frames"
      /// Repeat the current (VCA) frame and any associated non-VCA
      /// NALs on the encoders output stream. 
      /// Do not encode any new frames.
      /// When used, the VCA frame will be delivered as fast as possible by the system
      /// NOT at the user defined frame rate.
      /// 
      /// Example:
      ///     mmDictionarySet(hSession,  "svr_repeat_vca_frame", "50");
      /// </summary>        
      SVR_REPEAT_VCA_FRAME = 2,

      /// <summary>
      /// key "cli_dewarp"  value MM_CLIENT_DEWARP*
      /// Set decode session fisheye dewarping paramers
      /// Use pointer to a MM_CLIENT_DEWARP structure
      /// IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(Parms))
      /// Marshal.StructureToPtr(Parms, pnt, false)
      /// Marshal.FreeHGlobal(pnt)
      /// 
      /// Example:
      ///     mmDictionarySet(hSession,  "cli_dewarp", pnt);
      /// </summary>        
      CLI_DEWARP = 3,

      /// <summary>
      /// key "svr_overlay"  value MM_OVERLAY*
      /// Set session overlay
      /// Use pointer to a MM_OVERLAY structure
      /// IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(Parms))
      /// Marshal.StructureToPtr(Parms, pnt, false)
      /// Marshal.FreeHGlobal(pnt)
      /// 
      /// Example:
      ///     mmDictionarySet(hSession,  "overlay", pnt);
      /// </summary>
      SVR_OVERLAY = 4,

      /// <summary>
      /// key "svr_update_hwnd"  value pnt
      /// Update the currently encoded window handle
      /// Valid only when MM_SERVER_REQUEST_VIDEO_OF_HWND is used
      /// Use pointer to a uint data type
      /// IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(IntPtr))
      /// IntPtr hWnd = handle
      /// Marshal.StructureToPtr(hWnd, pnt, false)
      /// Marshal.FreeHGlobal(pnt)
      /// Example:
      ///     mmDictionarySet(hSession,  "svr_update_hwnd", pnt);
      /// </summary>
      SVR_UPDATE_HWND = 5,

      /// <summary>
      /// key "dec_zoom"  value MM_RECT*
      /// Set zoom window position as percentage values
      /// Use pointer to a MM_RECT structure
      /// IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(Parms))
      /// Marshal.StructureToPtr(Parms, pnt, false)
      /// Marshal.FreeHGlobal(pnt)
      /// 
      /// Example:
      ///     mmDictionarySet(hSession,  "cli_zoom", pnt);
      /// </summary>        
      CLI_ZOOM = 6,

      /// <summary>
      /// Internal use only
      CLI_RESET = 7,    
 
      /// <summary>
      /// key "cli_source"  value MM_CLIENT_SOURCE*
      /// Use pointer to a MM_CLIENT_SOURCE structure
      /// IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(Parms))
      /// Marshal.StructureToPtr(Parms, pnt, false)
      /// Marshal.FreeHGlobal(pnt)
      /// 
      /// Example:
      ///     mmDictionarySet(hSession,  "cli_source", pnt);
      /// </summary>        
      CLI_SOURCE = 8,

      /// <summary>
      /// key "cli_child"  value MM_CLIENT_CHILD*
      /// Use pointer to a MM_CLIENT_CHILD structure
      /// IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(Parms))
      /// Marshal.StructureToPtr(Parms, pnt, false)
      /// Marshal.FreeHGlobal(pnt)
      /// 
      /// Example:
      ///     mmDictionarySet(hSession,  "cli_child", pnt);
      /// </summary>        
      CLI_CHILD = 9,
   }
}