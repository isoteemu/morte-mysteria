using System;
using Glide;
using Jy = Jypeli;
using JyColor = Jypeli.Color;
using Animation = Jypeli.Animation;
using SDColor = System.Drawing.Color;

using System.Diagnostics;

namespace Morte
{
    public interface ITweetteri
    {
        Tweener Tweetteri { get; }

        void Update(Jy.Time time);
    }

    public partial class PeliObjekti : Jy.GameObject, ITweetteri
    {

        public Tweener Tweetteri { get => _Tweetteri; }
        private Tweener _Tweetteri = new Tweener();

        #region Constructors
        public PeliObjekti(Animation animation) : base(animation)
        {
        }

        public PeliObjekti(Jy.ILayout layout) : base(layout)
        {
        }

        public PeliObjekti(double width, double height) : base(width, height)
        {
        }

        public PeliObjekti(double width, double height, Jy.Shape shape) : base(width, height, shape)
        {
        }

        public PeliObjekti(double width, double height, double x, double y) : base(width, height, x, y)
        {
        }

        public PeliObjekti(double width, double height, Jy.Shape shape, double x, double y) : base(width, height, shape, x, y)
        {
        }
        #endregion

        /// <summary>
        /// Sävytä objekti värillä.
        /// </summary>
        /// <param name="väri">Väri jolla sävytetään</param>
        /// <param name="recursive">Sävytä lapsiobjektit</param>
        public void Sävytä(JyColor väri, bool recursive = true)
        {
            double hue = (double) SDColor.FromArgb(väri.AlphaComponent, väri.RedComponent, väri.GreenComponent, väri.BlueComponent).GetHue();
            Sävytä(hue, recursive);
        }

        public void Sävytä(double hue, bool recursive=true) { 

            Converter<JyColor, JyColor> op = delegate (JyColor c)
            {
                double v = (double)Math.Max(c.RedComponent, Math.Max(c.GreenComponent, c.BlueComponent)) / byte.MaxValue;

                JyColor nc = ColorUtils.HsvToRgb(hue, 1d, v);
                nc.AlphaComponent = c.AlphaComponent;
                return nc;
            };

            Image.ApplyPixelOperation(op);

            if(recursive)
            {
                Debug.WriteLine("Sävytetään no " + Objects.Count + " lasta");
                for(int i = 0; i < ObjectCount; i++)
                {
                    ((PeliObjekti)Objects[i]).Sävytä(hue, recursive);
                }
            }
        }




        public override void Update(Jy.Time time)
        {
            float since = (float)time.SinceLastUpdate.Seconds + ((float)time.SinceLastUpdate.Milliseconds / 1000);
            Tweetteri.Update(since);

            base.Update(time);
        }
    }

    public class FysiikkaObjekti : Jy.PhysicsObject, ITweetteri
    {

        public Tweener Tweetteri { get => _Tweetteri; }
        private Tweener _Tweetteri = new Tweener();

        #region Constructors
        public FysiikkaObjekti(Animation animation) : base(animation)
        {
        }
        
        public FysiikkaObjekti(Jy.RaySegment raySegment) : base(raySegment)
        {
        }

        public FysiikkaObjekti(double width, double height) : base(width, height)
        {
        }

        public FysiikkaObjekti(double width, double height, double x, double y) : base(width, height, x, y)
        {
        }

        public FysiikkaObjekti(double width, double height, Jy.Shape shape, double x = 0, double y = 0) : base(width, height, shape, x, y)
        {
        }
        #endregion


        /// <summary>
        /// Sävytä objekti värillä.
        /// </summary>
        /// <param name="väri">Väri jolla sävytetään</param>
        /// <param name="recursive">Sävytä lapsiobjektit</param>
        public void Sävytä(JyColor väri, bool recursive = true)
        {
            double hue = (double)SDColor.FromArgb(väri.AlphaComponent, väri.RedComponent, väri.GreenComponent, väri.BlueComponent).GetHue();
            Sävytä(hue, recursive);
        }

        public void Sävytä(double hue, bool recursive = true)
        {

            Converter<JyColor, JyColor> op = delegate (JyColor c)
            {
                double v = (double)Math.Max(c.RedComponent, Math.Max(c.GreenComponent, c.BlueComponent)) / byte.MaxValue;

                JyColor nc = ColorUtils.HsvToRgb(hue, 1d, v);
                nc.AlphaComponent = c.AlphaComponent;
                return nc;
            };

            Image.ApplyPixelOperation(op);

            if (recursive)
            {
                Debug.WriteLine("Sävytetään no " + Objects.Count + " lasta");
                for (int i = 0; i < ObjectCount; i++)
                {
                    ((PeliObjekti)Objects[i]).Sävytä(hue, recursive);
                }
            }
        }


        public override void Update(Jy.Time time)
        {
            float since = (float)time.SinceLastUpdate.Seconds + ((float)time.SinceLastUpdate.Milliseconds / 1000);
            Tweetteri.Update(since);

            base.Update(time);
        }
    }

    public partial class Vitkutin : Jy.Widget, ITweetteri
    {
        public Tweener Tweetteri { get => _Tweetteri; }
        private Tweener _Tweetteri = new Tweener();

        #region Constructors
        public Vitkutin(Animation animation) : base(animation)
        {
        }

        public Vitkutin(Jy.ILayout layout) : base(layout)
        {
        }

        public Vitkutin(double width, double height) : base(width, height)
        {
        }

        public Vitkutin(double width, double height, Jy.Shape shape) : base(width, height, shape)
        {
        }
        #endregion

        public override void Update(Jy.Time time)
        {
            float since = (float)time.SinceLastUpdate.Seconds + ((float)time.SinceLastUpdate.Milliseconds / 1000);
            Tweetteri.Update(since);

            base.Update(time);
        }

    }

    /// <summary>
    /// Käännettäessä objektin suuntaa, älä käännä tätä.
    /// </summary>
    interface ÄLÄ_KÄÄNNÄ { }
}
