using System;
using System.Diagnostics;
using Jypeli;
using Morte;

namespace Morte.Öggiäiset
{

    public class Käärme : Vihulainen
    {
        public static Image[] Spritet = Game.LoadImages(new String[] { "vihulaiset/mato0001", "vihulaiset/mato0002" });

        public Käärme() : this(Spritet[0]) { }

        public Käärme(Image sprite) : base(sprite)
        {
            MaxHitpoints = RandomGen.NextInt(4) + 2;
            PisteArvo = 1;
            Vahinko = 25;

            Liikkumisnopeus = 350;
            Mass = 40.0;

            Debug.WriteLine("Vihulainen: " + X);
            
            Animation = new Animation(Spritet);
            Animation.FPS = Animation.FPS / 3;
            Animation.Start();
        }

        public override void HyökiLoop()
        {
            base.HyökiLoop();
            Liikuta();
        }

        public override void Liikuta()
        {
            double v = Liikkumisnopeus * Force * Suunta;
            v *= 1 - Math.Abs(AbsoluteAngle.Radians) / Math.PI;

            Push(new Vector(v, 0));

            Debug.WriteLine(GetType().Name + " -> Liikuta()");
        }

        public override void Update(Time time)
        {
            base.Update(time);
        }

    }

}
