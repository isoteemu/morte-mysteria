using Jypeli;
using Jypeli.Assets;

namespace Morte.Aseet
{
    public class HolyHandgranade : Grenade
    {
        const string KuvaAsset      = "kranaatti";
        const string HohdeAsset     = "kranaatti-hohde";
        const string RäjähdysÄäni   = "hallelujah";

        public HolyHandgranade() : this(16.0) { }

        public HolyHandgranade(double radius) : base(radius)
        { 
            IgnoresPhysicsLogics = false;
            Image = Game.LoadImage(KuvaAsset);

            LinearDamping = 0.99;
            Width = Image.Width;
            Height = Image.Height;

            Explosion.Sound = Game.LoadSoundEffect(RäjähdysÄäni);

            /// TODO: muuta SpriteFX:ksi
            GameObject glow = new PeliObjekti(Game.LoadImage(HohdeAsset));
            glow.Shape = Shape.Circle;
            Add(glow);
        }

    }


    public class Kranaatti : IAse
    {
        const int SPESIAALIASE_HINTA = 60;
        const int VAHINKO = 9999;
        const int VOIMA = 7000;

        public int Käyttöhinta = SPESIAALIASE_HINTA;
        public int Vahinko = VAHINKO;

        public void Laukaise(Morte peli)
        {
            HolyHandgranade kranaatti = new HolyHandgranade()
            {
                AngularVelocity = 1 * peli.Pelaaja.Suunta
            };

            var suunta = (peli.Pelaaja.X >= peli.Mouse.PositionOnWorld.X) ? Angle.FromDegrees(135) : Angle.FromDegrees(45);

            peli.Pelaaja.Throw(kranaatti, suunta, VOIMA);
            peli.AddCollisionHandlerByTag<HolyHandgranade, Vihulainen>(kranaatti, "vihu", OsuVihuun);

            kranaatti.Explosion.AddShockwaveHandler("vihu", ShokkiaaltoOsuu);
            peli.Pelaaja.Vahingoita(Käyttöhinta);
        }

        public void Pysäytä() { }

        public void OsuVihuun(HolyHandgranade kranaatti, Vihulainen vihu)
        {
            kranaatti.Explode();
            ((Vihulainen)vihu).Vahingoita(Vahinko);
        }

        public void ShokkiaaltoOsuu(IPhysicsObject vihu, Vector Shokki)
        {
            vihu.Push(Shokki);
            ((Vihulainen)vihu).Vahingoita(Vahinko);
        }
    }
}