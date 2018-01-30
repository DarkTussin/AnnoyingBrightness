using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnnoyingBrightness
{
    public partial class Form1 : Form
    {
        [DllImport("gdi32.dll")]
        private unsafe static extern bool SetDeviceGammaRamp(Int32 hdc, void* ramp);
        private static bool initialized = false;
        private static Int32 hdc;
        private short curBrightness = 100;
        private Timer ticky = new Timer();
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            runLoad();
        }
        private void runLoad()
        {
            ticky.Interval = 1;
            ticky.Tick += ticky_Tick;
            ticky.Start();
        }
        void ticky_Tick(object sender, EventArgs e)
        {
            ticky.Interval = 20000;
            MessageBox.Show("Your battery is critically low! (15%)" + Environment.NewLine + "Would you like to enable battery saving mode?", "Battery Critically Low!", MessageBoxButtons.YesNo);
            curBrightness -= 20;
            SetBrightness(curBrightness);
        }
        private static void InitializeClass()
        {
            if (initialized)
                return;
            hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc().ToInt32();
            initialized = true;
        }
        public static unsafe bool SetBrightness(short brightness)
        {
            InitializeClass();
            if (brightness > 255)
                brightness = 255;
            if (brightness < 0)
                brightness = 0;
            short* gArray = stackalloc short[3 * 256];
            short* idx = gArray;
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 256; i++)
                {
                    int arrayVal = i * (brightness + 128);
                    if (arrayVal > 65535)
                        arrayVal = 65535;
                    *idx = (short)arrayVal;
                    idx++;
                }
            }
            bool retVal = SetDeviceGammaRamp(hdc, gArray);
            return retVal;
        }
    }
}
