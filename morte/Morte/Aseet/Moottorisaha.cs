using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jypeli;

namespace Morte.Aseet
{
    public class Moottorisaha : IAse
    {

        public Action Käynnistettäessä;
        public Action Pysäytettäessä;
        
        public void Laukaise(Morte peli)
        {
            Käynnistettäessä?.Invoke();

        }

        public void Pysäytä()
        {
            Pysäytettäessä?.Invoke();

        }
    }
}
