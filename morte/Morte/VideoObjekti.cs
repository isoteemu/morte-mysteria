using System;
using System.Diagnostics;
using Jypeli;

using XNATexture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using Microsoft.Xna.Framework.Media;


namespace Morte
{

    /// <summary>
    /// GameObject johon voi ympätä videon.
    /// </summary>
    [System.Obsolete("Käytä VideoWädgettiä")]
    public class VideoObjekti : PeliObjekti
    {

        public VideoPlayer videoPlayer = new VideoPlayer();
        public Video video;

        private XNATexture2D videoTexture;

        /// <summary>
        /// Kutsu jota kutsutaan kun video on pysähtynyt.
        /// </summary>
        public event Action OnStopped;

        public VideoObjekti(double width = 600, double height = 400) : base(width, height, Shape.Rectangle)
        {
            IsUpdated = true;
            IgnoresLighting = true;

            Destroyed += VideoStop;
        }

        public void VideoPause()
        {
            videoPlayer.Pause();
        }

        // TODO: Mieti tämän kutsurakenne uudelleen.
        public void VideoStop()
        {
            if (videoPlayer.State != MediaState.Stopped)
            {
                IsUpdated = false;
                videoPlayer.Stop();
                videoPlayer.Dispose();
            }

            OnStopped();
        }

        /// <summary>
        /// Update kutsu jolla piirretään video.
        /// Kopioi videon tekstuurin GameObjektin Imagekeksi, jonka jypeli osaa piirtää.
        /// </summary>
        /// <param name="time"></param>
        public override void Update(Time time)
        {
            XNATexture2D _videoTexture = null;
            // Kopioi videon data kuvaan.

            if (videoPlayer.State != MediaState.Stopped)
                _videoTexture = videoPlayer.GetTexture();

            if (_videoTexture != null && _videoTexture != videoTexture)
            {
                videoTexture = _videoTexture;
                this.Image = new Image(videoTexture);
            }

            if (videoPlayer.State == MediaState.Stopped)
            {
                Debug.WriteLine("Video on pysähtynyt");
                OnStopped();
            }

            base.Update(time);
        }
    }
}
