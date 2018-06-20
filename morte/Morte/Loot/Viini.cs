using System;
using System.Diagnostics;
using Jypeli;
using Jypeli.Controls;
using Jypeli.Physics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glide;

namespace Morte.Loot
{
    public class Viini : PudotettavaHärpäke, ÄLÄ_KÄÄNNÄ
    {
        public static string AssetsPath = "loot/Viini/";
        protected bool Käytetty = false;

        /// <summary>
        /// Missä kohden ehkä suu on.
        /// TODO: Siirrä tällaiset pisteet hahmoon.
        /// </summary>

        const double SUU_X = 20;
        const double SUU_Y = 62;

        /// <summary>
        /// Kuinka kauan kestää joutua maan alle
        /// </summary>
        const float RESPAWN_KESTO = 2.6f;

        public Viini() : base(Game.LoadImage(AssetsPath + "sprite"))
        {
            OnPoiminta += OtaKäteen;
            OnKlikattattaessa += Juo;

            Mass = 1;
        }

        protected void Juo()
        {
            if (Käytetty || Parent == null) return;
            Käytetty = true;

            var impulssi = new Vector(RandomGen.NextDouble(20, 50) * Poimija.Suunta, RandomGen.NextDouble(20, 50));

            Tweetteri.Tween(this, new {
                X = SUU_X * Poimija.Suunta,
                Y = SUU_Y },
            0.4f).OnComplete(delegate () {
                // Anglea ei voi tweenata
                Angle = Angle.FromDegrees(127 * Poimija.Suunta);
                Game.LoadSoundEffect(AssetsPath + "sound").Play();
            });


            // Heitä pullo veks.
            Timer.SingleShot(1.4, () => {
                Mass = 0.2;
                this.Hit(impulssi);
                AngularVelocity = 1*Poimija.Suunta;
            });

            // Hahmo "kuolee"
            Timer.SingleShot(2, () =>
            {
                Poimija.AngularVelocity = RandomGen.NextDouble(-0.7, 0.7);
                Poimija.IgnoresPhysicsLogics = true;
                //Poimija.IgnoresCollisionResponse = true;

                Morte.Instance.Kamera.Kohdista(new Vector(Poimija.X, Morte.Instance.Level.Bottom), RESPAWN_KESTO / 1.2f);

                var hautakivi = new Hautakivi();
                hautakivi.X = Poimija.AbsolutePosition.X;
                Game.Add(hautakivi, Morte.TASO_PAPPI);

                hautakivi.Nouse();

                Tweetteri.Tween(Poimija, new { Y = Game.Level.Bottom - Poimija.Height/2, X = Poimija.X }, RESPAWN_KESTO).Ease(Ease.QuadIn).OnComplete(delegate () {
                    Poimija.X = RandomGen.NextDouble(Game.Level.Left - Poimija.Left, Game.Level.Right - Poimija.Right);
                    Debug.WriteLine("Pappi sijainnissa " + Poimija.X);
                    Morte.Instance.ResetoiPelaaja();
                    Morte.Instance.Kamera.Palauta();

                    Poimija.Shape = Shape.Circle;

                });
                
            });
        }

        public void OtaKäteen()
        {
            IgnoresGravity = true;
            X = 4 * Poimija.Suunta;
            Y = -15;
        }

    }

    public class Hautakivi : PeliObjekti
    {
        protected Color TekstinVäri = new Color( 205, 27, 28 );

        public Hautakivi() : base(Game.LoadImage(Viini.AssetsPath + "/hautakivi")) {
            Y = Game.Level.Bottom - Height / 2;
        }
        
        public void Nouse()
        {
            IsUpdated = true;
            Debug.WriteLine("Hautakivi nousee "+X+","+Y);
            var _Width = Width;
            var _Height = Height;

            Width = Width * 0.8;
            Height = Height * 0.8;

            //Image.DrawTextOnImage(Image, string.Format("Manasi {0}", Morte.Instance.Manattu), Font.DefaultSmall, TekstinVäri);

            Tweetteri.Tween(this, new { Y = Game.Level.Bottom + _Height / 2, Width = _Width, Height = _Height }, 0.8f);
        }
    }
}
