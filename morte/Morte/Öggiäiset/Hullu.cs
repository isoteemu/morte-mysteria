using System;
using System.Diagnostics;
using Jypeli;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morte.Öggiäiset
{
    class Hullu : Vihulainen
    {
        /// <summary>
        /// Lista käyteyista kuvista
        /// </summary>
        public static Image[] Spritet = Game.LoadImages(new String[] { "vihulaiset/hullu0001", "vihulaiset/hullu0002", "vihulaiset/hullu0003" });

        public Hullu() : this(Spritet[0]) {}
        
        public Hullu(Image sprite) : base(sprite)
        {
            MaxHitpoints = RandomGen.NextInt(10) + 30;
            Vahinko = 50;
            PisteArvo = 3;

            Liikkumisnopeus = 200;
            Mass = 350.0;

            Animation = new Animation(Spritet);
            Animation.FPS = Animation.FPS / 3;
            Animation.Start();
        }

        public override void Update(Time time)
        {
            Liikuta();
            base.Update(time);
        }
    }
}
