using System;
using System.Windows.Forms;
using System.Drawing;

using System.Threading;
using System.Diagnostics;

using MultiMedia;

namespace MM.SDK
{
   public partial class Window : Form
   {
      public IntPtr _HWnd = IntPtr.Zero;
      protected int _ARGBTheme = 0x00FFFF;
      protected string _statusMessage = "";
      protected bool _updatingWindow = false;
      protected Label _osd = null;

      public delegate bool updateWindowDelegate(MM_WINDOW window);
      public updateWindowDelegate myUpdateWindowDelegate;

      public Window(int argbTheme)
      {
         _ARGBTheme = argbTheme;
         myUpdateWindowDelegate = new updateWindowDelegate(updateWindowMethod);
         InitializeComponent();
      }
      public IntPtr GetHWND()
      {
         if (_HWnd == IntPtr.Zero)
         {
            int count = 0;
            while (count < 50)
            {
               if (_HWnd == IntPtr.Zero)
               {
                  count++;
                  Thread.Sleep(100);
               }
               else
                  break;
            }
            if (_HWnd == IntPtr.Zero)
               Debug.Assert(false);
         }
         return _HWnd;
      }

      public void SetOSDText(string txt, int wWidth, int wHeight)
      {
         if (this.InvokeRequired)
         {
            Debug.Assert(false);
            //this.Invoke(new Action<string, int, int>(SetOSDText), txt, wWidth, wHeight);
            return;
         }
         else
         {
            if (_osd == null)
            {
               _osd = new Label();
               _osd.Location = new Point(0, 0);
               _osd.ForeColor = Color.WhiteSmoke;
               _osd.BackColor = Color.Black;

               Controls.Add(this._osd); // add it to the form's controls
               _osd.BringToFront(); // bring it to the front, to display it above the picture box
            }
            // avoid divide by zero errors if called before this.Load event
            if (wWidth >= 80 && wHeight >= 60)
            {
               _osd.Font = new Font("Arial", 15, FontStyle.Regular);
               _osd.AutoSize = false; // if we later choose to try and scale on demand
               _osd.Size = new Size(wWidth, wHeight / 10);
               _osd.Font = new Font(_osd.Font.FontFamily, _osd.Font.Size, _osd.Font.Style);
               _osd.AutoSize = true;
               _osd.Text = txt;
            }
         }
      }
      public void SetWindowText(string txt)
      {
         if (this.InvokeRequired)
         {
            Debug.Assert(false);
            //this.Invoke(new Action<string>(SetWindowText), txt);
            return;
         }
         this.Text = txt;
      }
      public bool updateWindowMethod(MM_WINDOW window)
      {
         if (this.InvokeRequired)
         {
            Debug.Assert(false);
            //return (bool)this.Invoke(myUpdateWindowDelegate, new object[] { window });
            return false;
         }

         bool bRedraw = false;
         _updatingWindow = true;

         if (window.TopMost == false)
            this.TopMost = false;
         else
            this.TopMost = true;

         if (window.BorderAndTitleBar)
         {
            if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.SizableToolWindow) 
            { 
               this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow; 
               bRedraw = true; 
            }
         }
         else
         {
            if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None) 
            { 
               this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; 
               bRedraw = true; 
            }
         }

         switch (window.ShowState)
         {
            case SHOWSTATE.MAXIMIZE:
               if (this.WindowState != FormWindowState.Maximized) { this.WindowState = FormWindowState.Maximized; bRedraw = true; }
               break;
            case SHOWSTATE.MINIMIZE:
               if (this.WindowState != FormWindowState.Minimized) { this.WindowState = FormWindowState.Minimized; bRedraw = true; }
               break;
            case SHOWSTATE.RESTORE:
            default:
               if (this.WindowState != FormWindowState.Normal) { this.WindowState = FormWindowState.Normal; bRedraw = true; }
               break;
         }

         // must do after borders..
         if (this.Location.X != window.Placement.Left || this.Location.Y != window.Placement.Top || this.Size.Width != window.Placement.Right || this.Size.Height != window.Placement.Bottom)
         {
            bRedraw = true;
            this.SetDesktopBounds(window.Placement.Left, window.Placement.Top, window.Placement.Right, window.Placement.Bottom);
         }

         if (window.ShowState == SHOWSTATE.HIDE)
            this.Hide();
         else
            this.Show();

         _updatingWindow = false;
         return bRedraw;
      }
      protected void PaintStatus(string status)
      {
         if (this.InvokeRequired)
         {
            Debug.Assert(false);
            //this.Invoke(new Action<string>(PaintStatus), status);
            return;
         }
         _statusMessage = status;
         if (status.Length > 0 || _osd != null)
         {
            Graphics graphics = this.CreateGraphics();
            if (!graphics.IsVisibleClipEmpty)
            {
               MM_RECT area;
               area.Left = 0; area.Top = 0; area.Right = (int)graphics.VisibleClipBounds.Width; area.Bottom = (int)graphics.VisibleClipBounds.Height;
               if (status.Length > 0)
               {
                  using (Font fontScaled = new Font("Arial", 120, FontStyle.Bold, GraphicsUnit.Pixel))
                  {
                     ApplyLetterBoxing(ref area);

                     Rectangle bounds = new Rectangle(area.Left, area.Top, area.Right - area.Left, area.Bottom - area.Top);

                     graphics.Clear(Color.Black);

                     Font goodFont = FindFont(graphics, status, bounds.Size, fontScaled);

                     TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter |
                     TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis;

                     TextRenderer.DrawText(graphics, status, goodFont,
                           new Rectangle(area.Left, area.Top, area.Right - area.Left, area.Bottom - area.Top),
                           Color.FromArgb((byte)((_ARGBTheme >> 16) & 0xff), (byte)((_ARGBTheme >> 8) & 0xff), (byte)(_ARGBTheme & 0xff)),
                           flags);
                     graphics.Dispose();
                  }
               }
               if (_osd != null)
                  SetOSDText(_osd.Text, area.Right, area.Bottom);
            }
         }
      }

      private void ApplyLetterBoxing(ref MM_RECT win)
      {
         // avoid fisheye text overhang as 1:1 source when letterboxing applied.
         const float aspectRatio = (float)1 / (float)1;
         float targetW = Math.Abs((float)(win.Right - win.Left));
         float targetH = Math.Abs((float)(win.Bottom - win.Top));

         float tempH = targetW / aspectRatio;

         if (tempH <= targetH)
         {
            float deltaH = Math.Abs(tempH - targetH) / 2;
            win.Top += (int)deltaH;
            win.Bottom -= (int)deltaH;
         }
         else
         {
            float tempW = targetH * aspectRatio;
            float deltaW = Math.Abs(tempW - targetW) / 2;

            win.Left += (int)deltaW;
            win.Right -= (int)deltaW;
         }
      }
      // This function checks the room size and your text and appropriate font
      // PreferedFont is the Font that you wish to apply
      // Room is your space in which your text should be in.
      // LongString is the string which it's bounds is more than room bounds.
      private Font FindFont(
         System.Drawing.Graphics g,
         string longString,
         Size Room,
         Font PreferedFont)
      {
         SizeF RealSize = g.MeasureString(longString, PreferedFont);
         float HeightScaleRatio = Room.Height / RealSize.Height;
         float WidthScaleRatio = Room.Width / RealSize.Width;

         float ScaleRatio = (HeightScaleRatio < WidthScaleRatio)
            ? ScaleRatio = HeightScaleRatio
            : ScaleRatio = WidthScaleRatio;

         float ScaleFontSize = PreferedFont.Size * ScaleRatio;

         return new Font(PreferedFont.FontFamily, ScaleFontSize);
      }
   }
}
