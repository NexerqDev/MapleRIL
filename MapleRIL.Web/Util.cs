using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Web
{
    public static class Util
    {
        public static byte[] BitmapToBytes(System.Drawing.Bitmap bmp, System.Drawing.Imaging.ImageFormat format = null)
        {
            if (format == null)
                format = System.Drawing.Imaging.ImageFormat.Png;

            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, format);
                return ms.ToArray();
            }
        }
    }
}
