using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jypeli;
using Glide;

using Morte.FX;

namespace Morte.Loot
{
    /// <summary>
    /// Sieni -lootitem. Kasvattaa pelaajan koon kaksinkertaiseksi.
    /// </summary>
    class Sieni : Härpäke
    {
        const int VAUHTI = 120;

        public int Suunta { get; set; }

        static string AssetsPath = "loot/Sieni/";

        protected bool Käytetty = false;

        protected SpriteFX FX = new RadiationFX();

        public Sieni() : base(Game.LoadImage(AssetsPath + "sprite")) {
            Mass = 4;
            IsUpdated = true;
            Shape = Shape.Circle;
            CanRotate = false;

            OnPaljastus += Karkaa;
            OnPoiminta += Pakene;

            FX.Bind(this);
        }

        protected void Karkaa()
        {
            if (X < Morte.Instance.Pelaaja.X)
                Suunta = Morte.VASEN;
            else
                Suunta = Morte.OIKEA;

            /// TODO: Lisää myös muihin objekteihin kohdistuvat vertikaaliset törmäykset
            Morte.Instance.AddCollisionHandlerByTag<Sieni, Öggiäinen>(this, "vihu", KäännäSuunta);
            Hit(new Vector(3000 * Suunta, 900));
        }

        protected void Pakene() {

            if (X < Poimija.X)
                Suunta = Morte.VASEN;
            else
                Suunta = Morte.OIKEA;

            Morte.Instance.AddCollisionHandler<Sieni, Öggiäinen>(this, Poimija, SyöMinut);
        }

        public void KäännäSuunta(Sieni sieni, Öggiäinen vihu)
        {
            sieni.Suunta = -1 * sieni.Suunta;
        }

        public void SyöMinut(Sieni sieni, Öggiäinen poimija)
        {

            if (Käytetty) return;
            Käytetty = true;

            Velocity = Vector.Zero;
            Stop();

            //taulu.PreferredSize = new Vector(100, 100);
            Debug.WriteLine("Sieni syöty");
            Tweetteri.Tween(poimija, new {
                Width = poimija.Width * 2,
                Height = poimija.Height * 2,
                Y = poimija.Y + poimija.Height / 2}, 1.5f, 0, false).Ease(Ease.BounceIn);

            Morte.Instance.Kantama = Morte.Instance.Kantama * 2;

            IgnoresPhysicsLogics = true;
            IgnoresCollisionResponse = true;
            
            Tweetteri.Tween(this, new { Width = 1, Height = 1 }, 1.51f, 0,false).Ease(Ease.QuadOut).OnComplete(() => Destroy());
        }


        public override void Update(Time time)
        {
            base.Update(time);
            if (!Käytetty) {

                if (Game.Level.Left - Width > this.X || Game.Level.Right + Width < this.X)
                    Destroy();

                var force = VAUHTI - Math.Abs(Velocity.X);

                //Push(new Vector(force * Suunta, 0));
                Hit(new Vector(force * Suunta, 0));
            }

        }
    }
}
