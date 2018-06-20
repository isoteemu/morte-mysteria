using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jypeli;
using Glide;

namespace Morte
{
    /// <summary>
    /// siirtää kameraa tahmaisesti hiiren liikkuessa.
    /// Kameran seurantaa asetetaan IsUpdated arvolla.
    /// </summary>
    public class Kamerointi : Vitkutin
    {
        const float NOPEA = 0.4f;
        const float NORMA = 0.8f;
        const float HIDAS = 1.2f;

        public bool Seuraahiirtä = true;

        public Kamerointi() : base(0, 0)
        {

        }

        /// <summary>
        /// Kohdistaa kameran pisteeseen.
        /// </summary>
        /// <param name="kohde"></param>
        /// <param name="kesto"></param>
        /// <param name="zoom"></param>
        public void Kohdista(Vector kohde, float kesto=Kamerointi.NORMA, double zoom=2)
        {
            Seuraahiirtä = false;

            double kuvan_reuna_w = Game.Screen.Width / 2 / zoom;
            double kuvan_reuna_h = Game.Screen.Height / 2 / zoom;

            double _x = kohde.X;
            double _y = kohde.Y;

            _x = Math.Min(Game.Level.Right - kuvan_reuna_w, _x);
            _x = Math.Max(Game.Level.Left + kuvan_reuna_w, _x);
            
            _y = Math.Min(Game.Level.Top - kuvan_reuna_h, _y);
            _y = Math.Max(Game.Level.Bottom + kuvan_reuna_h, _y);

            Tweetteri.Tween(Game.Camera, new { X = _x, Y = _y, ZoomFactor=zoom }, kesto).Ease(Ease.QuadIn);
        }

        public void Palauta(float kesto=Kamerointi.NOPEA)
        {
            var X = Game.Camera.X + LaskeHiirenOffset();
            Tweetteri.Tween<Camera>(Game.Camera, new { ZoomFactor = 1, Y = 0, X = X }, kesto).Ease(Ease.QuadOut)
                .OnComplete(() => Seuraahiirtä = true);
        }

        protected double LaskeHiirenOffset()
        {
            double diff = 0;
            double hiiri_rel = Game.Mouse.PositionOnScreen.X / Game.Screen.Right;

            // Normalisoi, ettei lasketa kuin ikkunan näkyvän koon mukaan
            hiiri_rel = Math.Max(-1, hiiri_rel);
            hiiri_rel = Math.Min(1, hiiri_rel);

            double kenttä_width = Game.Level.Right - Game.Screen.Right;

            diff = (kenttä_width * hiiri_rel) - Game.Camera.X;

            return diff;
        }
        
        public override void Update(Time time)
        {
            float since = (float)time.SinceLastUpdate.Seconds + ((float)time.SinceLastUpdate.Milliseconds / 1000);

            if (Seuraahiirtä && ! Game.Instance.IsPaused)
            {
                var diff = LaskeHiirenOffset();
                float fps;

                if (time.SinceLastUpdate.Milliseconds == 0)
                    fps = 1;
                else
                    fps = 1 / ((float)time.SinceLastUpdate.Milliseconds / 1000);

                Game.Camera.X = Game.Camera.X + (diff / fps);
            }

            base.Update(time);
        }
    }

}
