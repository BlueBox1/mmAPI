/*
 * app_decode_encode_sample_2d - uncompressed and compressed video source callback example
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


using MultiMedia;
using MM.SDK;

#if USING_OPEN_CV
// Using OpenCvSharp4.Windows all in one package
using OpenCvSharp;
#endif

namespace app_decode_encode_sample_2a
{
   class Program
   {
      private static class CommandLine
      {
         public static string _url;
         public static string _path;
         public static uint _interval;
         public static uint _count;
      }
      private static MM_DATA_CBKFN _dataCBKFN = new MM_DATA_CBKFN(DataCallbackFN);
      private static MM_STATUS_CBKFN _statusCBKFN = new MM_STATUS_CBKFN(StatusCallbackFN);
      private static mmStatus _sts = mmStatus.MM_STS_SRC_ERROR_UNINITIALISED;
      private static MMParameters _parmsDecode = new MMParameters();
      private static IntPtr _hSession = IntPtr.Zero;
      private static bool _play = true;
      private static uint _sampled = 0;
      private static uint _samples = 0;

      static int Main()
      {
         if (GetCommandLine())
         {
            // load the multimedia API
            MM_LOAD load = new MM_LOAD();
            if (MMHelper.MMLoad(ref load, Environment.CurrentDirectory))
            {
               MMHelper.InitClientParms(1, _parmsDecode);
               _parmsDecode.Open.UserName = "";
               _parmsDecode.Open.PassWord = "";

               string basicAuth = CommandLine._url;

               if (!string.IsNullOrEmpty(_parmsDecode.Open.UserName) && !string.IsNullOrEmpty(_parmsDecode.Open.PassWord))
               {
                  var index = CommandLine._url.IndexOf("://");
                  if (index != -1)
                     basicAuth = basicAuth.Insert(index + 3, _parmsDecode.Open.UserName + ":" + _parmsDecode.Open.PassWord + "@");
               }

               // account for UTF8 encoded data
               int len = Encoding.UTF8.GetByteCount(basicAuth);
               byte[] utf8Bytes = new byte[len + 1];
               Encoding.UTF8.GetBytes(basicAuth, 0, basicAuth.Length, utf8Bytes, 0);
               IntPtr nativeUtf8 = Marshal.AllocHGlobal(utf8Bytes.Length);
               Marshal.Copy(utf8Bytes, 0, nativeUtf8, utf8Bytes.Length);
               _parmsDecode.Open.OpenParms.PURL = nativeUtf8;

               _parmsDecode.Open.OpenParms.BNetTCP = 1;
               _parmsDecode.Open.OpenParms.PDataCBKFN = _dataCBKFN;
               _parmsDecode.Open.OpenParms.PStatusCBKFN = _statusCBKFN;
               _parmsDecode.Open.OpenParms.OpenFlags = MM_CLIENT_REQUEST.MM_CLIENT_REQUEST_VIDEO_SOURCE |
                                                       MM_CLIENT_REQUEST.MM_CLIENT_REQUEST_VIDEO_DECODED;

               _sts = mmMethods.mmClientOpen(out _hSession, ref _parmsDecode.Open.OpenParms);
               if (_sts == mmStatus.MM_STS_NONE)
               {
                  _sts = mmMethods.mmClientPlay(_hSession, ref _parmsDecode.Play.PlayParms);
                  if (_sts == mmStatus.MM_STS_NONE)
                  {
                     do
                     {
                        System.Threading.Thread.Sleep(1000);
                     } while (_play);
                  }
                  mmMethods.mmClose(_hSession);
               }
               Marshal.FreeHGlobal(_parmsDecode.Open.OpenParms.PURL);
               mmMethods.mmRelease(load.HInterface);
            }
            if (_sts != mmStatus.MM_STS_NONE)
               Console.WriteLine($"ERROR - Status 0x{_sts:X}");
         }
         return (int)_sts;
      }
      private static bool GetCommandLine()
      {
         try
         {
            string[] args = Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
               Console.WriteLine("Arg[{0}] = [{1}]", i, args[i]);

            // simple cmd line parser
            Dictionary<string, string> retval = args.ToDictionary(
                 k => k.Split(new char[] { '=' }, 2)[0].ToLower(),
                 v => v.Split(new char[] { '=' }, 2).Count() > 1
                  ? v.Split(new char[] { '=' }, 2)[1]
                  : null);

            uint tmp;
            foreach (var arg in retval) // <url> <saveCount> <saveInterval> <saveName>
            {
               if (arg.Key == "-url")
                  CommandLine._url = arg.Value;
               else if (arg.Key == "-savecount")
               {
                  if (uint.TryParse(arg.Value, out tmp))
                     CommandLine._count = tmp;
               }
               else if (arg.Key == "-saveinterval")
               {
                  if (uint.TryParse(arg.Value, out tmp))
                     CommandLine._interval = tmp;
               }
               else if (arg.Key == "-savename")
               {
                  CommandLine._path = arg.Value;
               }
            }

            if (string.IsNullOrEmpty(CommandLine._url))
               throw new System.ArgumentException("url");
            if (string.IsNullOrEmpty(CommandLine._path))
               throw new System.ArgumentException("saveName");
            if (CommandLine._count == 0)
               throw new System.ArgumentException("saveCount");
         }
         catch
         {
            string msg = "\nusage:\n<-url> <-saveCount> <-saveInterval> <-saveName>";
            Console.WriteLine(msg);
            return false;
         }
         return true;
      }
      private static mmStatus DataCallbackFN(IntPtr hSession, IntPtr pMediaSample, IntPtr pUserData)
      {
         try
         {
            MM_DATA mediaSample = (MM_DATA)Marshal.PtrToStructure(pMediaSample, typeof(MM_DATA));

            // ignore uncompressed frames and corrupt compressed frames
            if (((mediaSample.ContextFlag & (uint)MM_DATA_CONTEXT.MM_DATA_CONTEXT_UNCOMPRESSED_VIDEO) == 0) ||
                ((mediaSample.ContextFlag & (uint)MM_DATA_CONTEXT.MM_DATA_CONTEXT_UNCOMPRESSED_VIDEO_CORRUPTION) == (uint)MM_DATA_CONTEXT.MM_DATA_CONTEXT_UNCOMPRESSED_VIDEO_CORRUPTION))
            {
               return mmStatus.MM_STS_NONE;
            }

            Console.WriteLine($"MM_DATA - Session 0x{hSession:X} FourCC {mediaSample.FourCC}");

            _samples++;
            if (_samples % CommandLine._interval != 0)
               return mmStatus.MM_STS_NONE;

            _sampled++;
            if (_sampled > CommandLine._count)
            {
               _play = false;
               return mmStatus.MM_STS_NONE;
            }

            uint managedSize = mediaSample.Width * mediaSample.Height * 3 / 2;
            byte[] managed = new byte[managedSize];
            int dIndex = 0;
            int sIndex = 0;

            if ((MAKEFOURCC('n', 'v', '1', '2') == mediaSample.FourCC) ||
               (MAKEFOURCC('N', 'V', '1', '2') == mediaSample.FourCC) ||
               (MAKEFOURCC('n', 'v', '2', '1') == mediaSample.FourCC) ||
               (MAKEFOURCC('N', 'V', '2', '1') == mediaSample.FourCC))
            {
               for (uint y = 0; y < mediaSample.Height; y++) // y plane
               {
                  Marshal.Copy(mediaSample.PData[0] + sIndex, managed, dIndex, (int)mediaSample.Width);
                  dIndex += (int)mediaSample.Width;
                  sIndex += (int)mediaSample.Pitch[0];
               }
               sIndex = 0;
               for (uint y = 0; y < mediaSample.Height / 2; y++) // uv plane (interleaved)
               {
                  Marshal.Copy(mediaSample.PData[1] + sIndex, managed, dIndex, (int)mediaSample.Width);
                  dIndex += (int)mediaSample.Width;
                  sIndex += (int)mediaSample.Pitch[1];
               }
            }
            else if ((MAKEFOURCC('y', 'v', '1', '2') == mediaSample.FourCC) ||
                     (MAKEFOURCC('Y', 'V', '1', '2') == mediaSample.FourCC))
            {
               for (uint y = 0; y < mediaSample.Height; y++) // y plane
               {
                  Marshal.Copy(mediaSample.PData[0] + sIndex, managed, dIndex, (int)mediaSample.Width);
                  dIndex += (int)mediaSample.Width;
                  sIndex += (int)mediaSample.Pitch[0];
               }
               sIndex = 0;
               for (uint y = 0; y < mediaSample.Height / 2; y++) // v plane
               {
                  Marshal.Copy(mediaSample.PData[2] + sIndex, managed, dIndex, (int)mediaSample.Width / 2);
                  dIndex += (int)mediaSample.Width / 2;
                  sIndex += (int)mediaSample.Pitch[2];
               }
               sIndex = 0;
               for (uint y = 0; y < mediaSample.Height / 2; y++) // u plane
               {
                  Marshal.Copy(mediaSample.PData[1] + sIndex, managed, dIndex, (int)mediaSample.Width / 2);
                  dIndex += (int)mediaSample.Width / 2;
                  sIndex += (int)mediaSample.Pitch[1];
               }
            }
            else if ((MAKEFOURCC('i', '4', '2', '0') == mediaSample.FourCC) ||
                     (MAKEFOURCC('I', '4', '2', '0') == mediaSample.FourCC) ||
                     (MAKEFOURCC('i', 'y', 'u', 'v') == mediaSample.FourCC) ||
                     (MAKEFOURCC('I', 'Y', 'U', 'V') == mediaSample.FourCC))
            {
               for (uint y = 0; y < mediaSample.Height; y++) // y plane
               {
                  Marshal.Copy(mediaSample.PData[0] + sIndex, managed, dIndex, (int)mediaSample.Width);
                  dIndex += (int)mediaSample.Width;
                  sIndex += (int)mediaSample.Pitch[0];
               }
               sIndex = 0;
               for (uint y = 0; y < mediaSample.Height / 2; y++) // u plane
               {
                  Marshal.Copy(mediaSample.PData[1] + sIndex, managed, dIndex, (int)mediaSample.Width / 2);
                  dIndex += (int)mediaSample.Width / 2;
                  sIndex += (int)mediaSample.Pitch[1];
               }
               sIndex = 0;
               for (uint y = 0; y < mediaSample.Height / 2; y++) // v plane
               {
                  Marshal.Copy(mediaSample.PData[2] + sIndex, managed, dIndex, (int)mediaSample.Width / 2);
                  dIndex += (int)mediaSample.Width / 2;
                  sIndex += (int)mediaSample.Pitch[2];
               }
            }
            else
            {
               _sts = mmStatus.MM_STS_SRC_ERROR_INVALID_DATA;
               Console.WriteLine($"MM_DATA - Session 0x{hSession:X} {mediaSample.FourCC} Not Processed");
               _play = false;
            }

#if USING_OPEN_CV

            ColorConversionCodes cs;

            if ((MAKEFOURCC('n', 'v', '1', '2') == mediaSample.FourCC) ||
               (MAKEFOURCC('N', 'V', '1', '2') == mediaSample.FourCC) ||
               (MAKEFOURCC('n', 'v', '2', '1') == mediaSample.FourCC) ||
               (MAKEFOURCC('N', 'V', '2', '1') == mediaSample.FourCC))
            {
               cs = ColorConversionCodes.YUV2BGR_NV12;
            }
            else if ((MAKEFOURCC('y', 'v', '1', '2') == mediaSample.FourCC) ||
                     (MAKEFOURCC('Y', 'V', '1', '2') == mediaSample.FourCC))
            {
               cs = ColorConversionCodes.YUV2BGR_YV12;
            }
            else if ((MAKEFOURCC('i', '4', '2', '0') == mediaSample.FourCC) ||
                     (MAKEFOURCC('I', '4', '2', '0') == mediaSample.FourCC) ||
                     (MAKEFOURCC('i', 'y', 'u', 'v') == mediaSample.FourCC) ||
                     (MAKEFOURCC('I', 'Y', 'U', 'V') == mediaSample.FourCC))
            {
               cs = ColorConversionCodes.YUV2BGR_I420;
            }
            else
               return mmStatus.MM_STS_NONE;

            string samplePath = $"{CommandLine._path}_{_sampled}.bmp";
            Mat picYV12 = new Mat((int)((mediaSample.Height * 3) / 2), (int)mediaSample.Width, MatType.CV_8UC1, managed);
            Mat picBGR = new Mat();
            Cv2.CvtColor(picYV12, picBGR, cs);
            Cv2.ImWrite(samplePath, picBGR);
         
#endif
         }
         catch (Exception e)
         {
            Console.WriteLine("{0} Exception caught.", e);
         }
         return mmStatus.MM_STS_NONE;
      }

      private static void StatusCallbackFN(IntPtr hSession, uint status, IntPtr pMessage, IntPtr pUserData)
      {
         try
         {
            if ((((uint)status & (uint)mmStatusBase.MM_STS_SRC_INFO_BASE) == (uint)mmStatusBase.MM_STS_SRC_INFO_BASE) ||
                (((uint)status & (uint)mmStatusBase.MM_STS_LIB_INFO_BASE) == (uint)mmStatusBase.MM_STS_LIB_INFO_BASE))
            {
               Console.WriteLine($"INFO - Session 0x{hSession:X} Status 0x{status:X} - {Marshal.PtrToStringAnsi(pMessage)}");
            }
            else if ((((uint)status & (uint)mmStatusBase.MM_STS_SRC_WARNING_BASE) == (uint)mmStatusBase.MM_STS_SRC_WARNING_BASE) ||
                     (((uint)status & (uint)mmStatusBase.MM_STS_LIB_WARNING_BASE) == (uint)mmStatusBase.MM_STS_LIB_WARNING_BASE))
            {
               Console.WriteLine($"WARNING - Session 0x{hSession:X} Status 0x{status:X} - {Marshal.PtrToStringAnsi(pMessage)}");
            }
            else if ((((uint)status & (uint)mmStatusBase.MM_STS_SRC_ERROR_BASE) == (uint)mmStatusBase.MM_STS_SRC_ERROR_BASE) ||
                     (((uint)status & (uint)mmStatusBase.MM_STS_LIB_ERROR_BASE) == (uint)mmStatusBase.MM_STS_LIB_ERROR_BASE))
            {
               Console.WriteLine($"ERROR - Session 0x{hSession:X} Status 0x{status:X} - {Marshal.PtrToStringAnsi(pMessage)}");
               _play = false;
            }
         }
         catch (Exception e)
         {
            Console.WriteLine("{0} Exception caught.", e);
         }
      }

      public static UInt32 MAKEFOURCC(char ch0, char ch1, char ch2, char ch3)
      {
         UInt32 result = ((UInt32)(byte)(ch0) | ((UInt32)(byte)(ch1) << 8) |
                 ((UInt32)(byte)(ch2) << 16) | ((UInt32)(byte)(ch3) << 24));

         return result;
      }
   }
}