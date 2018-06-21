using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jypeli;
using Jypeli.Effects;

using Morte.Aseet;

namespace Morte.Loot
{
    public class Saha : Härpäke, ÄLÄ_KÄÄNNÄ
    {
        const double MAX_ETÄISYYS = 70;

        const double TÄRINÄ = 0.2;
        const double TÄRINÄ_TIMEOUT = 0.05;

        const double LÖPÖÄ = 50;
        const double LÖPÖÄ_KULUU_IDLE = 1;
        const double LÖPÖÄ_KULUU_KÄYNNISSÄ = 10;

        /// <summary>
        /// Sahan liikkumisen nopeus.
        /// "Nopeus (paikkayksikköä sekunnissa) jolla liikutaan."
        /// </summary>
        const double LIIKENOPEUS = 220;

        const double MASSA_KÄYNNISSÄ = 400;
        const double MASSA_IDLE = 7;

        const int VAHINKO = 40;

        const string SFX_PALJASTUS = "groovy";
        const string SFX_POIMI = "start";
        const string SFX_IDLE = "idle";
        const string SFX_PÄRINÄ = "rev";

        static string AssetsPath = "loot/Saha/";

        protected double MaxEtäisyys = MAX_ETÄISYYS;
        protected int Suunta = Morte.OIKEA;
        
        protected Moottorisaha Ase;

        public PeliObjekti Terä;
        protected double Löpöä = LÖPÖÄ;

        private Timer Polttomoottori = new Timer();

        private Sound SfxPärinä;
        private Sound SfxIdleä;

        private bool Käynnissä;
        private int Risti_Kantama;

        public Saha() :  base(Game.LoadImage(AssetsPath + "sprite")) {
            Mass = MASSA_IDLE;

            IsUpdated = true;
            Shape = Shape.Ellipse;

            IgnoresGravity = true;

            Polttomoottori.Interval = 1;
            Polttomoottori.Timeout += KulutaLöpöä;

            SfxIdleä = Game.LoadSoundEffect(AssetsPath + SFX_IDLE).CreateSound();
            SfxIdleä.IsLooped = true;
            SfxPärinä = Game.LoadSoundEffect(AssetsPath + SFX_PÄRINÄ).CreateSound();
            SfxPärinä.IsLooped = true;
        }

        protected override void AsetaTapahtumat()
        {
            base.AsetaTapahtumat();

            OnPaljastus += () => Game.LoadSoundEffect(AssetsPath + SFX_PALJASTUS).Play();
            OnPoiminta += PoimiSaha;

            //OnPudotettaessa += PoistaAse;
        }

        public void PoimiSaha()
        {
            IgnoresGravity = true;
            AddCollisionIgnoreGroup(Sankari.IGNORE_ID);

            Type t = Type.GetType("Morte.Aseet.Moottorisaha");
            Ase = (Moottorisaha)Activator.CreateInstance(t);
            Morte.Instance.Ase.Add(Ase);

            Ase.Käynnistettäessä += Pärise;
            Ase.Pysäytettäessä += Rauhoitu;

            Morte.Instance.AddCollisionHandlerByTag<FysiikkaObjekti, Vihulainen>(this, "vihu", OsuVihuun);

            var sfx = Game.LoadSoundEffect(AssetsPath + SFX_POIMI);
            sfx.Play();

            double duration = sfx.Duration.Seconds + sfx.Duration.Milliseconds / 1000 + 0.3;
            Timer.SingleShot( duration, () => SfxIdleä.Play());

            Polttomoottori.Start();
        }

        public void PoistaAse()
        {
            Poimija = null;
            IgnoresGravity = false;
            Polttomoottori.Stop();


            Ase?.Pysäytä();
            Morte.Instance.Ase.Remove(Ase);

            SfxIdleä.Stop();
            SfxPärinä.Stop();
        }

        public void KulutaLöpöä()
        {
            if(Löpöä <= 0)
            {
                Pudota();
                return;
            }

            Löpöä -= (Käynnissä) ? LÖPÖÄ_KULUU_KÄYNNISSÄ : LÖPÖÄ_KULUU_IDLE;
        }

        public void Pudota()
        {
            if(Käynnissä) Rauhoitu();
            Debug.WriteLine("Hylätään ase");
            var f = RandomGen.NextDouble(1000, 2000) * Mass;
            var impulssi = new Vector(f * Poimija.Suunta, f);
            Poimija = null;
            PoistaAse();
            Hit(impulssi);

            CanBeDead = true;
        }

        public void OsuVihuun(FysiikkaObjekti saha, Vihulainen vihu)
        {
            if(Käynnissä)
            {

                // Yritä paikantaa sopiva veriroiskeen paikka.
                var a = Angle.FromRadians(Math.Atan2(AbsolutePosition.Y - vihu.AbsolutePosition.Y, AbsolutePosition.X - vihu.AbsolutePosition.X));

                // HACK Purkkaviritys.
                var m = (double) (vihu.Width * vihu.Height) / (Width * Height);
                var d = Vector.Distance(AbsolutePosition, vihu.AbsolutePosition) / m;

                var roiske = AbsolutePosition - new Vector(d * a.Cos, d * a.Sin);

                Morte.Instance.Veriroiske.AddEffect(roiske, VAHINKO);

                vihu.Vahingoita(VAHINKO);
            }
        }

        /// <summary>
        /// Käynnistä saha.
        /// </summary>
        public void Pärise()
        {
            Käynnissä = true;
            Mass = MASSA_KÄYNNISSÄ;

            Risti_Kantama = Morte.Instance.Kantama;
            Morte.Instance.Kantama = (int) MaxEtäisyys;

            SfxIdleä.Stop();
            SfxPärinä.Play();

            _Pärise_Tärisytä();
        }

        /// <summary>
        /// Ajoitettu toiminta sahan tärisyttämiseksi.
        /// </summary>
        private void _Pärise_Tärisytä()
        {
            if (Käynnissä == false) return;

            Tärisytä();
            Timer.SingleShot(TÄRINÄ_TIMEOUT, _Pärise_Tärisytä);
        }

        public void Rauhoitu()
        {
            Käynnissä = false;

            Morte.Instance.Kantama = Risti_Kantama;
            Mass = MASSA_IDLE;
            
            SfxPärinä.Stop();
            SfxIdleä.Play();
        }

        public void Tärisytä()
        {
            Angle += Angle.FromRadians(RandomGen.NextDouble(TÄRINÄ /2*-1, TÄRINÄ/2));
            Position += new Vector(RandomGen.NextDouble(-2, 2), RandomGen.NextDouble(-2, 2));
        }

        /// <summary>
        /// Liikuta sahaa hiiren suuntaan.
        /// </summary>
        public void Kolmiopaikanna()
        {
            var mouse = Game.Mouse.PositionOnWorld;
            var rad = Math.Atan2(mouse.Y - Poimija.AbsolutePosition.Y, mouse.X - Poimija.AbsolutePosition.X);

            var a = Angle.FromRadians(rad);

            if (Vector.Distance(mouse, Poimija.AbsolutePosition) <= MaxEtäisyys)
                MoveTo(mouse, LIIKENOPEUS); //AbsolutePosition = mouse;
            else
            {
                if (Vector.Distance(Position, Poimija.Position) > MaxEtäisyys)
                {
                    // siirrä maksimietäisyydelle, jos liian kaukana.
                    var _a = Angle.FromRadians(Math.Atan2(Y - Poimija.Y, X - Poimija.X));
                    Position = Poimija.Position + new Vector(MaxEtäisyys * _a.Cos, MaxEtäisyys * _a.Sin);
                }

                // Liikuta hiiren suuntaan.
                MoveTo(Poimija.Position + new Vector(MaxEtäisyys * a.Cos, MaxEtäisyys * a.Sin), LIIKENOPEUS);
            }

            Angle = a;

            // Käännä saha osoittamaan oikeaan suuntaan.
            var d = Angle.GetPositiveDegrees() % 360;

            if ((d <= 90 || d >= 270))
            {
                if (Suunta != Morte.OIKEA)
                {
                    FlipImage();
                    Suunta = Morte.OIKEA;
                }
            }
            else if(Suunta != Morte.VASEN)
            {
                FlipImage();
                Suunta = Morte.VASEN;
            }

            if(Käynnissä) Tärisytä();

        }

        public override void Update(Time time)
        {
            if(Poimija != null) Kolmiopaikanna();
            base.Update(time);
        }
    }

}
