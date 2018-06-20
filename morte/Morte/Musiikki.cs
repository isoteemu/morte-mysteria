using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using Jypeli;

namespace Morte
{
    /// <summary>
    /// Tekee taustamusiikit peliin.
    /// Vertailee pelissä olevia Öggiäisiä <c>Äänitehosteet</c> -listaan, ja soittaa
    /// sopivat loopit.
    /// </summary>
    public class Musiikki
    {

        protected SoundEffect TaustaMusiikki { get; set; }
        
        public Dictionary<string, SoundEffect> Äänitehosteet { get; set; }
        private Timer Luupperi { get; set; }
        
        public Musiikki()
        {
            TaustaMusiikki = Game.LoadSoundEffect("music/bass");

            Äänitehosteet = new Dictionary<string, SoundEffect>
            {
                { "Käärme", Game.LoadSoundEffect("music/Käärme") },
                { "Hullu", Game.LoadSoundEffect("music/Hullu") },
                { "Lokki", Game.LoadSoundEffect("music/Lokki") },
                { "Koura", Game.LoadSoundEffect("music/Koura") },
            };

            Luupperi = new Timer();
            Luupperi.Interval = TaustaMusiikki.Duration.Seconds + (TaustaMusiikki.Duration.Milliseconds / 1000);
            Luupperi.Timeout += ToistaAmbient;

        }

        public void Käynnistä()
        {
            Luupperi.Start();
            ToistaAmbient();
        }

        public void Pysäytä()
        {
            foreach(SoundEffect s in Äänitehosteet.Values)
            {
                s.Stop();
            }
            Luupperi.Stop();
        }

        /// <summary>
        /// Etsi sopivat vihut, ja toista sopivat musiikit.
        /// </summary>
        protected void ToistaAmbient()
        {
            TaustaMusiikki.Play();

            string luokka;
            
            var nähty = new List<string>();

            bool tunnettu(IGameObject o) { return Äänitehosteet.ContainsKey(o.GetType().Name); }
            foreach (IGameObject o in Game.Instance.GetObjects(tunnettu)) {
                luokka = o.GetType().Name;

                if (!nähty.Contains(luokka))
                {
                    Äänitehosteet[luokka].Play();
                    nähty.Add(luokka);
                }

            }
        }

    }
}
