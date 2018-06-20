using System;
using System.Diagnostics;
using Jypeli;

using WForms = System.Windows.Forms;

namespace Morte.Wädgetti
{
    class Kursori : PeliObjekti
    {
        private Image[] Kuvat = new Image[19];
        private GameObject oor_kuvake;

        /// <summary>
        /// Onko Hiiri kantaman ulottamattomissa. Jos on, asettaa kuvakkeen.
        /// </summary>
        public bool OOR
        {
            get
            {
                return _oor;
            }
            set
            {
                if (_oor != value)
                {
                    if (value == false)
                        Animation.Start();
                    else
                        Animation.Stop();
                }
                _oor = value;
            }
        }

        public new bool IsVisible
        {
            get => base.IsVisible; set
            {
                if (value == true) ArtsyFartsyKursori();
                else NormaaliKursori();
                base.IsVisible = value;
            }
        }

        /// <summary>
        /// Sisäinen muuttuja pitämään kirjaa onko normaali kursori näkyvissä vai piilotettu. Cursor.Show()/Hide() stacks up Jerry!
        /// </summary>
        protected static bool _NormaaliKursori = true;

        /// <summary>
        /// Sisäinen toggle muuttuja kuvakkeelle.
        /// </summary>
        protected bool _oor;

        public Vector Sijainti
        {
            get { return this.Position; }
            set
            {
                value.X = value.X + this.Width / 2 - 3;
                value.Y = value.Y - this.Height / 2 + 3;
                this.AbsolutePosition = value;
            }
        }

        public Kursori() : base(38, 38)
        {
            for (int i = 1; i <= Kuvat.Length; i++)
            {
                Kuvat[i - 1] = Game.LoadImage(String.Format("kursori/kursori{0:D4}", i));
            }
            Animation = new Animation(Kuvat);
            Animation.Start();

            oor_kuvake = new PeliObjekti(32, 32, Shape.Circle)
            {
                X = Left + 3,
                Y = Top - 3,
                Image = Game.LoadImage("kursori/unable")
            };

            Add(oor_kuvake);

            Game.AssertInitialized(() => { IsVisible = true; });
        }

        protected void NormaaliKursori()
        {
            if (!_NormaaliKursori)
            {
                Debug.WriteLine("Vaihdetaan normaali kursori.");
                WForms.Cursor.Show();
                _NormaaliKursori = true;
            }

            foreach (GameObject lapsi in Objects)
            {
                lapsi.IsVisible = false;
            }
        }

        protected void ArtsyFartsyKursori()
        {
            if (_NormaaliKursori)
            {
                Debug.WriteLine("Piilotetaan normaali kursori");
                WForms.Cursor.Hide();
                _NormaaliKursori = false;
            }

            foreach (GameObject lapsi in Objects)
            {
                lapsi.IsVisible = true;
            }
        }

        public override void Update(Time time)
        {
            Sijainti = Game.Mouse.PositionOnWorld;

            if (IsVisible && _oor)
                oor_kuvake.IsVisible = true;
            else
                oor_kuvake.IsVisible = false;


            base.Update(time);
        }

    }

}
