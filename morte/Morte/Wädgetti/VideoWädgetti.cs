using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XNAMedia = Microsoft.Xna.Framework.Media;
using XNATexture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

using Jypeli;

namespace Morte.Wädgetti
{
    public class VideoWädgetti : Vitkutin
    {
        
        public event Action OnPlay;
        public event Action OnPause;
        public event Action OnStop;
        public event Action OnKlikattaessa;

        /// <summary>
        /// Tapahtuma jota kutsutaan videon lähestyessä loppua
        /// </summary>
        public event Action OnPäättymässä;

        /// <summary>
        /// Kuinka kauan ennen päättymistä kutsutaan <c>OnPäättymässä</c>.
        /// </summary>
        /// <remarks>
        /// Tätä spämmätään. Jos halutaan vain kerran, on video pysäytettävä:
        /// <code>
        /// OnPäättymässä += MyShit;
        /// OnPäättymässä += videovimpain.Stop;
        /// </code>
        /// </remarks>
        public double Päättyminen = 1.5;

        public bool IsLooped { get => VideoPlayer.IsLooped; set => VideoPlayer.IsLooped = value; }
        public double Volume { get => VideoPlayer.Volume; set => VideoPlayer.Volume = (float) value; }

        public bool IsPlaying
        {
            get
            {
                return VideoPlayer.State != XNAMedia.MediaState.Playing;
            }
        }

        /// <summary>
        /// Määrittele videotiedosto content -nimen perusteella. Säästyy XNA importilta, jota Jypelin kehittäjät
        /// haluavat estellä.
        /// </summary>
        public string VideoTiedosto
        {
            set
            {
                Video = Game.Content.Load<XNAMedia.Video>(value);
            }
        }

        protected XNAMedia.VideoPlayer VideoPlayer = new XNAMedia.VideoPlayer();

        public XNAMedia.Video Video;

        private XNATexture2D videoTexture;

        private XNAMedia.MediaState State = XNAMedia.MediaState.Stopped;

        public VideoWädgetti(double width, double height) : base(width, height)
        {
            IsUpdated = true;
            Destroyed += VideoPlayer.Dispose;
            AddedToGame += delegate () {
                Game.Mouse.ListenOn(this, MouseButton.Left, ButtonState.Released, OnKlikattaessa, null).InContext(this);
                Play();
            };
        }
        
        /// <summary>
        /// Käynnistä video. Jatka jos pausetettu.
        /// </summary>
        public void Play()
        {
            State = XNAMedia.MediaState.Playing;
            if (VideoPlayer.State == XNAMedia.MediaState.Stopped)
            {
                Debug.WriteLine("Käynnistetään video");
                for (int i = 0; i < 5; i++)
                {

                    try // Aina ei jostain kääntäjän syystä natsaa.
                    {
                        VideoPlayer.Play(Video);
                        OnPlay?.Invoke();
                        break;
                    }
                    catch (InvalidOperationException)
                    {
                        Debug.WriteLine("Videon lataus yritys #"+i+" epäonnistui");
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
            else if (VideoPlayer.State == XNAMedia.MediaState.Paused)
            {
                VideoPlayer.Resume();
                OnPlay?.Invoke();
            }

        }

        public void Pause()
        {
            State = XNAMedia.MediaState.Paused;
            VideoPlayer.Pause();
            OnPause?.Invoke();
        }

        public new void Stop()
        {
            State = XNAMedia.MediaState.Stopped;
            Debug.WriteLine("VideoWädgetti.Stop()");
            if (VideoPlayer.State != XNAMedia.MediaState.Stopped)
            {
                VideoPlayer.Stop();
                VideoPlayer.Dispose();
                OnStop?.Invoke();
            }

            base.Stop();
        }

        public override void Update(Time time)
        {
            XNATexture2D _videoTexture = null;

            if (State == XNAMedia.MediaState.Playing && VideoPlayer.State == XNAMedia.MediaState.Stopped)
            {
                /// Video saapunut loppuun.
                OnStop?.Invoke();
                Stop();
            }


            // Kopioi videon data kuvaan.
            if (VideoPlayer.State != XNAMedia.MediaState.Stopped)
                _videoTexture = VideoPlayer.GetTexture();

            if (_videoTexture != null && _videoTexture != videoTexture)
            {
                videoTexture = _videoTexture;
                this.Image = new Image(videoTexture);
            }

            // Tarkasta onko päättymässä. Huomaa että tätä spämmätään.
            if(VideoPlayer.State == XNAMedia.MediaState.Playing)
            {
                TimeSpan ennen = VideoPlayer.PlayPosition + TimeSpan.FromSeconds(Päättyminen);

                if(ennen > Video.Duration)
                {
                    OnPäättymässä?.Invoke();
                }
            }

            base.Update(time);
        }

    }
}
