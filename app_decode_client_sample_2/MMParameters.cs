using System.Runtime.InteropServices;

using MultiMedia;

namespace MM.SDK
{
   public enum CONTEXT
   {
      INTERNAL = 1,
      EXTERNAL = 2,
   }
   public enum SHOWSTATE
   {
      MINIMIZE = 0,
      MAXIMIZE = 1,
      RESTORE = 2,
      SHOW = 3,
      HIDE = 4,
   }
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public struct MM_WINDOW
   {
      public uint             Size;
      public MM_RECT          Placement;
      public bool             BorderAndTitleBar;
      public SHOWSTATE        ShowState;
      public int              ZOrder;
      public bool             TopMost;
      public uint             Alarm;
      public uint             AlarmRGB;
   }

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public class Open
   {
      public MM_CLIENT_OPEN   OpenParms;
      public string           URL;
      public string           URLSecondary;
      public string           UserName;
      public string           PassWord;
      public uint             Reserved;
      public string           Arguments;
   }
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public class Play
   {
      public MM_CLIENT_PLAY   PlayParms;
      public string           StartTime;
      public string           EndTime;
   }
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public class WindowForm
   {
      public MM_WINDOW        WindowParms;
   }
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public class Dewarp
   {
      public MM_CLIENT_DEWARP DewarpParms;
   }
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public class Zoom
   {
      public MM_RECT          ZoomParms;
   }
   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public class Source
   {
      public MM_CLIENT_SOURCE SourceParms;
   }

   [StructLayout(LayoutKind.Sequential, Pack = 1)]
   public class MMParameters
   {
      public uint uintID { get; set; }
      public uint ChildMarker { get; set; }
      public CONTEXT Context { get; set; }
      public int ARGBTheme { get; set; }
      public int TimeoutMS { get; set; }
      public string Logpath { get; set; }
      public string OSD { get; set; }
      public Open Open { get; set; }
      public Play Play { get; set; }
      public WindowForm Window { get; set; }
      public Dewarp Dewarp { get; set; }
      public Zoom Zoom { get; set; }
      public Source Source { get; set; }
      public bool AudioEnabled { get; set; }
      public bool AudioSubtitles { get; set; }
   }
}