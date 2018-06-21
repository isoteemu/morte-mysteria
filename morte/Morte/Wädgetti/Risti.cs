using System;
using System.Diagnostics;
using Jypeli;

using Glide;

namespace Morte.Wädgetti
{
    /// <summary>
    /// Hitpointti-osoitin ristin muodossa.
    /// Maksimihelat == kristitty.
    /// Nollahelat == larppaaja.
    /// </summary>
    public class Risti : Vitkutin
    {
        /// <summary>
        /// Hahmo johon risti on sidottu.
        /// </summary>
        public Öggiäinen Sidos { get => _Sidos;
            set {
                _Sidos = value;
                _Sidos.OnVahingoita += this.Päivitä;
            }
        }

        protected Öggiäinen _Sidos;
        
        /// <summary>
        /// Ristin vaakapalkki.
        /// </summary>
        GameObject Palkki;

        /// <summary>
        /// Äänitehoste healtin laskiessa.
        /// </summary>
        protected Sound Sfx;

        /// <summary>
        /// Healtin suhteellinen osuus jolloin <c>Sfx</c> triggeröidään.
        /// </summary>
        protected const double SFX_TRIGGER = 1.0 / 3;

        const double PALKKI_SKAALA = 3.5;

        public Risti() : base(Game.LoadImage("risti-pysty"))
        {
            Sfx = Game.LoadSoundEffect("syke").CreateSound();
            Sfx.IsLooped = true;
            
            Palkki = new PeliObjekti(Game.LoadImage("risti-vaaka"))
            {
                Y = Height / 2 - (Height / PALKKI_SKAALA)
            };

            IsUpdated = true;

            Add(Palkki);

            // Sävytä kun lapsiobjektikin (Palkki) on lisätty peliin
            //Palkki.AddedToGame += () => Sävytä(Color.Red, true);
        }

        /// <summary>
        /// Päivitä ristiä, ja lisää syke-tehoste jos healtit alkavat käydä vähiin.
        /// </summary>
        public void Päivitä()
        {
            // Laske helan skaala ja normalisoi
            double helaa = (double) Sidos.Hitpoints / Sidos.MaxHitpoints;
            helaa = Math.Max(0, helaa);
            helaa = Math.Min(1, helaa);


            double askel = Height / PALKKI_SKAALA;
            double skaala = askel * (PALKKI_SKAALA - 2);

            Tweetteri.Tween(Palkki, new { Y = (helaa - 0.5) * skaala }, 0.5f);

            // Sävytä huen asteikolla 240-360
            // TODO: Siirrä shaderiksi.
            //Sävytä(360 - 120 * helaa, true);
            
            if (helaa < SFX_TRIGGER)
            {
                Sfx.Play();

                float vol = 1.0f - (Sidos.Hitpoints /  ((float) Sidos.MaxHitpoints * (float) SFX_TRIGGER));

                /// Volume ei toimi lineaarisesti, vaan logaritmisesti. Käytetään <c>Glide.Easing</c>ia siihen.
                vol = Ease.ExpoIn(vol);
                vol = Math.Min(vol, 1.0f);

                Debug.WriteLine("Toistetaan äänitehoste vol" + vol);

                Tweetteri.Tween(Sfx, new { Volume = vol }, 0.7f);
            } else
            {
                Sfx.Volume = 0;
                Sfx.Stop();
            }
            
        }

    }
}
