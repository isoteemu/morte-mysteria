using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jypeli;

namespace Morte.Ikkunoita
{

    public class Ikkuna : Window
    {
        public Widget Sisältö;
        public Ikkuna(double width, double height) : base(width, height) {
            Color = new Color(0,0,0,0.9);

            Sisältö = new Widget(new VerticalLayout()
            {
                Spacing = 20,
                LeftPadding = 15,
                RightPadding = 15,
                TopPadding = 15,
                BottomPadding = 15,
            }) { Color = Color.Transparent };
            Add(Sisältö);
        }

        /// <summary>
        /// Luo uusi Widget sarakkeilla (HorizontalLayout)
        /// </summary>
        /// <returns></returns>
        public static Widget LuoWidgetti()
        {
            Widget widgetti = new Widget(new HorizontalLayout() { Spacing = 10.0 })
            {
                Color = Color.Transparent,
            };
            return widgetti;
        }
        
        public static PushButton LuoNappi(string teksti)
        {
            var nappi = new PushButton(teksti)
            {
                TextScale = new Vector(0.8, 0.8),
                TextColor = new Color(253, 253, 255),
                Color = new Color(230, 200, 200),
                YMargin = 7.0
            };

            return nappi;
        }

        public static Label LuoTeksti(string teksti)
        {
            Label label = new Label(teksti)
            {
                TextColor = new Color(248, 143, 28)
            };

            return label;
        }

    }

    public class LadataanIkkuna : Ikkuna
    {
        public Widget Nappirivi { get; set; }

        public PushButton HailSatan { get; set; }
        public PushButton PeruutaNappi { get; set; }

        public LadataanIkkuna() : base(Game.Screen.Width, Game.Screen.Height / 4)
        {

            AddedToGame += delegate ()
            {
                Game.Keyboard.Listen(Key.Escape, ButtonState.Released, this.Close, "Sulje ikkuna").InContext(this);
            };

            Sisältö.Add(LuoTeksti("Hailing Satan"));
            Nappirivi = LuoWidgetti();
            Sisältö.Add(Nappirivi);

            HailSatan = LuoNappi("Hail Satan!");
            HailSatan.Color = new Color(230, 200, 200);
            Nappirivi.Add(HailSatan);

            HailSatan.Clicked += delegate ()
            {
                Game.LoadSoundEffect("hail").Play();
            };

            PeruutaNappi = LuoNappi("Armahda");
            PeruutaNappi.Color = new Color(230, 0, 0);
            Nappirivi.Add(PeruutaNappi);

            PeruutaNappi.Clicked += Close;
        }
    }

    public class PisteIkkuna : Ikkuna
    {

        public Label Otsikko { get; set; }
        public Widget Tulokset { get; set; }
        public Widget Nappirivi { get; set; }
        public PushButton SuljeNappi { get; set; }

        public List<ScoreItem> Pistelista { get; set; }
        public PisteIkkuna(List<ScoreItem> pistelista) : base(Game.Screen.Width, Game.Screen.Height)
        {
            Pistelista = pistelista;

            AddedToGame += delegate ()
            {
                Game.Keyboard.Listen(Key.Escape, ButtonState.Released, this.Close, "Sulje ikkuna").InContext(this);
            };

            Otsikko = LuoTeksti("Pahimmat armottomat");
            Sisältö.Add(Otsikko);

            Otsikko.TextColor = new Color(248, 143, 28);
            Otsikko.SizeMode = TextSizeMode.AutoSize;
            Otsikko.HorizontalAlignment = HorizontalAlignment.Center;
            Otsikko.VerticalAlignment = VerticalAlignment.Top;


            Tulokset = new Widget(new VerticalLayout() { Spacing = 5, LeftPadding = 25, RightPadding = 25 }) { Color = Color.Transparent };
            Sisältö.Add(Tulokset);

            for(int i = 0; i < pistelista.Count; i++) {
                var tulos = ScoresPHP.ValidoiScoreItem(pistelista[i]);
                var siw = LuoWidgetti();

                var t = LuoTeksti(String.Format("{0}.", tulos.Position));
                t.HorizontalAlignment = HorizontalAlignment.Left;
                t.TextColor = new Color(239, 239, 239);
                siw.Add(t);

                var n = LuoTeksti(tulos.Name);
                n.SizeMode = TextSizeMode.None;
                n.HorizontalAlignment = HorizontalAlignment.Left;
                n.XMargin = 20;
                n.HorizontalSizing = Sizing.Expanding;
                siw.Add(n);

                var s = LuoTeksti(String.Format("{0}", tulos.Score));
                s.HorizontalAlignment = HorizontalAlignment.Right;
                s.TextColor = new Color(173, 171, 169);
                siw.Add(s);

                if (i % 2 == 0)
                {
                    t.TextColor = Color.Darker(t.TextColor, 30);
                    n.TextColor = Color.Darker(n.TextColor, 30);
                    s.TextColor = Color.Darker(s.TextColor, 30);
                }

                Tulokset.Add(siw);
            }

            Nappirivi = LuoWidgetti();
            Sisältö.Add(Nappirivi);
            
            SuljeNappi = LuoNappi(RandomGen.SelectOne<string>("Perkele", "Saatana", "Jumalauta"));
            SuljeNappi.HorizontalAlignment = HorizontalAlignment.Right;
            SuljeNappi.Color = new Color(250, 158, 26);

            SuljeNappi.Clicked += this.Close;
            Nappirivi.Add(SuljeNappi);

        }
        
    }


    public class PaussiIkkuna : Ikkuna
    {
        public Widget Nappirivi { get; set; }

        public PushButton PaussiNappi { get; set; }
        public PushButton PisteetNappi { get; set; }
        public PushButton LopetaNappi { get; set; }


        public PaussiIkkuna() : base(Game.Screen.Width, Game.Screen.Height / 4)
        {
            //paussiRuutu.Add(container);

            Sisältö.Add(LuoTeksti("Paussi"));

            Nappirivi = LuoWidgetti();
            Sisältö.Add(Nappirivi);

            PaussiNappi = LuoNappi("Jatka");
            PaussiNappi.Color = new Color(250, 158, 26);
            Nappirivi.Add(PaussiNappi);

            PisteetNappi = LuoNappi("Pisteet");
            PisteetNappi.Color = new Color(249, 27, 227);
            Nappirivi.Add(PisteetNappi);

            LopetaNappi = LuoNappi("Luovuta");
            PisteetNappi.Color = new Color(247, 67, 27);
            Nappirivi.Add(LopetaNappi);

        }
    }

    public class GameOverIkkuna : Ikkuna
    {
        public Widget Nappirivi { get; set; }

        public PushButton UusipeliNappi { get; set; }
        public PushButton PisteetNappi { get; set; }
        public PushButton LopetaNappi { get; set; }

        public GameOverIkkuna() : base(Game.Screen.Width, Game.Screen.Height / 4)
        {
            Sisältö.Add(LuoTeksti("Game Over"));

            Nappirivi = LuoWidgetti();
            Sisältö.Add(Nappirivi);

            UusipeliNappi = LuoNappi("Uusi peli");
            UusipeliNappi.Color = new Color(250, 158, 26);
            Nappirivi.Add(UusipeliNappi);

            PisteetNappi = LuoNappi("Pisteet");
            PisteetNappi.Color = new Color(249, 27, 227);
            Nappirivi.Add(PisteetNappi);

            LopetaNappi = LuoNappi("Lopeta");
            PisteetNappi.Color = new Color(247, 67, 27);
            Nappirivi.Add(LopetaNappi);

        }
    }
}
