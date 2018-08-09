using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Jypeli;
using Newtonsoft.Json;

/**
 * ScoreItem Tulospalvelu.Add("nimimerkki", 50);
 *
 **/

    /*
namespace Morte
{

    /// <summary>
    /// Vastaa keskustelusta scores.php scriptin kanssa.
    /// </summary>
    public class Tulospalvelu
    {
        public string Avain { get; set; }
        public string URL { get; set; }
        protected Version Versio = new Version(1, 0);

        protected TulosLista Välimuisti;

        protected WebClient yhteys;

        /// <summary>
        /// <c>TULOS</c> hakee sijoituksella, <c>SIJOITUS</c> pisteillä
        /// </summary>
        public enum TOIMINTO : int {
            TULOS = 0x00, SIJOITUS = 0x01, PARHAAT = 0x02
        };


        public Tulospalvelu(string avain, string url)
        {
            Avain = avain;
            URL = url;
        }
        
        public ScoreItem Add(string nimimerkki, string pisteet)
        {
            WebClient lataaja = new WebClient();

            var hash = Md5Sum(Avain + "|" + nimimerkki + "|" + pisteet);

            NameValueCollection tulos = new NameValueCollection
            {
                { "name", nimimerkki },
                { "score", pisteet },
                { "v", Versio.ToString() },
                { "m", "add" },
                { "hash", hash },
            };

            lataaja.UploadValues(URL, tulos);
        }


        public ScoreItem Tulos(int sijoitus)
        {
            return new ScoreItem();
        }
        

        public TulosLista Tulokset()
        {
            return
        }
    

        protected WebClient Kysely(TOIMINTO toiminto)
        {
            return 
        }

    }

    public class TulosLista : INotifyList<ScoreItem>
    {
        protected string Aikaleima;
        public event Action Changed;

        protected ScoreItem[] _tulokset;

        protected Tulospalvelu Tulospalvelu { get; set; }

        public TulosLista(Tulospalvelu tulospalvelu)
        {
            Tulospalvelu = tulospalvelu;
        }

        public ScoreItem this[int position]
        {
            get {
                return _tulokset[position - 1];
            }
            set
            {
                for (int i = _tulokset.Length - 1; i > position - 1; i--)
                    _tulokset[i] = _tulokset[i - 1];
                _tulokset[position - 1] = value;

                for (int i = 0; i < _tulokset.Length; i++)
                    _tulokset[i].Position = i + 1;

                OnChanged();
            }
        }

        private void OnChanged()
        {
            Changed?.Invoke();
        }

        public ScoreItem HaeTulos(int sijoitus)
        {
            var json = Tulospalvelu.Kysely(Tulospalvelu.TOIMINTO.TULOS);
            return JsonConvert.DeserializeObject<ScoreItem>(json);

        }

        #region ENumerator
        public int Count
        {
            get { return _tulokset.Length; }
        }

        public IEnumerator<ScoreItem> GetEnumerator()
        {
            foreach (ScoreItem item in _tulokset)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    
}
*/
