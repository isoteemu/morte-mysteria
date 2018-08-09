using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Jypeli;

using Newtonsoft.Json;

namespace Morte
{
    /// <summary>
    /// Kyselee scores.php tiedostolta tuloslistausta.
    /// </summary>
    /// <remarks>
    /// Itse kysely suoritetaan vasta seuraavalla päivityskerralla. Jos kysely halutaan suorittaa välittömästi, voidaan kutsua:
    /// <c>ScorePHPYhteys.Kysele();</c>
    /// </remarks>
    public class ScoresPHP
    {
        /// <summary>
        /// scores.php scriptin sijainti.
        /// </summary>
        public Uri URI { get; set; }

        public string Token { get; set; }

        public int Määrä = 5;
        public Version Versio;

        const string TOIMINTO_PARHAAT = "top";
        const string TOIMINTO_LISÄÄ = "add";
        const string TOIMINTO_SIJOITUS = "rank";

        const string KENTTÄ_NIMI = "n";
        const string KENTTÄ_TULOS = "s";
        const string KENTTÄ_TOIMINTO = "m";
        const string KENTTÄ_MÄÄRÄ = "l";
        const string KENTTÄ_VERSIO = "v";
        const string KENTTÄ_TOKEN = "t";

        const int SUURIN_INT = 2147483647;

        protected string UserAgent { get; set; }

        public ScoresPHP(String URL)
        {
            URI = new Uri(URL);

            Assembly assembly = Assembly.GetExecutingAssembly();
            string product = (assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0] as AssemblyProductAttribute).Product;
            string version = (assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0] as AssemblyFileVersionAttribute).Version;
            UserAgent = String.Format("{0}/{1}", product, version);

        }

        /// <summary>
        /// Lisää tulos tulospalveluun. 
        /// </summary>
        /// <param name="nimi"></param>
        /// <param name="tulos"></param>
        /// <param name="TulosCallback"></param>
        /// <param name="FailCallback"></param>
        public ScorePHPYhteys LuoLisääTulosKysely(string nimi, double tulos)
        {
            var yhteys = LuoYhteys();
            NameValueCollection data = new NameValueCollection
            {
                { KENTTÄ_NIMI, nimi },
                { KENTTÄ_TULOS, tulos.ToString() },
                { KENTTÄ_TOIMINTO, TOIMINTO_LISÄÄ },
                { KENTTÄ_MÄÄRÄ, Määrä.ToString() }
            };
            if (Versio != null)
                data.Add(KENTTÄ_VERSIO, Versio.ToString());
            if (Token != null)
                data.Add(KENTTÄ_TOKEN, LuoToken(data));

            yhteys.Data = data;

            Morte.DoNextUpdate(() => yhteys.Kysele());
            return yhteys;

        }

        public ScorePHPYhteys LuoSijoitusKysely(double tulos)
        {
            var yhteys = LuoYhteys();
            NameValueCollection data = new NameValueCollection
            {
                { KENTTÄ_TULOS, tulos.ToString() },
                { KENTTÄ_TOIMINTO, (tulos > 0) ? TOIMINTO_SIJOITUS : TOIMINTO_PARHAAT },
                { KENTTÄ_MÄÄRÄ, Määrä.ToString() }
            };

            if (Versio != null)
                data.Add(KENTTÄ_VERSIO, Versio.ToString());
            if (Token != null)
                data.Add(KENTTÄ_TOKEN, LuoToken(data));

            yhteys.Data = data;

            return yhteys;

        }

        protected ScorePHPYhteys LuoYhteys()
        {
            ScorePHPYhteys yhteys = new ScorePHPYhteys();
            yhteys.Headers.Add("User-Agent", UserAgent);
            yhteys.URI = URI;

            return yhteys;
        }

        /// <summary>
        ///  Salli vain järkevät arvot ScoreItemille.
        /// </summary>
        /// <param name="tulos"></param>
        /// <returns></returns>
        public static ScoreItem ValidoiScoreItem(ScoreItem tulos)
        {
            tulos.Position = (int)Math.Max(0, Math.Min(tulos.Position, SUURIN_INT));
            tulos.Score = (int)Math.Max(0, Math.Min(tulos.Score, SUURIN_INT));

            // Poista merkit joita ei voida tulostaa.
            string name = "";
            for(int i = 0; i < tulos.Name.Length; i++)
            {
                char merkki = tulos.Name[i];
                int bitti = (int)merkki;
                if (bitti < 33 || bitti > 253) continue;
                if (char.IsControl(merkki)) continue;

                name += merkki;
            }
            tulos.Name = name;

            return tulos;
        }

        public string LuoToken(NameValueCollection data)
        {
            if (Token == null) return "";

            return Md5Sum(Token + "|" + data[KENTTÄ_NIMI] + "|" + data[KENTTÄ_TULOS]);
        }

        /// <summary>
        /// Laskee MD5 Summan PHP yhteensopivasti.
        /// Alkuperäinen tekijä: Matthew Wegner https://wiki.unity3d.com/index.php?title=MD5
        /// </summary>
        /// <param name="strToEncrypt"></param>
        /// <returns></returns>
        protected static string Md5Sum(string strToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }
    }

    public class ScorePHPYhteys : WebClient
    {
        public Action<List<ScoreItem>> Suoritettu;
        public Action Epäonnistui;

        public NameValueCollection Data { get; set; }
        public Uri URI { get; set; }

        /// <summary>
        /// Seuraa onko kysely jo suoritettavana.
        /// </summary>
        protected bool _Kyselee { get; set; }

        public ScorePHPYhteys()
        {
            /// Luo tapahtuma datan saantiin.
            UploadValuesCompletedEventHandler p = delegate (object res, UploadValuesCompletedEventArgs args) {

                if (args.Cancelled)
                {
                    Debug.WriteLine("Saatiin ScorePHP dataa, mutta yhteys on peruttu.");

                    /// Tapahtuma EI invokea epäonnistumista.
                    return;
                }
                else if (args.Error != null)
                {
                    Debug.WriteLine("Score.php datan latauksessa virhe: {0}", args.Error);
                    Epäonnistui?.Invoke();
                    return;
                }
                else
                {
                    try
                    {
                        String tulosdata = Encoding.UTF8.GetString(args.Result);
                        Debug.WriteLine("Saatu data: {0} ", tulosdata);
                        List<ScoreItem> r = JsonConvert.DeserializeObject<List<ScoreItem>>(tulosdata);

                        Suoritettu?.Invoke(r);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Virhe purettaessa tulosdataa: " + e.Message);
                        Epäonnistui?.Invoke();
                    }
                }
            };

            UploadValuesCompleted += new UploadValuesCompletedEventHandler(p);

        }

        public void Kysele()
        {
            if (_Kyselee) return;

            _Kyselee = true;
            UploadValuesAsync(URI, Data);
        }

        public void Peruuta()
        {
            CancelAsync();

        }
    }
}
