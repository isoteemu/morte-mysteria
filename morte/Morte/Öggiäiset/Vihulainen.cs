using System;
using Jypeli;
using System.Diagnostics;

using Glide;

namespace Morte
{
    /// <summary>
    /// Perusluokka terroristeille.
    /// </summary>
    public abstract class Vihulainen : Öggiäinen
    {
        /// <summary>
        /// Vihulaisen hoitelusta saatavat pisteet.
        /// </summary>
        public int PisteArvo { get; set; }

        /// <summary>
        /// CollisionIgnoreGroupin ID.
        /// </summary>
        public const int IgnoreCollisionID = 2;
        public Öggiäinen Pelaaja { get; set; }

        public event Action OnTapettu;

        protected TimeSpan kuolinAika;
        protected float KuolemanKesto = 0.8f;

        protected Timer HyökkimisAjastin;

        /// <summary>
        /// Alkuperäinen frame, johon kuolintehoste lasketaan.
        /// Alkuperäinen suunnitelma oli käyttää vain laskentaa, mutta liian pienet askeleet sotkivat sen,
        /// koska värit tallennetaan kokonaislukuina, eikä murtolukuina. Joten tällainen muistihukka.
        /// </summary>
        protected Color[,] _AlkuperäinenFrame;

        /// <summary>
        /// Kuinka monta askelmaa tavussa on.
        /// </summary>
        const float väriaskel = 1.0f / byte.MaxValue;

        /// <summary>
        /// Määrittelee kuvan siirtymän valkoiseen.
        /// </summary>
        public double Vaaleus
        {
            get => _Vaaleus;
            set
            {
                value = Math.Min((double)1, (double)value);

                // Jos väri ei ole muuttunut, ohita värimuutokset.
                if (Math.Floor(_Vaaleus / väriaskel) == Math.Floor(value / väriaskel))
                {
                    _Vaaleus = value;
                    return;
                }

                if (_AlkuperäinenFrame == null)
                {
                    if (Animation != null)
                        _AlkuperäinenFrame = Animation.CurrentFrame.GetData();
                    else
                        _AlkuperäinenFrame = Image.GetData();
                }

                // Luodaan uusi työskentely-array.
                Color[,] varidata = new Color[_AlkuperäinenFrame.Length / Image.Width, Image.Width];
                Array.Copy(_AlkuperäinenFrame, varidata, _AlkuperäinenFrame.Length);

                int r = 0;
                int c = 0;

                double punainen = byte.MinValue;
                double vihreä = byte.MinValue;
                double sininen = byte.MinValue;

                // Shortcut
                Color _väri;

                for (int i = 0; i < varidata.Length; i++)
                {
                    r = i / Image.Width;
                    c = i % Image.Width;

                    _väri = _AlkuperäinenFrame[r, c];

                    // Jostain syystä AlphaComponent ei säily, joten skipataan läpinäkyvät pikselit :(
                    if ((byte)_väri.AlphaComponent == byte.MinValue) continue;

                    if (_Vaaleus >= 1)
                    {
                        punainen = vihreä = sininen = byte.MaxValue;
                    }
                    else if (_Vaaleus <= 0)
                    {
                        punainen = _väri.RedComponent;
                        vihreä = _väri.GreenComponent;
                        sininen = _väri.BlueComponent;
                    }
                    else
                    {
                        punainen = _väri.RedComponent + (byte.MaxValue - _väri.RedComponent) * value;
                        vihreä = _väri.GreenComponent + (byte.MaxValue - _väri.GreenComponent) * value;
                        sininen = _väri.BlueComponent + (byte.MaxValue - _väri.BlueComponent) * value;
                    }

                    varidata[r, c] = new Color(
                        (byte)punainen, (byte)vihreä, (byte)sininen,
                        _väri.AlphaComponent
                    );
                }
                /*
                if (Animation != null)
                    Animation.CurrentFrame.SetData(varidata);
                    */
                Image = new Image(Image.Width, Image.Height, Color.Transparent);
                Image.SetData(varidata);

                _Vaaleus = value;
            }
        }

        private double _Vaaleus = 0.0;

        private static Image[] Spritet;

        public Vihulainen() : this(Spritet[0]) { }

        public Vihulainen(Image sprite) : base(sprite)
        {
            Pelaaja = Pelaaja;
            CollisionIgnoreGroup = IgnoreCollisionID;
            IsUpdated = true;
            Image = sprite;

            Y = Game.Level.Bottom - Bottom;

            OnKuolema += LopetaHyökkiminen;
        }


        public virtual void HyökiLoop()
        {
            if (Hitpoints <= 0) return;

            Suunta = (Pelaaja.X < X) ? Morte.VASEN : Morte.OIKEA;

        }

        public virtual void Hyöki()
        {
            HyökkimisAjastin = new Timer();
            HyökkimisAjastin.Interval = 1;
            HyökkimisAjastin.Timeout += HyökiLoop;
            HyökkimisAjastin.Start();
        }

        public void LopetaHyökkiminen()
        {
            HyökkimisAjastin.Stop();
        }

        /// <summary>
        /// Tapahtuma kun vihu osuu sankariin.
        /// </summary>
        /// <param name="Sankari"></param>
        public virtual void VahingoitaSankaria(Öggiäinen sankari)
        {
            Debug.WriteLine("Vahingoitetaan sankaria hp " + Vahinko);
            sankari.Vahingoita(Vahinko);

        }

        public override void Vahingoita(int vahinko = 1)
        {
            base.Vahingoita(vahinko);
            if (Hitpoints <= 0 && !Kuollut)
            {
                OnTapettu?.Invoke();
                Kuole();
            }
        }

        /// <summary>
        /// Tuhoaa olion, kutsumalla sen taivaaseen.
        /// </summary>
        public override void Kuole()
        {
            base.Kuole();

            IgnoresExplosions = true;
            IgnoresGravity = true;
            IgnoresCollisionResponse = true;
            IgnoresPhysicsLogics = true;

            Animation.Pause();
            //Image = Animation.CurrentFrame;

            Velocity = Vector.Zero;

            Debug.WriteLine(this.GetType().Name + " Kuolema!");

            kuolinAika = TimeSpan.Zero;

            KuolinAnimaatio();
        }

        public virtual void KuolinAnimaatio() {            
            //  Y = Game.Screen.Top - this.Bottom 
            var _Y = Game.Screen.Top + Height;
            var _X = X + RandomGen.NextDouble(-40, 40);

            Tweetteri.Tween(this, new { Width = 10, Height = Height * 2, Y = _Y, Vaaleus = 1.0, X = _X }, KuolemanKesto)
                .Ease(Ease.QuadIn).OnComplete(ReallyKuole);
        }
        
        protected void ReallyKuole()
        {
            Kuollut = true;
            Debug.WriteLine(GetType().Name + " kuolee todella.");
            Destroy();
        }

        public override void Update(Time time)
        {
            if (Top < Game.Level.Bottom)
            {
                Debug.WriteLine(GetType().Name + " on alle kentän pohjan. Poistetaan.");
                ReallyKuole();
            }

            if (! Kuollut)
            {
                Oikaise();
                // Tarkista ettei olla yli kentän rajojen.
                if (Game.Level.Left - Width * 2 > this.X || Game.Level.Right + Width * 2 < this.X) {
                    Debug.WriteLine("Vihu rajojen ulkopuolella!");
                    ReallyKuole();
                }
            }

            base.Update(time);
        }

    }
}
