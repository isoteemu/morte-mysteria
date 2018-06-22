using System;
using System.Diagnostics;
using Jypeli;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glide;

namespace Morte
{

    public class Sankari : Öggiäinen
    {
        /// <summary>
        /// Collision Ignore ID.
        /// </summary>
        public const int IGNORE_ID = 1;
        public const int MAX_HITPOINTS = 666;

        public Vector Sijainti_Suu { get; set; }
        
        public Sankari(Image sprite) : base(sprite) {

            LataaOletukset();
        }

        public virtual void LataaOletukset()
        {

            Suunta = Morte.VASEN;
            Shape = Shape.Hexagon;

            Kuollut = false;
            MaxHitpoints = MAX_HITPOINTS;
            Hitpoints = MaxHitpoints;

            Angle = Angle.Zero;
            X = 0;

            IgnoresPhysicsLogics = false;
            IgnoresCollisionResponse = false;
            Restitution = 0.2;
            AngularDamping = 0.98;

            IgnoresExplosions = true;

            AddCollisionIgnoreGroup(IGNORE_ID);
        }

        public override void Vahingoita(int vahinko = 1)
        {
            base.Vahingoita(vahinko);
            if (Hitpoints <= 0 && ! Kuollut)
                Kuole();
        }

        public override void Liikuta()
        {
            Oikaise();
            base.Liikuta();
        }


        public override void Update(Time time)
        {
            if (Top < Game.Level.Bottom)
            {
                // Ei pitäisi päästä tapahtumaan, mutta ei sitä Jypelistä koskaan tiedä.
                Debug.WriteLine(GetType().Name + " on alle kentän pohjan. Poistetaan.");
                Kuole();
            }

            if (Game.Level.Left > Left || Game.Level.Right < Right)
            {
                Debug.WriteLine("Siirretään sankari kentän rajojen sisään.");
                X = Math.Max(Game.Level.Left + Width / 2, AbsolutePosition.X);
                X = Math.Min(Game.Level.Right - Width / 2, AbsolutePosition.X);
            }
            base.Update(time);
        }
    }
}
