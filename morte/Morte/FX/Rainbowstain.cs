using System;
using Jypeli;
using Jypeli.Effects;
using Microsoft.Xna.Framework;

namespace Morte.FX { 

    public class Rainbowstain : ParticleSystem
    {
        public const string Sprite = "partikkeli";

        public const int AngleRange = 5;

        public Vector Gravity = Vector.Zero;

        public Rainbowstain(Image kuva, int maxAmountOfParticles=100) : base(kuva, maxAmountOfParticles)
        {
            Angle = Angle.FromDegrees(90);
        }

        protected override void InitializeParticles()
        {
            MinLifetime = .4;
            MaxLifetime = .8;

            MinScale = 4;
            MaxScale = 12;

            MinVelocity = 100;
            MaxVelocity = 120;

            ScaleAmount = -1.0;
            AlphaAmount = 1.0;

        }



        /// <summary>
        /// Lasketaan liekin suunnalle satunnaisuutta
        /// </summary>
        /// <returns>Partikkelin suunta</returns>
        protected override Vector GiveRandomDirection()
        {
            var dir = Angle.FromDegrees(RandomGen.NextDouble(AngleRange/2*-1, AngleRange/2));
            return Vector.FromLengthAndAngle(1, Angle + dir);
        }

        protected override void InitializeParticle(Particle p, Vector position)
        {
            base.InitializeParticle(p, position);
            p.Acceleration += Gravity;
            if (p.Acceleration.X > 0)
                MirrorImage();
            


        }
    }

}

