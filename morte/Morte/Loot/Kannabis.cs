using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jypeli;

using Morte.Aseet;


namespace Morte.Loot
{
    class Kannabis : PudotettavaHärpäke
    {
        const string SPRITE = "sprite";
        const string JOINTTI = "joint";
        const string SFX_INHALE = "Toke_and_Exhale-Jaime_Os_Rivera-930234911";

        const double KESTO = 20d;
        const string ASEEN_LUOKKA = "Morte.Aseet.Oksennus";

        public static string AssetsPath = "loot/Kannabis/";

        protected bool Käytetty;
        protected IAse Ase;


        public Kannabis() : base(Game.LoadImage(AssetsPath + SPRITE))
        {
            OnPoiminta += PoltaJointti;
            OnKlikattattaessa += Tumppaa;
            OnPudotettaessa += PoistaAse;

            Mass = 0.2;
        }

        public void PoltaJointti()
        {
            Game.LoadSoundEffect(AssetsPath + SFX_INHALE).Play();
            Timer.SingleShot(KESTO, () => Tumppaa());

            Type t = Type.GetType(ASEEN_LUOKKA);
            Ase = (IAse) Activator.CreateInstance(t);
            Morte.Instance.Ase.Add(Ase);

            var img = Game.LoadImage(AssetsPath + JOINTTI);

            if (Poimija.Suunta == Morte.VASEN)
                img = Image.Mirror(img);

            Image = img;
            Width = Image.Width;
            Height = Image.Height;

            Position = Poimija.Sijainti_Suu - new Vector((-Width/2)*Poimija.Suunta, 0);

        }

        public void PoistaAse()
        {
            Ase?.Pysäytä();
            Morte.Instance.Ase.Remove(Ase);
        }

        public void Tumppaa()
        {
            if (Käytetty || Parent == null) return;
            Käytetty = true;

            var impulssi = new Vector(RandomGen.NextDouble(20, 50) * Poimija.Suunta, RandomGen.NextDouble(20, 50));
            this.Hit(impulssi);
            AngularVelocity = 1 * Poimija.Suunta;
            
        }

    }
}
