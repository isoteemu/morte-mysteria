using System;
using System.Diagnostics;
using Jypeli;
using Jypeli.Physics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morte.Loot
{
    class Hattu : PudotettavaHärpäke
    {
        static string AssetsPath = "loot/hattu/";
        
        public Hattu() : base(Game.LoadImage(AssetsPath+"sprite")) {
            OnPoiminta += AsetaPäähän;
            OnKlikattattaessa += TipsFedora;

            LinearDamping = 0.99;
            Mass = 0.3;
            Shape = Shape.Circle;
        }

        protected void TipsFedora()
        {
            if (Parent == null) return;

            var impulssi = new Vector(RandomGen.NextDouble(40, 100) * Poimija.Suunta, RandomGen.NextDouble(40, 100));
            this.Hit(impulssi);
            Mass = double.PositiveInfinity;
            Game.LoadSoundEffect(AssetsPath + "sound").Play();
            Timer.SingleShot(2, Destroy);
        }

        public void AsetaPäähän()
        {
            if (Poimija.Suunta == Morte.OIKEA)
                MirrorImage();

            X = 0;
            Y = Poimija.Height / 2;

            IsUpdated = true;
        }

    }
}
