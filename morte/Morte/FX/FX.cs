using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jypeli;

namespace Morte.FX
{
    public class SpriteFX : PeliObjekti
    {

        const int TASO = -1;

        protected bool SeuraaKulma = true;
        protected bool SeuraaSijainti = true;

        protected GameObject Sidos { get; set; }

        public SpriteFX(Image sprite) : base(sprite) { }

        public SpriteFX(Animation animation) : base(animation)
        {
            IsUpdated = true;
        }

        public static Animation LoadAnimation(string basename, int count = 1, int offset = 0)
        {
            Image[] kuvat = new Image[count];
            for (int i = 0; i < kuvat.Length; i++)
            {
                kuvat[i] = Game.LoadImage(String.Format("{0}{1:D4}", basename, i + offset));
            }
            return new Animation(kuvat);
        }

        /// <summary>
        /// Sitoo itsensä seuraamaan jotain toista objektia.
        /// </summary>
        /// <param name="sidos"></param>
        public void Bind(GameObject sidos)
        {
            Sidos = sidos;
            sidos.Destroyed += Destroy;
            Position = sidos.AbsolutePosition;
            Angle = sidos.AbsoluteAngle;
            Game.Add(this, TASO);
        }
        
        public override void Update(Time time)
        {
            if(Parent == null && Sidos != null)
            {
                if(SeuraaSijainti)
                    Position = Sidos.AbsolutePosition;
                if(SeuraaKulma)
                    Angle = Sidos.AbsoluteAngle;

                IsVisible = Sidos.IsVisible;
            }
            base.Update(time);
        }

    }

    public class Sunrays : SpriteFX
    {

        public const double ANGLE_ASKEL = 30;

        public Sunrays() : base(Game.LoadImage("FX/Sunrays"))
        {
            SeuraaKulma = false;
            IsUpdated = true;
        }

        public override void Update(Time time)
        {
            base.Update(time);

            double r = (double)time.SinceStartOfGame.Seconds + (double)time.SinceStartOfGame.Milliseconds / 1000;
            r *= ANGLE_ASKEL;
            
            Angle = Angle.FromDegrees((Angle.Degrees - r) % 360);

        }
    }

    class RadiationFX : SpriteFX
    {
        public RadiationFX() : base(LoadAnimation("FX/energy/energy", 10))
        {
            SeuraaKulma = false;

            Animation.FPS = 15;
            Animation.Start();
            
        }

    }
}
