using System;
using System.Diagnostics;
using Jypeli;

namespace Morte.Öggiäiset
{
    public class Pappi : Sankari
    {
        public static Image[] Spritet = Game.LoadImages(new string[] {"pappi"});

        public Pappi() : base(Spritet[0])
        {
            Sijainti_Suu = new Vector(-2, 46);
            
            //IgnoresPhysicsLogics = true;
            //IgnoresCollisionResponse = true;

            Add(LuoSilmä(Morte.VASEN));
            Add(LuoSilmä(Morte.OIKEA));
        }

        public override void LataaOletukset()
        {
            base.LataaOletukset();
            Mass = 120;
        }

        public override void Liikuta()
        {
            base.Liikuta();
        }

        protected GameObject LuoSilmä(int suunta)
        {
            Silmä silmä = new Silmä(9, 10);

            silmä.X = (silmä.Width / 2 + 1) * suunta;
            silmä.Y = 62;

            return silmä;
        }

        /// <summary>
        /// Oma GameObject silmien pyöritystä varten Update()n aikana.
        /// </summary>
        class Silmä : PeliObjekti, ÄLÄ_KÄÄNNÄ
        {
            public Silmä(double width = 9, double height = 10) : base(width, height)
            {
                Image = Game.LoadImage("silma");
                IsUpdated = true;
            }

            public override void Update(Time time)
            {
                var mouse = Game.Mouse.PositionOnWorld;
                var rad = Math.Atan2(mouse.Y - Y, mouse.X - X);
                this.AbsoluteAngle = Angle.FromRadians(rad);

                base.Update(time);
            }
        }
    }
}
