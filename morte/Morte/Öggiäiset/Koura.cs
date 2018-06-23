using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glide;
using Jypeli;

namespace Morte.Öggiäiset
{
    public class Koura : Vihulainen
    {

        const float Hyökkimisen_Kesto = 0.7f;
        const double MASSA_LISÄYS = 9999;

        public static Image[] Spritet = Game.LoadImages(new String[] { "vihulaiset/koura-auki", "vihulaiset/koura-kiinni" });

        protected double Pelaajan_Sijainti;

        protected bool Hyökkimässä = false;
        protected bool Napattu = false;

        public Koura() : this(Spritet[0]) { }

        public Koura(Image sprite) : base(sprite)
        {
            MaxHitpoints = RandomGen.NextInt(1) + 5;
            Vahinko = 5;
            PisteArvo = 1;

            CanRotate = false;
            IgnoresGravity = true;

            Mass = double.PositiveInfinity;
            Shape = Shape.Rectangle;

            //Y = Game.Level.Bottom - Height / 2;

            OnKuolema += VapautaPelaaja;
        }

        public override void HyökiLoop()
        {
            if (Hyökkimässä || Napattu) return;
            Hyökkimässä = true;

            X = RandomGen.NextDouble(Game.Level.Left + Width, Game.Level.Right - Width);

            // Aseta ylösnousun animaatio.
            Tweetteri.Tween(this, new { Y = Game.Level.Bottom + Height / 2 }, Hyökkimisen_Kesto).OnComplete(delegate ()
            {
                // Esillä, aseta piilotuksen animaation.
                Tweetteri.Tween(this, new { Y = Game.Level.Bottom - Height / 2 }, Hyökkimisen_Kesto * 2).OnComplete(delegate ()
                 {
                     Hyökkimässä = false;
                 });
            });

            base.HyökiLoop();
        }

        public override void VahingoitaSankaria(Öggiäinen sankari)
        {
            base.VahingoitaSankaria(sankari);
            
            if (!Kuollut && ! Napattu)
            {
                Debug.WriteLine("Koura sai napattua sankarin");

                Tweetteri.Cancel();
                Tweetteri.Update(0.0f);

                Image = Spritet[1];

                TarraaPelaajaan();

                // Paljasta pintaa.
                if (Y < Game.Level.Bottom)
                    Y = Game.Level.Bottom + Height/5;
            }
        }

        protected void TarraaPelaajaan()
        {
            Napattu = true;
            Pelaajan_Sijainti = Pelaaja.X - X;

            Pelaaja.Mass += MASSA_LISÄYS;
        }

        protected void VapautaPelaaja()
        {
            Image = Spritet[0];

            Debug.WriteLine("Pelaajan Massa:" + Pelaaja.Mass);

            if(Napattu && Pelaaja.Mass >= MASSA_LISÄYS)
            {
                Pelaaja.Mass -= MASSA_LISÄYS;
            }
        }

        public override void KuolinAnimaatio()
        {
            var _Y = Game.Screen.Top + Height;
            var _X = X + RandomGen.NextDouble(-40, 40);

            Tweetteri.Tween(this, new { Y = Game.Level.Bottom - Height / 2, Vaaleus = 1.0 }, KuolemanKesto)
                .Ease(Ease.QuadIn).OnComplete(ReallyKuole);

        }

        public override void Update(Time time)
        {
            if (Napattu)
                X = Pelaaja.X - Pelaajan_Sijainti;

            base.Update(time);
        }

    }
}
