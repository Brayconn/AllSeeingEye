using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

namespace GuxtEditor
{
    static class CommonDraw
    {
        public static Image Scale(Image i, int scale, InterpolationMode mode = InterpolationMode.NearestNeighbor)
        {
            Bitmap o = new Bitmap(i.Width * scale, i.Height * scale);
            using (Graphics g = Graphics.FromImage(o))
            {
                g.InterpolationMode = mode;
                g.DrawImage(i, 0, 0, o.Width, o.Height);
            }
            return o;
        }
    }
}
