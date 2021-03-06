
//------------------------------------------------------------------------------
// mmStatus.h - Status codes for the multimedia API
//------------------------------------------------------------------------------

#define mmStatus unsigned
#define MM_STS_NONE                                0

// ms winsock errors can be found at bit 0x100000, for example 0x11100000 |= error

#define MM_STS_SRC_ERROR_BASE                      0x11000000
#define MM_STS_SRC_WARNING_BASE                    0x12000000
#define MM_STS_SRC_INFO_BASE                       0x14000000

#define MM_STS_LIB_ERROR_BASE                      0x21000000
#define MM_STS_LIB_WARNING_BASE                    0x22000000
#define MM_STS_LIB_INFO_BASE                       0x24000000

#define MM_STS_HWM_ERROR_BASE                      0x31000000
#define MM_STS_HWM_WARNING_BASE                    0x32000000
#define MM_STS_HWM_INFO_BASE                       0x34000000

// third party error base address
#define MM_STS_LIB_INTEL_BASE                          0x1000
#define MM_STS_LIB_AMD_BASE                            0x2000
#define MM_STS_LIB_NVIDIA_DEC_BASE                     0x3000
#define MM_STS_LIB_VCM_BASE                            0x4000
#define MM_STS_LIB_NVIDIA_ENC_BASE                     0x5000

//MM_STS_SRC_
#define MM_STS_SRC_ERROR_UNKNOWN				         ( MM_STS_SRC_ERROR_BASE + 0x0001 )
#define MM_STS_SRC_ERROR_UNINITIALISED             ( MM_STS_SRC_ERROR_BASE + 0x0002 )
#define MM_STS_SRC_ERROR_INVALID_HWND              ( MM_STS_SRC_ERROR_BASE + 0x0003 )
#define MM_STS_SRC_ERROR_SOURCE_NOT_FOUND          ( MM_STS_SRC_ERROR_BASE + 0x0004 )
#define MM_STS_SRC_ERROR_FAILED_SUB_SESSION_INIT   ( MM_STS_SRC_ERROR_BASE + 0x0005 )
#define MM_STS_SRC_ERROR_NO_SUB_SESSIONS           ( MM_STS_SRC_ERROR_BASE + 0x0006 )
#define MM_STS_SRC_ERROR_INVALID_STATE             ( MM_STS_SRC_ERROR_BASE + 0x0007 )
#define MM_STS_SRC_ERROR_UNDEFINED_BEHAVIOR        ( MM_STS_SRC_ERROR_BASE + 0x0008 )
#define MM_STS_SRC_ERROR_INCOMPATIBLE_API          ( MM_STS_SRC_ERROR_BASE + 0x0009 )
#define MM_STS_SRC_ERROR_INVALID_FLAGS             ( MM_STS_SRC_ERROR_BASE + 0x000a )
#define MM_STS_SRC_ERROR_INVALID_HANDLE            ( MM_STS_SRC_ERROR_BASE + 0x000b )
#define MM_STS_SRC_ERROR_INVALID_POINTER           ( MM_STS_SRC_ERROR_BASE + 0x000c )
#define MM_STS_SRC_ERROR_INVALID_THREAD_CALL       ( MM_STS_SRC_ERROR_BASE + 0x000d )
#define MM_STS_SRC_ERROR_UNKNOWN_SERVER_ERROR      ( MM_STS_SRC_ERROR_BASE + 0x000e )
#define MM_STS_SRC_ERROR_INVALID_CODEC             ( MM_STS_SRC_ERROR_BASE + 0x000f )
#define MM_STS_SRC_ERROR_UNSUPPORTED               ( MM_STS_SRC_ERROR_BASE + 0x0010 )
#define MM_STS_SRC_ERROR_LICENCE                   ( MM_STS_SRC_ERROR_BASE + 0x0011 )
#define MM_STS_SRC_ERROR_URL_FORMAT                ( MM_STS_SRC_ERROR_BASE + 0x0012 )
#define MM_STS_SRC_ERROR_INVALID_DATA              ( MM_STS_SRC_ERROR_BASE + 0x0013 )
#define MM_STS_SRC_ERROR_PROTOCOL_NOT_FOUND        ( MM_STS_SRC_ERROR_BASE + 0x0014 )
#define MM_STS_SRC_ERROR_VIDEO_NOT_FOUND           ( MM_STS_SRC_ERROR_BASE + 0x0015 )
#define MM_STS_SRC_ERROR_PARSE_HEADER              ( MM_STS_SRC_ERROR_BASE + 0x0016 )
#define MM_STS_SRC_ERROR_PARSING                   ( MM_STS_SRC_ERROR_BASE + 0x0017 )
#define MM_STS_SRC_ERROR_INVALID_PARAMETER         ( MM_STS_SRC_ERROR_BASE + 0x0018 )
#define MM_STS_SRC_ERROR_ADAPTER_NOT_FOUND         ( MM_STS_SRC_ERROR_BASE + 0x0019 )
#define MM_STS_SRC_ERROR_INVALID_TIME_FORMAT       ( MM_STS_SRC_ERROR_BASE + 0x0020 )
#define MM_STS_SRC_ERROR_UNABLE_TO_HOOK_WINDOW     ( MM_STS_SRC_ERROR_BASE + 0x0021 )
#define MM_STS_SRC_ERROR_NETWORK_TIMEOUT           ( MM_STS_SRC_ERROR_BASE + 0x0022 )
#define MM_STS_SRC_ERROR_NETWORK_UNAUTHORIZED      ( MM_STS_SRC_ERROR_BASE + 0x0023 )

#define MM_STS_SRC_WARNING_UNKNOWN                 ( MM_STS_SRC_WARNING_BASE + 0x0001 )
#define MM_STS_SRC_WARNING_SOURCE_BUFFER_TOO_SMALL ( MM_STS_SRC_WARNING_BASE + 0x0002 )
#define MM_STS_SRC_WARNING_INVALID_CODEC_RECEIVED  ( MM_STS_SRC_WARNING_BASE + 0x0003 )

#define MM_STS_SRC_INFO_UNKNOWN                    ( MM_STS_SRC_INFO_BASE + 0x0001 )
#define MM_STS_SRC_INFO_RTCP_BYE_RECEIVED          ( MM_STS_SRC_INFO_BASE + 0x0002 )
#define MM_STS_SRC_INFO_RTSP_CLOSING_STREAM        ( MM_STS_SRC_INFO_BASE + 0x0003 )
#define MM_STS_SRC_INFO_RTSP_SDP_RECEIVED          ( MM_STS_SRC_INFO_BASE + 0x0004 )
#define MM_STS_SRC_INFO_RTSP_PORT_CONNECTED        ( MM_STS_SRC_INFO_BASE + 0x0005 )
#define MM_STS_SRC_INFO_RTSP_PORT_INITIATED        ( MM_STS_SRC_INFO_BASE + 0x0006 )
#define MM_STS_SRC_INFO_RTP_SOCKET_SIZES           ( MM_STS_SRC_INFO_BASE + 0x0007 )
#define MM_STS_SRC_INFO_SOURCE_MEDIUM_UNKNOWN      ( MM_STS_SRC_INFO_BASE + 0x0008 )
#define MM_STS_SRC_INFO_SOURCE_RTSP_SERVER_URL     ( MM_STS_SRC_INFO_BASE + 0x0009 )
#define MM_STS_SRC_INFO_SOURCE_URL_OPEN            ( MM_STS_SRC_INFO_BASE + 0x000a )
#define MM_STS_SRC_INFO_SOURCE_URL_VIDEO           ( MM_STS_SRC_INFO_BASE + 0x000b )
#define MM_STS_SRC_INFO_SOURCE_EOF                 ( MM_STS_SRC_INFO_BASE + 0x000c )
#define MM_STS_SRC_INFO_SOURCE_URL_CLOSE           ( MM_STS_SRC_INFO_BASE + 0x000d )
#define MM_STS_SRC_INFO_SOURCE_URL_PAUSE           ( MM_STS_SRC_INFO_BASE + 0x000e )
#define MM_STS_SRC_INFO_SOURCE_URL_PLAY            ( MM_STS_SRC_INFO_BASE + 0x000f )
#define MM_STS_SRC_INFO_FFMPEG                     ( MM_STS_SRC_INFO_BASE + 0x0010 )
#define MM_STS_SRC_INFO_NO_NETWORK_FRAME_TIMEOUT   ( MM_STS_SRC_INFO_BASE + 0x0011 )
#define MM_STS_SRC_INFO_RTSP_START_STREAMING       ( MM_STS_SRC_INFO_BASE + 0x0012 )
#define MM_STS_SRC_INFO_RTSP_PAUSE_STREAMING       ( MM_STS_SRC_INFO_BASE + 0x0013 )
#define MM_STS_SRC_INFO_RTSP_SEEK_STREAMING        ( MM_STS_SRC_INFO_BASE + 0x0014 )
#define MM_STS_SRC_INFO_RTSP_STOP_STREAMING        ( MM_STS_SRC_INFO_BASE + 0x0015 )
#define MM_STS_SRC_INFO_SOURCE_URL_VIDEO_EX1       ( MM_STS_SRC_INFO_BASE + 0x0016 )
#define MM_STS_SRC_INFO_SOURCE_RTSP_VIDEO          ( MM_STS_SRC_INFO_BASE + 0x0017 )


// MM_STS_LIB_
#define MM_STS_LIB_ERROR_UNKNOWN					      ( MM_STS_LIB_ERROR_BASE + 0x0001 )
#define MM_STS_LIB_ERROR_UNINITIALISED             ( MM_STS_LIB_ERROR_BASE + 0x0002 )
#define MM_STS_LIB_ERROR_NOT_ENOUGH_MEMORY         ( MM_STS_LIB_ERROR_BASE + 0x0003 )
#define MM_STS_LIB_ERROR_BUFFER_NOT_VALID          ( MM_STS_LIB_ERROR_BASE + 0x0004 )
#define MM_STS_LIB_ERROR_INVALID_STATE             ( MM_STS_LIB_ERROR_BASE + 0x0005 )
#define MM_STS_LIB_ERROR_INVALID_INDEX             ( MM_STS_LIB_ERROR_BASE + 0x0006 )
#define MM_STS_LIB_ERROR_INVALID_HWND              ( MM_STS_LIB_ERROR_BASE + 0x0007 )
#define MM_STS_LIB_ERROR_INVALID_FILEPATH          ( MM_STS_LIB_ERROR_BASE + 0x0008 )
#define MM_STS_LIB_ERROR_INVALID_MODE              ( MM_STS_LIB_ERROR_BASE + 0x0009 )
#define MM_STS_LIB_ERROR_INVALID_POINTER           ( MM_STS_LIB_ERROR_BASE + 0x000a )
#define MM_STS_LIB_ERROR_SCREEN_SCRAPE             ( MM_STS_LIB_ERROR_BASE + 0x000b )
#define MM_STS_LIB_ERROR_DECODING_HEADER           ( MM_STS_LIB_ERROR_BASE + 0x000c )
#define MM_STS_LIB_ERROR_INCOMPATIBLE_API          ( MM_STS_LIB_ERROR_BASE + 0x000d )
#define MM_STS_LIB_ERROR_ALREADY_INITIALISED       ( MM_STS_LIB_ERROR_BASE + 0x000e )
#define MM_STS_LIB_ERROR_UNDEFINED_BEHAVIOR        ( MM_STS_LIB_ERROR_BASE + 0x000f )
#define MM_STS_LIB_ERROR_UNSUPPORTED_CODEC         ( MM_STS_LIB_ERROR_BASE + 0x0010 )
#define MM_STS_LIB_ERROR_HARDWARE_ABORTED          ( MM_STS_LIB_ERROR_BASE + 0x0011 )
#define MM_STS_LIB_ERROR_VIDEO_PARMS_CHANGED       ( MM_STS_LIB_ERROR_BASE + 0x0012 )

#define MM_STS_LIB_WARNING_UNKNOWN                 ( MM_STS_LIB_WARNING_BASE + 0x0001 )
#define MM_STS_LIB_WARNING_INPUT_DATA_DROPPED      ( MM_STS_LIB_WARNING_BASE + 0x0002 )
#define MM_STS_LIB_WARNING_NO_AVAILABLE_SURFACE    ( MM_STS_LIB_WARNING_BASE + 0x0003 )
#define MM_STS_LIB_WARNING_RESTARTING_HARDWARE     ( MM_STS_LIB_WARNING_BASE + 0x0004 )

#define MM_STS_LIB_INFO_UNKNOWN                    ( MM_STS_LIB_INFO_BASE + 0x0001 )
#define MM_STS_LIB_INFO_MORE_INPUT_DATA            ( MM_STS_LIB_INFO_BASE + 0x0002 )