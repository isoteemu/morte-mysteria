using Jypeli;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using Glide;

namespace Morte
{
    public abstract class Öggiäinen : FysiikkaObjekti
    {
        /// <summary>
        /// Hahmon maksimihitpointit.
        /// </summary>
        public virtual int MaxHitpoints { get; set; }

        public virtual int Vahinko { get; set; }

        /// <summary>
        /// Hahmon maksimiliikkumisnopeus.
        /// </summary>
        public int Liikkumisnopeus = 200;

        /// <summary>
        /// Määrittelee kivan luvun liikkumiseen. 
        /// Sen laskuun sovelletaan mBMI:tä, ja kerrotaan se kivaksi.
        /// </summary>
        public double Force { get {
                if(_Force == 0)
                    _Force = Math.Pow(Math.Sqrt(Width * Height), 2.3) / (1.33 * Mass) * 2;
                return _Force;
            } set => _Force = value; }

        protected double _Force;

        public const int KULMA_POW = 7;

        /// <summary>
        /// Hahmon jäljellä olevat hitpointit.
        /// </summary>
        public int Hitpoints
        {
            get
            {
                if (_Hitpoints == null)
                    _Hitpoints = MaxHitpoints;
                return (int)_Hitpoints;
            }
            set { _Hitpoints = value; }
        }

        public event Action OnVahingoita;
        public event Action OnKuolema;

        public bool Kuollut;

        protected int? _Hitpoints;

        public static Dictionary<string,Shape> MuotoCache = new Dictionary<string, Shape>();

        private static Image[] Spritet;

        public int Suunta
        {
            get => _Suunta; set
            {
                if (value != _Suunta)
                {
                    for (var i = 0; i < this.ObjectCount; i++)
                    {

                        this.Objects[i].X *= -1;
                        if (Objects[i] is ÄLÄ_KÄÄNNÄ)
                            continue;

                        this.Objects[i].MirrorImage();
                    }

                    MirrorImage();
                }

                _Suunta = value;

                /*
                // Suunnan pitää olla asetettu tätä tehdessä.
                if (Shape != null && IsAddedToGame) {
                    Shape = LuoMuoto();
                }
                */
            }
        }

        protected int _Suunta = Morte.VASEN;


        /// <summary>
        /// Määrittelee kuinka peitossa piksenlin on oltava, että sitä pidetään oikeana muotona.
        /// </summary>
        [Obsolete("LuoMuoto() käyttää, mutta LuoMuotoa ei itsessään käytetä")]
        protected int Muodon_toleranssi = byte.MaxValue / 2;

        public Öggiäinen() : this(Spritet[0]) { }

        public Öggiäinen(Image sprite) : base(sprite)
        {
            MaxHitpoints = 1;
            Vahinko = 1;
            Shape = Shape.Circle;

            //CanRotate = false;
            /*
            AddedToGame += delegate() {
                Shape = LuoMuoto();
            };
            */
        }

        public virtual void Vahingoita(int vahinko = 1)
        {
            Hitpoints -= vahinko;
            OnVahingoita?.Invoke();
        }

        /// <summary>
        /// Merkitse <c>Öggiäinen</c> kuolleeksi.
        /// </summary>
        public virtual void Kuole()
        {
            if (Kuollut) return;
            Kuollut = true;

            OnKuolema?.Invoke();
        }

        public virtual void Liikuta()
        {
            if (Math.Abs(Velocity.X) < Liikkumisnopeus)
            {
                var v = Liikkumisnopeus * Force * Suunta;
                v *= 1 - Math.Abs(AbsoluteAngle.Radians) / Math.PI;

                Push(new Vector(v, 0));
            }
        }

        /// <summary>
        /// Suorista hahmoa jos ei ole pystyssä.
        /// </summary>
        public virtual void Oikaise()
        {
            if (Angle != Angle.Zero)
            {
                //AngularAcceleration = 0;

                if (AbsoluteAngle.Radians < 0.05 && AbsoluteAngle.Radians > -0.05 &&
                    AngularVelocity < 0.5 && AngularVelocity > -0.5 )
                {
                    // Riittävän suorassa, pakotetaan pystyyn.
                    AbsoluteAngle = Angle.Zero;
                    return;
                }
                
                double a = Math.Abs(AbsoluteAngle.Radians) / Math.PI;

                a = (double) Ease.QuadInOut((float)a - 0.1f) + 0.1; 
                
                double t = a * KULMA_POW;

                if (AbsoluteAngle.Radians > 0)
                    t = Math.Max(t * -1, AngularVelocity - t);
                else
                    t = Math.Min(t, AngularVelocity + t);

                AngularVelocity = t;
                return;

            }
        }

        /// <summary>
        /// Luo uuden approximaation sopivasta muodosta.
        /// Hakee MuotoCachesta, jos sellainen on käytettävissä.
        /// 
        /// Jollain OpenCV:llä olisi helpompi, mutta ulkoisten riippuvuuksien välttämiseksi näin.
        /// </summary>
        /// <remarks>
        /// Fukken tin sux! Hiiren klikkaukset ei rekisteröidy välttämättä, ja fysiikkamoottorin parametrien tarkentaminen on estetty.
        /// </remarks>
        /// 
        [Obsolete()]
        public Shape LuoMuoto(bool force = false) 
        {
            if (this is Sankari)
                return Shape.Hexagon;

            return Shape.Circle;

            var avain = (string) GetType().FullName + ":" + Suunta;
            if (MuotoCache.ContainsKey(avain) && ! force )
                return MuotoCache[avain];

            // Käänteisen muodon avain.
            var avain_r = (string)GetType().FullName + ":" + (Suunta * -1);

            Debug.WriteLine("luo muoto");

            // Jos muotoa ei ole / löydy sopivaa, generoi yksi ynnäämällä läpinäkyviä pikseleitä spriteistä,
            // ja luomalla siitä uusi muoto.
            Image[] spritet;

            if (Animation != null)
            {
                spritet = new Image[Animation.FrameCount];
                // Animaation frameja ei saa suoraan, joten ne on kopioitava käsin.
                // Addendum: kristus.
                int frame = 0;
                foreach(Image kuva in Animation)
                {
                    spritet[frame] = kuva;
                    frame++;
                }
            } else
            {
                spritet = new Image[1];
                spritet[0] = this.Image;
            }
            
            // TODO: Hanskaa myös erikoiset spritet. Nyt koko katsotaan ekasta.
            int[,] läpinäkyvyys = new int[spritet[0].Height, spritet[0].Width];
            Color[,] pikselit;
            int r, c;
            int nr = spritet.Length;
            
            for(int i = 0; i < nr; i ++ )
            {
                pikselit = spritet[i].GetData();
                for(var l = 0; l < pikselit.Length; l++)
                {
                    r = l / spritet[i].Width;
                    c = l % spritet[i].Width;
                    läpinäkyvyys[r,c] += (int) pikselit[r,c].AlphaComponent;
                }
            }

            Color[,] bittidata = new Color[spritet[0].Height, spritet[0].Width];

            var peitto = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            var läpinäkyvä = new Color(byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue);

            var z = läpinäkyvyys.GetLength(1);
            // Käy pikselit läpi, ja tee läpinäkyväksi sopivat alueet.
            // TODO: etsi isoin muoto, ja poista muut.
            for (int i = 0; i < läpinäkyvyys.Length; i++)
            {
                r = i / z;
                c = i % z;

                var p = läpinäkyvyys[r, c] / nr;
                if (p > Muodon_toleranssi)
                    bittidata[r, c] = peitto;
                else
                    bittidata[r, c] = läpinäkyvä;
            }

            var bittikuva = new Image(spritet[0].Width, spritet[0].Height, Color.Transparent);
            bittikuva.SetData(bittidata);

            Shape shp = Shape.FromImage(bittikuva);
            
            Vector[] ver = shp.Cache.OutlineVertices;
            Vector[] polygonVertices = new Vector[ver.Length];
            for (int i = 0; i < ver.Length; i++)
                polygonVertices[i] = new Vector(ver[i].X * Suunta, ver[i].Y);

            ShapeCache cache = new ShapeCache(polygonVertices);

            MuotoCache[avain] = (Shape) shp;
            MuotoCache[avain_r] = (Shape)new Polygon(cache);

            return shp;
        }

    }
}
