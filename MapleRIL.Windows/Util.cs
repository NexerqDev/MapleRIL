using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MapleRIL.Windows
{
    public static class Util
    {
        public static Version AppVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public static string FriendlyAppVersion => $"v{AppVersion.Major}.{AppVersion.Minor}.{AppVersion.Build}";

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static BitmapFrame DrawingBmpToWpfBmp(System.Drawing.Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return BitmapFrame.Create(ms);
        }
    }
}
