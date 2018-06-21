using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using Jypeli;

namespace Morte.Aseet
{
    class Oksennus: IAse
    {
        const double KOKO = 12d;
        const int PALLOJA = 2;
        const double VOIMA = 400d;

        public double Koko = KOKO;
        public double Voima = VOIMA;
        public int Palloja = PALLOJA;

        protected Timer Toistin;

        public void Laukaise(Morte peli)
        {
            Debug.WriteLine("Asetetaan ajastin");

            if (Toistin != null)
                Toistin.Stop();

            Toistin = new Timer();
            Toistin.Interval = 0.01;
            Toistin.Timeout += () => Palleroi(peli);
            Toistin.Start();
        }

        public void Pysäytä()
        {

            if (Toistin != null)
                Toistin.Stop();
        }

        public void Palleroi(Morte peli)
        {
            for (int i = 0; i < Palloja; i++)
            {
                double scale = RandomGen.NextDouble(0.5, 1.5);
                double dir = RandomGen.NextDouble(-10, 10);
                double velocity = VOIMA;
                double hue = (dir + 10) / 20 * 240;

                var p = new FysiikkaObjekti(Koko * scale, Koko * scale);
                p.Color = ColorUtils.HsvToRgb(hue, 1, 0.9);
                p.Shape = Shape.Circle;
                p.LifetimeLeft = TimeSpan.FromSeconds(2);

                p.Position = peli.Pelaaja.AbsolutePosition + peli.Pelaaja.Sijainti_Suu;

                var mouse = peli.Mouse.PositionOnWorld;
                var rad = Math.Atan2(mouse.Y - p.Position.Y, mouse.X - p.Position.X);

                p.Hit(Vector.FromLengthAndAngle(velocity, Angle.FromRadians(rad) + Angle.FromDegrees(dir)));

                p.AddCollisionIgnoreGroup(Sankari.IGNORE_ID);
                peli.AddCollisionHandlerByTag<FysiikkaObjekti, Vihulainen>(p, "vihu", OsuVihuun);

                peli.Add(p, Morte.TASO_OLETUS);
            }
            Toistin.Interval += 0.002;

            peli.Pelaaja.Vahingoita(2);
        }

        public void OsuVihuun(FysiikkaObjekti pallura, Vihulainen vihu)
        {
            vihu.Vahingoita(1);
            pallura.Destroy();
        }

    }
}
