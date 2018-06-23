using System;
using System.Linq;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Controls;
using Jypeli.Widgets;
using System.Diagnostics;

using Glide;

using Morte.Wädgetti;
using Morte.Öggiäiset;
using Morte.Aseet;
using Morte.Loot;
using Morte.FX;

using System.Net;
using System.Collections.Specialized;
using System.Threading.Tasks;

/// @author Teemu Autto
/// @version 04.04.2018

namespace Morte
{
    public class Morte : PhysicsGame
    {
        /// <summary>
        /// Taustakuvat by layer.
        /// Taustakuvat liitetään järjestyksessä eri tasoille, alkaen -3 + i.
        /// </summary>
        /// <remarks>
        /// Tasot liikkuvat eri vauhdilla, jos sillä on taustakuva.
        /// Joten nyt taustoina käytetään -3, -2, sekä 2.
        /// </remarks>
        public String[] taustat = new String[6] { "tausta-0", "tausta-1", null, null, "edusta", null };

        /// <summary>
        /// Thease are the numbers defined by the gods of yesteryears. Do not question them!
        /// </summary>
        private static int ikkunan_leveys = 700;
        private static int ikkunan_korkeus = 400;
        private static int kentän_leveys = 2200;

        /// <summary>
        /// Kuinka kauas pelaajasta vihulaisia voi lahdata
        /// </summary>
        const int OLETUS_KANTAMA = 300;
        public int Kantama = OLETUS_KANTAMA;

        /// <summary>
        /// Vahinkopisteiden hinta spesiaaliasetta käytettäessä.
        /// </summary>
        const int SPESIAALIASE_HINTA = 60;

        /// <summary>
        /// Hahmon oletuskoko. Jos tyhä (<c>Vector.Zero</c>), heataan hahmon ensilatauksella.
        /// </summary>
        private Vector Oletus_Koko = Vector.Zero;

        public int Pisteet = 0;
        public int Manattu = 0;

        /// <summary>
        /// Vihulaisen keskiverto -elinaika.
        /// Käytetään laskemaan uuden vihun tarpeellisuutta.
        /// </summary>
        protected TimeSpan Avg_Lifetime { get; set; }

        /// <summary>
        /// Maksimiviive uuden vihun luomiselle.
        /// </summary>
        const int VIHUSPAWNER_VIIVE = 7;

        /// <summary>
        /// Lista vihulaisista. Uutta vihua luotaessa näistä poimitaan yksi.
        /// </summary>
        public List<String> Vihulista = new List<String>{
            "Morte.Öggiäiset.Käärme",
            "Morte.Öggiäiset.Hullu",
            "Morte.Öggiäiset.Lokki",
            "Morte.Öggiäiset.Koura",
        };

        /// <summary>
        /// Lista lootboxksesita. (Todennäköisyys asteikolla 0.0 - 1.0, ja lootboxin luokka)
        /// </summary>
        public List<(double, string)> Lootboxit = new List<(double, string)>()
        {
            (0.2, "Morte.Loot.Hattu"),
            (0.1, "Morte.Loot.Viini"),
            (0.1, "Morte.Loot.Sieni"),
            (0.15, "Morte.Loot.Kannabis"),
            (0.05, "Morte.Loot.Saha"),
        };

        const string SPRITE_LOOTBOX = "loot/box";
        const string SPRITE_LOOTBOX_CRUSHED = "loot/box-crushed";

        const string INTRO_VIDEO = "intro";
        const double INTRO_LEIKKAA_LOPUSTA = 3.0;

        public const int VASEN = -1;
        public const int OIKEA = 1;

        /// <summary>
        /// Oletustaso jolle pappi lisätään.
        /// Seuraava ja edellinen taso tulisi olla normaaleita, spriteille käytettävissä olevia.
        /// </summary>
        public const int TASO_OLETUS = 0;
        public const int TASO_PAPPI = TASO_OLETUS - 1;
        public const int TASO_EDUSTA = 3;

        public Action TapahtumaLataa;
        public Action TapahtumaAlusta;
        public Action TapahtumaResetoi;
        public Action TapahtumaKäynnistä;

        /// <summary>
        /// Jypelin instanssi. Muutetaan se muutaman näppäimen säästämiseksi tässä oikeaan luokkaan.
        /// </summary>
        public new static Morte Instance { get; private set; }

        /// <summary>
        /// Paussiin sisältyvät objektit
        /// </summary>

        protected Timer _VihuSpawner;

        private Window paussiRuutu;

        private Kursori Kursori;
        public Kamerointi Kamera { get; set; }

        public Sankari Pelaaja;

        /// <summary>
        /// Health indicator
        /// </summary>
        protected Risti Risti;

        public Musiikki MusiikkiInstanssi { get; set; }

        /// <summary>
        /// Aseet
        /// </summary>
        public List<IAse> Ase;

        protected Layer HUD;

        public Jypeli.Effects.ParticleSystem Veriroiske;

        protected Image LootBoxSprite;

        protected VideoWädgetti IntroVideo;
        protected Task PelinLataaja;

        protected ScoreList Pistelista = new ScoreList(10, false, 0);
        protected Tweener Tweetteri = new Tweener();

        /// <summary>
        /// Verkko-osoite, johon tulokset lähetetään / josta ne ladataan.
        /// </summary>
        private string TulostauluURL = "https://morte-mysteria.appspot.com/scores.php";

        /// <summary>
        /// Super secret special -avain, jota käytetään sallimaan vain "oikeat" pyynnöt tulostauluun.
        /// </summary>
        /// <remarks>
        /// Pitäisi piilottaa, C# ei ole kovin luotettava tällaisessa.
        /// </remarks>
        private string TulostauluAvain = "Yksi lensi yli käenpesän.";

        private bool _AlkuNäyttöOhi;
        private bool _LopetaAlkuNäyttö;

        public Morte() : base()
        {
            Instance = this;
        }

        /// <summary>
        /// Alusta peli.
        /// </summary>
        public override void Begin()
        {
            base.Begin();

            SetWindowSize(ikkunan_leveys, ikkunan_korkeus, false);
            Screen.Width = ikkunan_leveys;
            Screen.Height = ikkunan_korkeus;

#if DEBUG
            DebugKeyEnabled = true;
#endif

            PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");

            TapahtumaAlusta  += AlustaTaso;

            TapahtumaLataa   += LataaShitit;
            TapahtumaLataa   += LataaPelaaja;
            TapahtumaLataa   += LataaTaustat;
            TapahtumaLataa   += LataaHUD;

            TapahtumaResetoi += AlustaAseet;
            TapahtumaResetoi += AlustaMusiikki;
            TapahtumaResetoi += AlustaVihuSpawner;
            TapahtumaResetoi += ResetoiPelaaja;

            TapahtumaKäynnistä += AsetaOhjain;
            TapahtumaKäynnistä += KäynnistäVihuSpawner;

            PelinLataaja = Task.Factory.StartNew(LataaPeli);

            TapahtumaAlusta?.Invoke();

            // Peli käynnistetään alkunäyttöjen päätyttyä.
            AlkuNäyttö();
        }


        #region Lataus
        /// <summary>
        /// Asettaa Game.Levelin.
        /// </summary>
        public void AlustaTaso()
        {
            Level.Height = ikkunan_korkeus;
            Level.Width = kentän_leveys;

            var maa = Level.CreateBottomBorder();
            maa.Tag = "maa";

            // Aseta maa
            Gravity = new Vector(0, -700);
        }


        public void LataaPelaaja()
        {
            // TODO
            // Luo pelaajan hahmo, ja tiputa maailmaan.
            Pelaaja = new Pappi();

            Pelaaja.Y = Camera.Y + Screen.Top + Pelaaja.Height;
            Pelaaja.OnKuolema += GameOver;

            // Tallenna pelaajan oletuskoko
            if (Oletus_Koko == Vector.Zero)
                Oletus_Koko = Pelaaja.Size;

        }

        /// <summary>
        /// Lataa omat objektit peliin, ideaalisti alkunäyttöjen aikana.
        /// </summary>
        /// TODO: Restructuroi
        public void LataaShitit()
        {
            Kursori = new Kursori()
            {
                IsVisible = false
            };
            Add(Kursori, TASO_EDUSTA);

            TapahtumaResetoi += () => Kursori.IsVisible = true;

            Veriroiske = new Bloodenstain(LoadImage("veripisara"), 100)
            {
                MinScale = 6,
                MaxScale = 12,
                Gravity = Gravity
            };
            Add(Veriroiske, TASO_EDUSTA);
            
            Kamera = new Kamerointi();
            // Käynnistä kamera myöhemmin, alkunäyttöjen päätyttyä.
            Kamera.IsUpdated = false;
            TapahtumaKäynnistä += () => Kamera.IsUpdated = true;

            Add(Kamera, TASO_EDUSTA);

            LootBoxSprite = Game.LoadImage(SPRITE_LOOTBOX);

        }

        void LataaHUD()
        {
            // LUD taso ja sille sälää.
            HUD = Layer.CreateStaticLayer();

            Risti = new Risti()
            {
                Sidos = Pelaaja,
                Y = Screen.Height / 2 - 48,
                X = Screen.Width / 2 * -1 + 35,
            };
            HUD.Objects.Add(Risti);

            var reunus = new Borderi();
            HUD.Objects.Add(reunus);

            TapahtumaKäynnistä += () => Layers.Add(HUD);
        }

        void LataaTaustat()
        {
            Level.BackgroundColor = new Color(133, 31, 10);

            PeliObjekti _tausta_obj;
            Image _tausta_img;

            double m;

            // Aseta ensin taustat, ja niiden relatiivinen liikkuminen.
            for (var i = 0; i < taustat.Length; i++)
            {
                if (taustat[i] == null) continue;

                _tausta_img = Game.LoadImage(taustat[i]);
                _tausta_obj = new PeliObjekti(_tausta_img)
                {
                    Y = Level.Bottom + _tausta_img.Height / 2
                };

                Add(_tausta_obj, -3 + i);
                m = (double)(_tausta_img.Width - Screen.Right) / Level.Width;
                Layers[-3 + i].RelativeTransition = new Vector(m, Math.Min(m, 1));
            }
        }

        protected void LataaPeli()
        {
            Debug.WriteLine("LataaPeli");
            TapahtumaLataa?.Invoke();
            Debug.WriteLine("Lataukset suoritettu");
        }

        #endregion

        /// <summary>
        /// Alustaa erikoisaseiden listan.
        /// </summary>
        protected void AlustaAseet()
        {
            if (Ase == null)
                Ase = new List<IAse>();
            else
                PysäytäAse();

            Ase.Clear();

            // Lisää oletusase
            Ase.Add(new Kranaatti());

        }

        /// <summary>
        /// Taustamusiikkihärpäke.
        /// </summary>
        /// TODO: Lue volume jostain.
        protected void AlustaMusiikki()
        {
            if(MusiikkiInstanssi == null)
                MusiikkiInstanssi = new Musiikki();
            else
                MusiikkiInstanssi.Pysäytä();

            MusiikkiInstanssi.Käynnistä();
        }

        /// <summary>
        /// Alustaa / resetoi vihuspawnerin.
        /// </summary>
        void AlustaVihuSpawner()
        {
            if (_VihuSpawner != null)
            {
                _VihuSpawner.Stop();
                _VihuSpawner.Reset();
            }
            else
            {
                _VihuSpawner = new Timer();
                _VihuSpawner.Timeout += VihuSpawner;
                // Interval asetetaan VihuSpawnerissa.
            }

            Avg_Lifetime = TimeSpan.FromSeconds(VIHUSPAWNER_VIIVE);
        }

        void KäynnistäVihuSpawner()
        {
#if (!DEBUG)
            VihuSpawner();
#endif
        }

        /// <summary>
        /// Asettaa ohjaimet
        /// </summary>
        void AsetaOhjain()
        {
            Mouse.ListenMovement(0.0, TarkkaileHiirtä, "Tarkkaile Hiiren etäisyyttä hahmosta");

            Keyboard.Listen(Key.P, ButtonState.Released, Paussi, "Pysäyttää pelin");

            Keyboard.Listen(Key.Left, ButtonState.Down, LiikutaPelaajaa, null, Pelaaja, VASEN);
            Keyboard.Listen(Key.Left, ButtonState.Released, PysäytäPelaaja, null, Pelaaja);
            Keyboard.Listen(Key.A, ButtonState.Down, LiikutaPelaajaa, null, Pelaaja, VASEN);
            Keyboard.Listen(Key.A, ButtonState.Released, PysäytäPelaaja, null, Pelaaja);

            Keyboard.Listen(Key.Right, ButtonState.Down, LiikutaPelaajaa, null, Pelaaja, OIKEA);
            Keyboard.Listen(Key.Right, ButtonState.Released, PysäytäPelaaja, null, Pelaaja);
            Keyboard.Listen(Key.D, ButtonState.Down, LiikutaPelaajaa, null, Pelaaja, OIKEA);
            Keyboard.Listen(Key.D, ButtonState.Released, PysäytäPelaaja, null, Pelaaja);

            Keyboard.Listen(Key.H, ButtonState.Released, () => AvaaPistelista(), "Avaa parhaiden pisteiden lista");

            // Aseta ase
            Keyboard.Listen(Key.Space, ButtonState.Pressed, LaukaiseAse, "Laukaise Erikoisase");
            Keyboard.Listen(Key.Space, ButtonState.Released, PysäytäAse, "Lopeta Erikoisase");

#if DEBUG
            Keyboard.Listen(Key.K, ButtonState.Released, GameOver, "Game Over");

            Keyboard.Listen(Key.Q, ButtonState.Down, delegate (Sankari p) { p.AngularVelocity += 1; Debug.WriteLine("Kierretään vasemmalle"); }, "Pyöritä Pelaajaa vasemmalle", Pelaaja);
            Keyboard.Listen(Key.E, ButtonState.Down, delegate (Sankari p) { p.AngularVelocity -= 1; Debug.WriteLine("Kierretään oikealle"); }, "Pyöritä Pelaajaa oikealle", Pelaaja);
            Keyboard.Listen(Key.W, ButtonState.Down, delegate (Sankari p) { p.Oikaise(); Debug.WriteLine("Oikaistaan pelaajaa"); }, "Oikaise pelaajaa", Pelaaja);

            Keyboard.Listen(Key.D0, ButtonState.Released, VihuSpawner, "Käynnistä vihuspawner");

            // Pikanäppäimet vihujen spawnaamiseen.
            for (var i = 0; i < Math.Min(Vihulista.Count(), 9); i++)
            {
                Keyboard.Listen(Key.D1 + i, ButtonState.Released, AmpiaisTehdas, "Uusi vihu " + Vihulista[i], i);
            }

            for (var i = 0; i < Math.Min(Lootboxit.Count(), 9); i++)
            {
                Keyboard.Listen<string>(Key.NumPad1 + i, ButtonState.Released, PudotaLootBox, "Pudota uusi lootbox " + Lootboxit[i].Item2, Lootboxit[i].Item2);
            }

            // Kameran zoomaus
            var mwtimer = new Timer();
            mwtimer.Interval = 0.1;
            mwtimer.Timeout += delegate ()
            {
                Camera.ZoomFactor = 1 * Camera.ZoomFactor + (double)Mouse.WheelChange / 10;
            };
            mwtimer.Start();
#endif

        }

        #region Alkunäytöt

        /// <summary>
        /// Toista alkunäytöt.
        /// </summary>
        void AlkuNäyttö()
        {
            Game.SmoothTextures = false;

            IntroVideo = new VideoWädgetti(Screen.Width, Screen.Height)
            {
                VideoTiedosto = INTRO_VIDEO,
                Päättyminen = INTRO_LEIKKAA_LOPUSTA,
            };
            
            IntroVideo.Y = Screen.Top + IntroVideo.Height / 2;

            IntroVideo.OnPäättymässä += AlkuNäyttöOhi;

            IntroVideo.OnStop += () => Debug.WriteLine("Intro OnStop");
            IntroVideo.OnStop += LopetaAlkuNäyttö;
            IntroVideo.OnKlikattaessa += () => Debug.WriteLine("Intro OnKlikattaessa");
            IntroVideo.OnKlikattaessa += AlkuNäyttöOhi;

            Camera.Position = IntroVideo.Position;

            IsMouseVisible = false;

            Add(IntroVideo, TASO_PAPPI);

            Keyboard.Listen(Key.Escape, ButtonState.Pressed, delegate ()
            {
                if (IntroVideo != null)
                    LopetaAlkuNäyttö();
            }, "Ohita Alkunäyttö");
        }

        /// <summary>
        /// Tapahtuma videon lähestyessä loppua. Panoroi kentän 0 -tasolle.
        /// </summary>
        protected void AlkuNäyttöOhi()
        {
            if (_AlkuNäyttöOhi) return;
            _AlkuNäyttöOhi = true;

            IntroVideo.Volume = 0d;

            IntroVideo.Tweetteri.Tween(Camera, new { Y = 0 }, (float)IntroVideo.Päättyminen).Ease(Ease.QuadInOut).OnComplete(IntroVideo.Stop);
        }

        /// <summary>
        /// Alkunäytön loppuessa -tapahtuma. Tuhoa video-objekti ja käynnistä peli.
        /// </summary>
        void LopetaAlkuNäyttö()
        {
            if (_LopetaAlkuNäyttö) return;
            _LopetaAlkuNäyttö = true;

            if (IntroVideo.IsPlaying)
                IntroVideo.Stop();

            // Varmista että tweening on ehtinyt päättyä ennen elementin poistoa.
            IntroVideo.Tweetteri.TargetCancel(Camera);
            //IntroVideo.Tweetteri.Update(0f);
            IntroVideo.Destroy();

            Camera.Y = 0;

            IsMouseVisible = true;
            Game.SmoothTextures = true;

            PelinLataaja.Wait();

            Debug.WriteLine(" -> Peli ladattu, käynnistetään.");

            Käynnistä();
        }

        #endregion


        public void Käynnistä()
        {
            Debug.WriteLine("Resetoidaan peli.");
            TapahtumaResetoi?.Invoke();
            System.GC.Collect();

            TapahtumaKäynnistä?.Invoke();
        }

        /// <summary>
        /// Aloita uusi peli.
        /// Resetoi parametrit, ja pyyhkii tunnetut hyödykkeet tasolta.
        /// </summary>
        public void UusiPeli()
        {
            var it = GetAllObjects();
            for (var i = 0; i < it.Count(); i++)
            {
                if (it[i] is Vihulainen)
                    ((Vihulainen)it[i]).Destroy();
                else if (it[i] is Lootboxi)
                    it[i].Destroy();
                else if(it[i] is Härpäke)
                    it[i].Destroy();
            }

            Manattu = 0;

            TapahtumaResetoi?.Invoke();

            IsPaused = false;
        }

        /// <summary>
        /// Resetoi pelaajan, ja pudottaa uudestaan peliin.
        /// </summary>
        /// TODO: Poista pelaaja, ja luo uudelleen.
        public void ResetoiPelaaja()
        {
            Pelaaja.X = 0;
            Pelaaja.Y = Camera.Y + Screen.Top + Pelaaja.Height;

            Pelaaja.Size = Oletus_Koko;

            Pelaaja.IgnoresPhysicsLogics = false;
            Pelaaja.IsVisible = true;

            Pelaaja.LataaOletukset();

            Kantama = OLETUS_KANTAMA;

            // Poista loot itemit
            for (int i = 0; i < Pelaaja.ObjectCount; i++) {
                if (Pelaaja.Objects[i] is Härpäke)
                    Pelaaja.Objects[i].Destroy();
            }

            if(! Pelaaja.IsAddedToGame)
                Add(Pelaaja, TASO_PAPPI);

            Risti.Päivitä();

            //Pelaaja.AngularVelocity = -8;
        }

        void LiikutaPelaajaa(Öggiäinen pelaaja, int suunta)
        {
            pelaaja.Suunta = suunta;
            pelaaja.Liikuta();
        }

        void PysäytäPelaaja(PhysicsObject pelaaja)
        {
            if(pelaaja.Bottom <= Level.Bottom)
                pelaaja.StopHorizontal();

            //pelaaja.Move(Vector.Zero);
        }

        public void TarkkaileHiirtä()
        {
            double etäisyys = Vector.Distance(Mouse.PositionOnWorld, Pelaaja.AbsolutePosition);

            if (IsPaused == true)
                Kursori.OOR = false;
            else if (etäisyys > Kantama)
                Kursori.OOR = true;
            else
                Kursori.OOR = false;

            return;
        }

        public void LaukaiseAse()
        {
            Debug.WriteLine("Laukaistaan spesiaaliase");
            Ase.Last<IAse>().Laukaise(this);
        }

        public void PysäytäAse()
        {
            Debug.WriteLine("Pysäytetään spesiaaliase");
            Ase.Last<IAse>().Pysäytä();
        }

        /// <summary>
        /// Spawnaa vihuja hieman keskiverto vihun elämää tiheämmällä tahdilla. 
        /// </summary>
        public void VihuSpawner() {
            AmpiaisTehdas();

            double odotus;
            var avg = new TimeSpan(Avg_Lifetime.Ticks / 10 * 8);
            odotus = (float)avg.Seconds + ((float)avg.Milliseconds / 1000);

            _VihuSpawner.Interval = Math.Min((odotus > 0) ? odotus : VIHUSPAWNER_VIIVE, VIHUSPAWNER_VIIVE);

            _VihuSpawner.Start(1);

        }

        /// <summary>
        /// Spawnaa uuden satunnaisen vihulaisen.
        /// </summary>
        public void AmpiaisTehdas(int vihu_idx=-1)
        {
            if(vihu_idx == -1)
                vihu_idx = RandomGen.NextInt(Vihulista.Count());

            var vihu_str = Vihulista[vihu_idx];
            Type t = Type.GetType(vihu_str);
            var vihu = (Vihulainen) Activator.CreateInstance(t);

            vihu.Pelaaja = Pelaaja;
            vihu.Tag = "vihu";
            vihu.OnKuolema += ArvoLootbox;
            vihu.OnTapettu += delegate () { VihuTapettu(vihu); };

            vihu.Suunta = RandomGen.SelectOne(Morte.VASEN, Morte.OIKEA);
            vihu.X = (Level.Width / 2 + vihu.Width - 1) * vihu.Suunta * -1;

            AddCollisionHandler<Öggiäinen, Vihulainen>(Pelaaja, vihu, VihuHyögii);
            Mouse.ListenOn<Vihulainen>(vihu, MouseButton.Left, ButtonState.Released, IskeVihua, "Iske "+vihu.GetType().Name+" dunkkuun", vihu);

            Add(vihu, TASO_OLETUS);
            vihu.Hyöki();
        }

        /// <summary>
        /// Tapahtuma vihulaisen osuessa pelaajaan.
        /// </summary>
        /// <param name="pelaaja"></param>
        /// <param name="vihu"></param>
        public void VihuHyögii(Öggiäinen pelaaja, Vihulainen vihu)
        {
            Debug.WriteLine("Vihu hyögii niskaan");
            vihu.VahingoitaSankaria(pelaaja);

        }

        /// <summary>
        /// Hiiren tapahtuma klikattaessa vihua.
        /// </summary>
        /// <param name="vihu"></param>
        public void IskeVihua(Vihulainen vihu)
        {
            if (!Kursori.OOR && !IsPaused)
            {
                vihu.Vahingoita(Pelaaja.Vahinko);

                int veriroiskeita = ( 1 - (vihu.MaxHitpoints - vihu.Hitpoints) / vihu.MaxHitpoints ) * 29 + 1;
                Veriroiske.AddEffect(Mouse.PositionOnWorld.X, Mouse.PositionOnWorld.Y, veriroiskeita);
            }
        }

        /// <summary>
        /// Lisää vihu lahdattujen joukkoon.
        /// </summary>
        /// <param name="vihu"></param>
        protected void VihuTapettu(Vihulainen vihu)
        {
            Manattu += 1;
            Pisteet += vihu.PisteArvo;

            if (Manattu == 1)
                Avg_Lifetime = vihu.Lifetime;
            else
            {
                var diff = (vihu.Lifetime - Avg_Lifetime).Ticks / Manattu;
                Avg_Lifetime += new TimeSpan(diff);
            }

            Debug.WriteLine("Vihulainen tapettu. Pisteet: " + Pisteet);
        }


#region Gambling

        /// <summary>
        /// Arvo uusi lootbox. Ehkä.
        /// </summary>
        public void ArvoLootbox()
        {
            var lootbox_idx = RandomGen.NextInt(Lootboxit.Count()-1) ;
            var lootbox = Lootboxit[lootbox_idx];
            Debug.WriteLine("Arvotaan uutta lootboxia: " + lootbox.Item2);

            if (RandomGen.NextDouble(0, 1) > lootbox.Item1)
                return;

            Debug.WriteLine("Uusi lootbox valittu: " + lootbox.Item2);

            PudotaLootBox(lootbox.Item2);

        }

        /// <summary>
        /// Pudottaa uuden lootboxin peliin, ja asettaa sille tapahtumat.
        /// </summary>
        /// <param name="sisältöluokka"></param>
        public void PudotaLootBox(string sisältöluokka="Morte.Loot.Hattu")
        {
            var laatikko = new Lootboxi(LootBoxSprite)
            {
                Sisältö = sisältöluokka
            };

            AddCollisionHandler<Sankari, Lootboxi>(Pelaaja, laatikko, AvaaLootBox);
            AddCollisionHandlerByTag<Lootboxi,Vihulainen>(laatikko, "vihu", LittaaLootbox);
            Add(laatikko, TASO_OLETUS + 1);

        }

        public void AvaaLootBox(Sankari pelaaja, Lootboxi laatikko)
        {
            if (laatikko.Avattu)
                return;

            laatikko.Stop();
            laatikko.Velocity = Vector.Zero;

            laatikko.Avaa();

            Pisteet += 1;
            
            Type t = Type.GetType(laatikko.Sisältö);
            var lootti = (Härpäke)Activator.CreateInstance(t);

            lootti.Position = new Vector(laatikko.X, laatikko.Top + lootti.Height);

            Add(lootti, TASO_OLETUS);

            lootti.Paljastus();
            
            Timer.SingleShot(1, delegate() {
                lootti.Poimi(pelaaja);
                laatikko.Destroy();
            });
        }

        public void LittaaLootbox(Lootboxi laatikko, Vihulainen vihu)
        {
            if (laatikko.Avattu)
                return;

            Debug.WriteLine("Vihu " + vihu.GetType().Name + " törmäsi lootboxiin, littaa se");
            laatikko.Avattu = true;

            var top = laatikko.Top;
            laatikko.Image = Game.LoadImage(SPRITE_LOOTBOX_CRUSHED);
            laatikko.Y = top - laatikko.Image.Height / 2;
            laatikko.Height = laatikko.Image.Height;

            Timer.SingleShot(1, delegate() {
                laatikko.IgnoresCollisionResponse = true;
            });

            //laatikko.Height = laatikko.Height / 10;

        }

#endregion

        public void AvaaPistelista(bool tallennetaan=false)
        {
            
            var old_kursori = Kursori.IsVisible;
            var old_Pause = IsPaused;

            Kursori.IsVisible = false;
            IsPaused = true;

            //paussiRuutu?.Destroy();

            HighScoreWindow ranking_ikkuna = new HighScoreWindow(
                             "Parhaat pisteet",
                             "Kerro nimesi:",
                             Pistelista, tallennetaan ? Pisteet : 0 )
            {
                Color = new Color(0, 0, 0, 0.8)
            };

            ranking_ikkuna.Message.TextColor = new Color(255, 255, 0);
            
            ranking_ikkuna.Closed += delegate(Window w) { IsPaused = old_Pause; Kursori.IsVisible = old_kursori; };
            HUD.Objects.Add(ranking_ikkuna);
        }

        /// <summary>
        /// Pysäyttää pelin suorituksen
        /// </summary>
        public void Paussi()
        {
            IsPaused = !IsPaused;
            Debug.WriteLine("Asetetaan Pause: " + (IsPaused ? "on" : "off"));

            if (IsPaused)
            {
                Kursori.IsVisible = false;
                paussiRuutu?.Destroy();
                paussiRuutu = new Window(Screen.Width, Screen.Height / 4)
                {
                    Color = new Color(0, 0, 0, 0.8),
                };

                //paussiRuutu.Add(container);
                
                var rivit = new Widget(new VerticalLayout() { Spacing = 5.0 })
                {
                    Color = Color.Transparent
                };

                rivit.Add(new Label("Paussi") {
                    TextColor = new Color(204,204,204)
                });
                
                var lopeta = new PushButton("Jatka")
                {
                    TextScale = new Vector(0.9, 0.9),
                    TextColor = Color.Black,
                    Color = new Color(227, 0, 0)
                };

                lopeta.Clicked += Paussi;
                rivit.Add(lopeta);

                paussiRuutu.Add(rivit);
                
                HUD.Objects.Add(paussiRuutu);

                MusiikkiInstanssi.Pysäytä();

                Keyboard.Listen(Key.P, ButtonState.Released, Paussi, "Jatka peliä").InContext(paussiRuutu);
                Keyboard.Listen(Key.Escape, ButtonState.Released, Paussi, "Jatka peliä").InContext(paussiRuutu);
            }
            else
            {
                Kursori.IsVisible = true;
                paussiRuutu.Destroy();
                MusiikkiInstanssi.Käynnistä();
            }

        }

        public void GameOver()
        {
            IsPaused = true;
            Kursori.IsVisible = false;

            paussiRuutu?.Destroy();
            paussiRuutu = new Window(Screen.Width, Screen.Height / 4)
            {
                Color = new Color(0, 0, 0, 0.8),
            };

            var rivit = new Widget(new VerticalLayout() { Spacing = 10 }){
                Color = Color.Transparent
            };
            rivit.Add(new Label("Game Over")
            {
                Color = Color.Transparent,
                TextColor = new Color(204, 204, 204)
            });

            var napit = new Widget(new HorizontalLayout() { Spacing = 10.0 })
            {
                Color = Color.Transparent
            };

            var nappi_uusi = new PushButton("Uusi peli")
            {
                TextScale = new Vector(0.9, 0.9),
                TextColor = Color.Black,
                Color = new Color(230, 200, 200),
            };
            var nappi_pisteet = new PushButton("Pisteet")
            {
                TextScale = new Vector(0.9, 0.9),
                TextColor = Color.Black,
                Color = new Color(230, 230, 0)
            };
            var nappi_lopeta = new PushButton("Lopeta") {
                TextScale = new Vector(0.9, 0.9),
                TextColor = Color.Black,
                Color = new Color(230, 0, 0)
            };

            nappi_uusi.Clicked += delegate () { paussiRuutu?.Destroy(); UusiPeli(); };
            nappi_pisteet.Clicked += delegate() { AvaaPistelista(); };
            nappi_lopeta.Clicked += Exit; 

            napit.Add(nappi_uusi);
            napit.Add(nappi_pisteet);
            napit.Add(nappi_lopeta);

            rivit.Add(napit);
            paussiRuutu.Add(rivit);

            HUD.Objects.Add(paussiRuutu);
        }

    }
}