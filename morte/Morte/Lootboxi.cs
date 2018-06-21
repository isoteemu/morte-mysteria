using System;
using System.Diagnostics;
using Jypeli;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morte
{
    /// <summary>
    /// Yksinkertainen PhysicsObject joka muistaa rivin tekstiä.
    /// </summary>
    public class Lootboxi : FysiikkaObjekti
    {
        /// <summary>
        /// Sisällön luokan nimi.
        /// </summary>
        public string Sisältö { get; set; }

        public bool Avattu { get; set; }

        public Lootboxi() : this(Game.LoadImage("loot/box")) { }

        public Lootboxi(Image sprite) : base(sprite)
        {
            Y = Game.Level.Top;
            X = RandomGen.NextDouble(Game.Level.Left, Game.Level.Right);
            Mass = 2.5;

            Tag = "lootbox";

            //Shape = Shape.CreateRegularPolygon(5);
        }

        public void Avaa() {
            if (Avattu) return;
            Avattu = true;

            Debug.WriteLine("Lootbox avataan...");

            //fx.Y = fx.Height / 2 + 9;
            //Add(fx);
        }
    }



}
