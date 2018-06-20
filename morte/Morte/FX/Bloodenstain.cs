using System;
using Jypeli;
using Jypeli.Effects;
using Microsoft.Xna.Framework;

namespace Morte.FX { 

    public class Bloodenstain : ExplosionSystem
    {
        public Vector Gravity = Vector.Zero;
        
        public Bloodenstain(Image kuva, int maxAmountOfParticles=100) : base(kuva, maxAmountOfParticles)
        {
        }

        protected override void InitializeParticles()
        {
            MinLifetime = .4;
            MaxLifetime = .8;

            MinScale = 4;
            MaxScale = 12;

            MinVelocity = 20;
            MaxVelocity = 200;

            ScaleAmount = -1.0;
            AlphaAmount = 1.0;

        }


        protected override void InitializeParticle(Particle p, Vector position)
        {
            base.InitializeParticle(p, position);
            p.Acceleration += Gravity;
            if (p.Acceleration.X > 0)
                MirrorImage();

            var minä = this;
        }
    }

}

