using System;
using System.Diagnostics;
using Jypeli;

using Morte;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morte.Loot
{
    public class Härpäke : FysiikkaObjekti
    {
        public const double ZOOMAUKSEN_KESTO = 1.7;
        
        public Sankari Poimija;

        public event Action OnPaljastus;
        public event Action OnPoiminta;
        public event Action OnKlikattattaessa;

        /// <summary>
        /// If object is dropped, can it be marked as dead.
        /// </summary>
        protected bool CanBeDead = false;

        public Härpäke(Image sprite) : base(sprite)
        {
            Game.AssertInitialized(AsetaTapahtumat);
        }

        protected virtual void AsetaTapahtumat()
        {
            OnPaljastus += () => Debug.WriteLine("Lootitemin paljastus");
            OnPoiminta += () => Debug.WriteLine("Lootitemin poiminta");


            Game.Mouse.ListenOn(this, MouseButton.Left, ButtonState.Pressed, KlikkausTapahtuma, "Poimi "+GetType().Name);

            OnPaljastus += KameraAjo;
        }

        public void KlikkausTapahtuma() {
            Debug.WriteLine("Klikattu härpäkettä " + GetType().Name);
            OnKlikattattaessa?.Invoke();
        }

        public virtual void KameraAjo()
        {
            var Peli = (Morte)Game.Instance;
            Peli.Kamera.Kohdista(AbsolutePosition, 0.4f, 2.5);

            var fx = new FX.Sunrays();
            fx.Bind(this);

            var _IgnoresGravity = IgnoresGravity;
            var _Acceleration = Acceleration;
            var _Velocity = Velocity;
            var _IgnoresPhysicsLogics = IgnoresPhysicsLogics;

            Stop();
            IgnoresGravity = true;
            IgnoresPhysicsLogics = true;

            Timer.SingleShot(ZOOMAUKSEN_KESTO,  delegate() {
                Peli.Kamera.Palauta();
                fx.Destroy();
                IgnoresGravity = _IgnoresGravity;
                Acceleration = _Acceleration;
                Velocity = _Velocity;
                IgnoresPhysicsLogics = _IgnoresPhysicsLogics;

            });

        }

        public void Paljastus()
        {
            OnPaljastus?.Invoke();
        }

        /// <summary>
        /// Tapahtuma joka tapahtuu tavaraa poimittaessa.
        /// </summary>
        /// <param name="poimija"></param>
        public virtual void Poimi(Sankari poimija)
        {
            Poimija = poimija;
            Game.DoNextUpdate(delegate ()
            {
                OnPoiminta?.Invoke();
            });

        }

        public void HiiriKlikkaus()
        {
            OnKlikattattaessa?.Invoke();
        }

        public override void Update(Time time)
        {
            if (CanBeDead && Velocity == Vector.Zero && Parent == null)
            {
                // Mark it as dead.
                Debug.WriteLine("Dead Härpäke: " + GetType().Name);
                //IgnoresPhysicsLogics = true;
                IgnoresCollisionResponse = true;
                IsUpdated = false;
            }
            base.Update(time);
        }
    }

    public class PudotettavaHärpäke : Härpäke
    {
        public const double VELOCITY_TOLERANCE = 5;

        /// <summary>
        /// Voima jonka liikkeen esine kestää. Pienemmät resetoidaan nollaksi.
        /// </summary>
        protected double VelocityTolerance = VELOCITY_TOLERANCE;

        public event Action OnPudotettaessa;

        public PudotettavaHärpäke(Image sprite) : base(sprite)
        {
            LinearDamping = 0.99;

            IsUpdated = true;
        }

        public override void Poimi(Sankari poimija)
        {

            base.Poimi(poimija);

            if (Parent != null)
                Parent.Remove(this);
            else
                Game.Remove(this);

            IgnoresGravity = true;

            Stop();
            Velocity = Vector.Zero;
            Angle = Angle.Zero;
            IsVisible = false;

            Game.DoNextUpdate(delegate ()
            {
                IsVisible = true;
                CollisionIgnoreGroup = poimija.CollisionIgnoreGroup;
                poimija.Add(this);
            });

        }

        public void Pudota()
        {
            var _Position = AbsolutePosition;
            var _Angle = AbsoluteAngle;
            Poimija.Remove(this);
            IsVisible = false;

            Game.DoNextUpdate(delegate ()
            {
                Position = _Position;
                Angle = _Angle;
                IsVisible = true;
                Game.Add(this);
                IgnoresGravity = false;
                CanBeDead = true;

                OnPudotettaessa?.Invoke();
            });
        }

        public override void Update(Time time)
        {
            if (Parent != null && Velocity != Vector.Zero)
            {
                if(Velocity.Magnitude >= VelocityTolerance)
                    Pudota();
            }
            base.Update(time);
        }
    }

}
