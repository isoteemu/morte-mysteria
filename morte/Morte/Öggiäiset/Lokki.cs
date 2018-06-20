using System;
using System.Diagnostics;
using Jypeli;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morte.Öggiäiset
{

    class Lokki : Vihulainen
    {
        /// <summary>
        /// Siiveniskun voima
        /// </summary>
        protected Vector LiikeVektori = new Vector(300, 1800);

        public static Image[] Spritet = Game.LoadImages(new String[] { "vihulaiset/Lokki/lokki0001", "vihulaiset/Lokki/lokki0002", "vihulaiset/Lokki/lokki0003",
            "vihulaiset/Lokki/lokki0004", "vihulaiset/Lokki/lokki0005", "vihulaiset/Lokki/lokki0006", "vihulaiset/Lokki/lokki0007", "vihulaiset/Lokki/lokki0008",
            "vihulaiset/Lokki/lokki0009", "vihulaiset/Lokki/lokki0010", "vihulaiset/Lokki/lokki0011", "vihulaiset/Lokki/lokki0012", "vihulaiset/Lokki/lokki0013",
            "vihulaiset/Lokki/lokki0014", "vihulaiset/Lokki/lokki0015", "vihulaiset/Lokki/lokki0016", "vihulaiset/Lokki/lokki0017", "vihulaiset/Lokki/lokki0018",
            "vihulaiset/Lokki/lokki0019" });

        /// <summary>
        /// Suunta, mutta mirroroi myös lapsiobjektit.
        /// </summary>
        public new int Suunta {

            get { return base.Suunta; }
            set {
                if (value != base.Suunta)
                {
                    for(var i=0; i < this.ObjectCount; i++)
                    {
                        this.Objects[i].MirrorImage();
                    }
                }
                base.Suunta = value;
            }
        }

        /// <summary>
        /// Etäisyys maahan (Level.Bottom) jota yritetään ylläpitää.
        /// </summary>
        protected int Lentokorkeus = 120;

        protected GameObject Silmäluomet;

        public Lokki() : this(Spritet[0]) { }

        public Lokki(Image sprite) : base(sprite)
        {
            MaxHitpoints = 5 + RandomGen.NextInt(0,10);
            PisteArvo = 2;
            Vahinko = 10;

            Mass = 2;
            IsUpdated = true;

            Y = Game.Level.Bottom + Lentokorkeus;
            Animation = new Animation(Spritet);
            Animation.FPS = Animation.FPS / 2;
            Animation.Start();

            // Ilmanvastus
            LinearDamping = 0.985;

            // Silmät ovat juuri päinvastaiset, eli silmäluomet
            Silmäluomet = new PeliObjekti(Game.LoadImage("vihulaiset/Lokki/silmät"));
            Add(Silmäluomet);
            ToggleSilmät();
        }

        protected void ToggleSilmät()
        {
            Silmäluomet.IsVisible = !Silmäluomet.IsVisible;

            var hetki = RandomGen.NextDouble(0.5, 3.0);

            if (Silmäluomet.IsVisible)
                hetki *= 0.15;

            Timer.SingleShot(hetki, ToggleSilmät );
        }

        public override void Update(Time time)
        {
            base.Update(time);

            if (Kuollut) return;

            if (Y < Game.Level.Bottom + Lentokorkeus)
            {
                var impulssi = LiikeVektori;                

                if (Pelaaja.X > this.X) {
                    Suunta = Morte.OIKEA;
                    impulssi.X = Math.Min(impulssi.X, impulssi.X - Velocity.X);
                } else {
                    Suunta = Morte.VASEN;
                    impulssi.X = Math.Max(impulssi.X, Velocity.X - impulssi.X);
                }

                impulssi.X = Suunta * impulssi.X * RandomGen.NextDouble(0.8, 1.0);
                impulssi.Y = impulssi.Y * RandomGen.NextDouble(0.8, 1.0);

                this.Hit(impulssi);
                Animation.Start();
            }
        }
    }
}
