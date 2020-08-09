# Morte Mysteria dom Domine de Daemonium

``C#`` peli toteutettu osana Jyväskylän Yliopiston Ohjelmointi 1 -kurssia. Pelimoottorina toimii [Jypeli -kirjasto](https://github.com/Jypeli-JYU/Jypeli).

![Screeshot](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Assets/Capture.png).

Peli on alunperin toteutettu Macromedia Flash -pelinä 2002, osana Audio-visuaalisen viestinnän koulutusta. Ja täten "kärsii" ajalle tyypillisistä piirteistä, kuten triggerhappy hiirilookista, ja yliampuvasta veriroiskeesta.

Pelissä pappi on joutunut helvettiin ehtoollisviinat juotuaan, ja joutuu selviytymään alati kasvavia vihulaislaumoja vastaan. Alkuperäisinä tekijöinä Ville Hakonen, Teemu Hirvonen ja Teemu Autto. Jypeli-versiosta vastaa viimeksi mainittu.

## Nuorten peliohjelmointikurssin suorittajien kannalta kiinnostavia piirteitä

Alkunäyttö; Lootboxi; Oma hiiren kursori; Dynaaminen musiikki; Verkossa olevat tuloslistat; Tweenausta ja dynaamista sheidausta; Partikkeliveri;

 - Pelin alkunäytöt ovat windows media file tiedosto, joka piirrettään XNAlla jypeli objektin tekstuuriin. [``Morte / Wädgetti / VideoWädgetti.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/W%C3%A4dgetti/VideoW%C3%A4dgetti.cs). Video-elemettejä voidaan käyttää kuten mitä tahansa jypeli-elementtiä, mutta kuva on korvattu videolla.

    Esimerkki alkunäytöistä [``Morte / morte.cs[247]``](https://github.com/isoteemu/morte-mysteria/blob/74f962fe895cd5d24a3902c62763083d47e275aa/morte/morte.cs#L478):
    ```c#
    void AlkuNäyttö() {
        // Luo näytön kokoinen videoelementti, kerro mikä on assetin nimi (intro)
        IntroVideo = new VideoWädgetti(Screen.Width, Screen.Height)
        {
            VideoTiedosto = "intro",
        };
        // Siirrä kamera videon kohdalle
        IntroVideo.Y = Screen.Top + IntroVideo.Height / 2;
        Camera.Position = IntroVideo.Position;

        // Funktio joka suoritetaan videon päättyessä.
        IntroVideo.OnStop += LopetaAlkuNäyttö;

        // Lisää elementti peliin
        Add(IntroVideo);
    }

    void LopetaAlkuNäyttö() {
        // Varmistetaan videon pysähtyminen
        IntroVideo.Stop();

        // Tuhotaan videoelementti
        IntroVideo.Destroy();

        // Siirrä kamera oletuskohtaan
        Camera.Y = 0;

        // Käynnistä varsinainen peli, näytä alkuvalikko, whatnot [--]
    }
    ```

    Toteutuksessa alkunäytöt näytetään pelin assettien (grafiikat/musiikit yms.) latautuessa, mutta todellisuudessa alkunäyttöjen latauksessa kestää kauemmin kuin kaiken muun.

 - Verkossa olevat parhaat tulokset. Toteutus on raakile, eikä suositella ilman syvempää ymmärtämistä käytettäväksi. Palvelinpuolen toteutus PHP:lla [``www / scores.php``](https://github.com/isoteemu/morte-mysteria/blob/master/www/scores.php). Tulosten noudosta ja lähetyksestä vastaava kirjasto [``morte / Morte / ScoresPHP.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/ScoresPHP.cs). Esimerkki tulosten noudosta [``morte / morte.cs[914]``](https://github.com/isoteemu/morte-mysteria/blob/74f962fe895cd5d24a3902c62763083d47e275aa/morte/morte.cs#L914)

 - Hiiren piilottaminen ja korvaaminen omalla kursorilla. Jypeli itsessään on tehnyt hiiren piilottamisesta ... *vaikeaa*, ja kursori piilotetaan windowsin kutsuilla. Hiiren sijainti luetaan jokaisella jypelin päivitykerralla, ja sidottu jypelin elementti siirretään vastaavaan paikkaan. Kursorin elementti on omana kirjastona [``morte / Morte / Wädgetti / Kursori.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/W%C3%A4dgetti/Kursori.cs).

 - Dynaaminen musiikki, joka muuttuu pelissä olevien vihujen mukaan [``morte / Morte / Musiikki.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/Musiikki.cs). Musiikit ovat toistuvia, yhtä pitkiä luuppeja, jotka toistetaan riippuen siitä, havaitaanko looppiin liitettyä peliobjektia pelissä.
    ```c#
    public Musiikki() {
        // [--]
        // Lista objekteista joita pelistä etsitään, ja siihen yhdistyvä musiikki. Avaimet/nimet vastaavat _luokan_ nimeä
        Äänitehosteet = new Dictionary<string, SoundEffect> {
                { "Käärme", Game.LoadSoundEffect("music/Käärme") },
                { "Hullu", Game.LoadSoundEffect("music/Hullu") },
                // [--]
            };
    }


    ```

 - Lootboxeja. Lootboxit ovat toteutettu reflektoinnilla, joka mahdollistaa dynaamisen lootboxien lisäämisen. Itse laatikko on simppeli luokka [``morte / Morte / Lootboxi.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/Lootboxi.cs), ja laatikosta paljastuva elementti on [``morte / Morte / Loot / Härpäke.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/Loot/H%C3%A4rp%C3%A4ke.cs), joka joko on klikattava tai poimittava. Lootboxista saatu härpäke taas on jokainen implementoitu sopivassa ``Härpäkeen`` toteuttavassa luokassa. Pelin lootboxien tiputuksesta vastaava koodi löytyy [``morte / morte.cs[762]``](https://github.com/isoteemu/morte-mysteria/blob/74f962fe895cd5d24a3902c62763083d47e275aa/morte/morte.cs#L762). Todennäköisyys elementeille on määritelty tupleissa [``morte / morte.cs[89]``](https://github.com/isoteemu/morte-mysteria/blob/74f962fe895cd5d24a3902c62763083d47e275aa/morte/morte.cs#L89):
    ```c#
        // Ensimmäinen arvo kertoo elementin todennäköisyyden (0-1), toinen lootitemin luokan, joka lisätään arpaonnen osuessa peliin.
        public List<(double, string)> Lootboxit = new List<(double, string)>()
        {
            (0.2, "Morte.Loot.Hattu"),
            (0.1, "Morte.Loot.Viini"),
            (0.1, "Morte.Loot.Sieni"),
            (0.2, "Morte.Loot.Kannabis"),
            (0.05, "Morte.Loot.Saha"),
        };
    ```
    Koska kyseessä on dynaaminen lista, siihen voi pelin suorituksen aikana lisätä ja poistaa elementtejä pay2win tyyppisesti.

 - Tweenaus on keino, jolla voidaan määritellä elementille alkupiste, loppupiste ja aika kuinka kauan siirtymään tulee käyttää. Käyttäen matematiikkaa, siirtymälle voidaan yleensä antaa pehmeä aloitus, pehmeä lopetus, tai yhdistelmä. Pelin jokaisella päivityskerralla (/ framella) elementin välisiirtymäpiste lasketaan automaattisesti, eikä koodarin tarvitse jokaista välipistettä itse määritellä, eikä epätasainen framerate muuta siirtymän nopeutta. Tweenaukseen käytetään [Glide -kirjastoa](https://github.com/jacobalbano/Glide). Morte Mysteriassa on oma PeliObjekti, joka laajentaa Jypelin GameObject luokkaa lisäämällä tweenauksen. 
 
    Esimerkki kameran siirtämisestä [``morte / Morte / Kamerointi.cs``](https://github.com/isoteemu/morte-mysteria/blob/74f962fe895cd5d24a3902c62763083d47e275aa/morte/Morte/Kamerointi.cs#L36), jossa kamera "zoomaa" valittuun kohteeseen sulavasti:

    ```c#
    public void Kohdista(Vector kohde, float kesto=Kamerointi.NORMA, double zoom=2) {
        // [--]
        // Lasketaan kohta johon kamera halutaan zoomattavaksi, ja siirretään Jypelin kameraa tähän pisteeseen tweenaamalla.
        // _x ja _y ovat kameran uudet koordinaatit, ja kesto on kuinka monta sekunttia siirtymiseen käytetään.
        // ease(Ease.QuadIn) kertoo että kohteeseen halutaan saapua pehmeästi, ie. hidastaen lähestyttäessä.
        Tweetteri.Tween(Game.Camera, new { X = _x, Y = _y, ZoomFactor=zoom }, kesto).Ease(Ease.QuadIn);
    }
    ```

    Vaikka koordinaatit ovat luontainen asia tweenata, voidaan mitä tahansa jypelin GameObjektin numeraalista arvoa tweenata. Tästä esimerkkinä [``morte Morte / Öggiäiset / Vihulainen.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/%C3%96ggi%C3%A4iset/Vihulainen.cs), jossa ``Vaaleus`` on arvo 0-1, ja sitä tweenaamalla saadaan vihulaiset muuttumaan sulavasti kummituksiksi.

- Vihulaisia spawnataan peliin oletuksena 7s välein, mutta pelin edetessä vihujen nopea hoitelu pienenetää tätä kestoa mittaamalla kauanko vihu ehti olla olemassa, ja spawnaamalla uuden hieman tätä nopeammin. [``morte / morte.cs[666]``](https://github.com/isoteemu/morte-mysteria/blob/74f962fe895cd5d24a3902c62763083d47e275aa/morte/morte.cs#L666). Helppojen vihujen todennäköisyys spawnauksessa on suurempi kuin vaikeiden. Kuten lootboxeissa, tämä on määritelty listalla ja toteutetaan reflektoinnilla [``morte / morte.cs[682]``](https://github.com/isoteemu/morte-mysteria/blob/74f962fe895cd5d24a3902c62763083d47e275aa/morte/morte.cs#L682).

 - Partikkeliveri. 'nuff said. [``morte / Morte / FX / Bloodenstain.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/FX/Bloodenstain.cs)
    ```c#
    Veriroiske = new Bloodenstain(LoadImage("veripisara"))
    Add(Veriroiske);

    // Luo vihu
    vihu = new GameObject([..])

    // Vihua klikattaessa kutsu parikkeliverta roiskuvaksi hiiren sijainnissa 30 partikkelilla.
    Mouse.ListenOn(vihu, MouseButton.Left, ButtonState.Released, () => Veriroiske.AddEffect(Mouse.PositionOnWorld.X, Mouse.PositionOnWorld.Y, 30));
    ```
