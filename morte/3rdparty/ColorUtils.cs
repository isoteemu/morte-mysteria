using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jypeli;

namespace Morte
{
    /// <summary>
    /// Muunna HSV arvot RGB väriksi
    /// </summary>
    public class ColorUtils
    {
        /// Source: http://www.java2s.com/Code/CSharp/2D-Graphics/HsvToRgb.htm
        public static Color HsvToRgb(double h, double s, double v)
        {
            int hi = (int)Math.Floor(h / 60.0) % 6;
            double f = (h / 60.0) - Math.Floor(h / 60.0);

            double p = v * (1.0 - s);
            double q = v * (1.0 - (f * s));
            double t = v * (1.0 - ((1.0 - f) * s));

            Color ret;

            switch (hi)
            {
                case 0:
                    ret = ColorUtils.GetRgb(v, t, p);
                    break;
                case 1:
                    ret = ColorUtils.GetRgb(q, v, p);
                    break;
                case 2:
                    ret = ColorUtils.GetRgb(p, v, t);
                    break;
                case 3:
                    ret = ColorUtils.GetRgb(p, q, v);
                    break;
                case 4:
                    ret = ColorUtils.GetRgb(t, p, v);
                    break;
                case 5:
                    ret = ColorUtils.GetRgb(v, p, q);
                    break;
                default:
                    ret = new Color(byte.MinValue, byte.MinValue, byte.MinValue, byte.MaxValue);
                    break;
            }
            return ret;
        }
        public static Color GetRgb(double r, double g, double b)
        {
            return new Color((byte)(r * 255.0), (byte)(g * 255.0), (byte)(b * 255.0), byte.MaxValue);
        }

    }
}
