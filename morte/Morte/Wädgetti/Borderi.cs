using System;
using Jypeli;

namespace Morte.Wädgetti
{
    class Borderi : PeliObjekti
    {
        public Borderi() : base(700, 400)
        {
            Image = Game.LoadImage("border");
            Game.AssertInitialized(this.PäivitäKoko);
        }

        public void PäivitäKoko()
        {
            Width = Game.Screen.Width;
            Height = Game.Screen.Height; 
        }
    }
}
