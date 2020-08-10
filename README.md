# Morte Mysteria dom Domine de Daemonium

``C#`` peli toteutettu osana Jyväskylän Yliopiston Ohjelmointi 1 -kurssia. Pelimoottorina toimii [Jypeli -kirjasto](https://github.com/Jypeli-JYU/Jypeli).

![Screeshot](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Assets/Capture.png).

Peli on alunperin toteutettu Macromedia Flash -pelinä 2002, osana Audio-visuaalisen viestinnän koulutusta. Ja täten "kärsii" ajalle tyypillisistä piirteistä, kuten triggerhappy hiirilookista, yliampuvasta veriroiskeesta, ja pienestä peli-ikkunasta. Erityismaininta Tapio Puolimatkalle, jonka ysäripelottelut satanisteilla toimivat vahvana motivaationa teemalle.

Pelissä pappi on joutunut helvettiin ehtoollisviinat juotuaan, ja joutuu selviytymään alati kasvavia vihulaislaumoja vastaan. Alkuperäisinä tekijöinä Ville Hakonen, Teemu Hirvonen ja Teemu Autto. Jypeli-versiosta vastaa viimeksi mainittu.

## Nuorten peliohjelmointikurssin suorittajien kannalta kiinnostavia piirteitä

Alkunäyttö; Lootboxi; Oma hiiren kursori; Dynaaminen musiikki; Verkossa olevat tuloslistat; Tweenausta ja dynaamista sheidausta; Partikkeliveri; Paussi;

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

- Risti joka muuttuu ristiksi väärinpäin pelaajan teveyden vähentyessä [``morte / Morte / Wädgetti / Risti.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/W%C3%A4dgetti/Risti.cs). Risti kahdesta objektista, pystypalkista joka on jypelin `Widget`, ja lapsielementistä joka muodostaa vaakapalkin. Luokka seuraa sidotun öggiäisen teveyttä, ja liikuttaa vaakapalkkia suhteessa terveydentilaan.

- Paussiruutu [``morte / morte.cs[861]``](https://github.com/isoteemu/morte-mysteria/blob/74f962fe895cd5d24a3902c62763083d47e275aa/morte/morte.cs#L861). Jypeli itsessään tarjoaa helpon paussi-toiminnallisuuden, mutta dokumentoinnin ollessa puuttellista ja "jypeli paussi" ollessa googlatuimpia jypeli -termejä sen käyttö toimii näin helposti: `IsPaused = True;`. `IsPaused` attribuutin ollessa `True`, jypeli päivittää ainoastaan Widget -elementtejä. Pausen toteuttaminen `[ESC]` näppäintä painettaessa:
    ```c#
    public override void Begin()
        // Liitä Paussi funktion kutsu näppäimen painallukseen.
        Keyboard.Listen(Key.Escape, ButtonState.Released, Paussi, "Paussita peli");

        // [--]
    }

    protected void Paussi() {
        // Vaihtaa arvon käänteiseksi. True -> False ja False -> True.
        IsPaused = !IsPaused;

        if (IsPaused) {
            // Peli on paussilla, luo valikko, whatnot
            MessageDisplay.Add("Peli on paussilla");
        } else {
            // Paussista poistutaan. tuhoa valikko, whatnot
            MessageDisplay.Add("Peli jatkuu.");
        }
    }
    ```

## Lootitemeitä
    
| Esine | Toteutus |
| ----- | -------- |
| ![Kranaatti](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Content/kranaatti.png) [``morte / Morte / Aseet / Kranaatti.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/Aseet/Kranaatti.cs) | *Bring out the holy handgranade!* Oletus-erikoisase. Käyttö vapaata, mutta vie pelaajan terveyttä. Käyttää jypelin tarjoamaan `Jypeli.Grenade` luokkaa ja `Jypeli.Explosion` -tehostetta. Laukaisu ei tapahdu pelaajan suuntaan, vaan hiiren suuntaan. |
| ![Hattu](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Content/loot/Hattu/sprite.png) [``morte / Morte / Loot / Hattu.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/Loot/Hattu.cs) | Hattu on yksinkertaisimpia esineitä. Objekti tarkkailee sidotun hahmon sijaintia, ja päivittää omaansa vastaavaan [``morte / Morte / FX / FX.cs[42]``](https://github.com/isoteemu/morte-mysteria/blob/994a91cd2486e14971001e063660d469aeb6a280/morte/Morte/FX/FX.cs#L42). Näin siksi, että Jypelissä on mahdollista GameObjektiin lisätä lapsia **yhdellä** tasolla, muttei usemalla puumaisesti. Hattua klikattaessa sidos irrotettaan, asetetaan liikevektori / impulssi, ja elementille annetaan ääretön massa. Näin vihuun osuessaan kohtaa vihu pysäyttämättömän voiman. |
| ![Kannabis](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Content/loot/Kannabis/sprite.png) [``morte / Morte / Loot / Kannabis.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/Loot/Kannabis.cs) | Kuten hattu, sidotaan kannabis hahmon sijaintiin. Painettaessa välilyöntiä luodaan joukko GameObject partikkeleita, joille määritetään pienellä varianssilla suunta, ja suunnan perusteella väri [``morte / Morte / Aseet / Oksennus.cs[45]``](https://github.com/isoteemu/morte-mysteria/blob/994a91cd2486e14971001e063660d469aeb6a280/morte/Morte/Aseet/Oksennus.cs#L45). Partikkelin sävy – hue – on valittu sateenkaaren spektrillä, jolloin partikkelit ampaistessaan hahmon suusta luovat sateenkaaren. Osuessaan vihulaiseen partikkeli vahingoittaa sitä. Kannabikselle on määritelty myös "energia", jonka loppuessa aseen käyttö estetään, ja jointti "tumpataan" maahan. |
| ![Moottorisaha](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Content/loot/Saha/sprite.png) [``morte / Morte / Loot / Saha.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/Loot/Saha.cs) | Moottorisaha seuraa laiskasti pelaajan hiirtä. Liikkeelle on määritetty maksiminopeus, ja maksimietäisyys hahmosta [``morte / Morte / Loot / saha.cs[215]``](https://github.com/isoteemu/morte-mysteria/blob/994a91cd2486e14971001e063660d469aeb6a280/morte/Morte/Loot/Saha.cs#L215). Kuten kannabis, on sahalla oma "energia-taso" – löpö –, joka kuluu aseen käytössä. Osuessaan vihuun saha aiheittaa vahinkoa vihuun, ja luo partikkeliverta vihun ja sahan ristileikkauspisteeseen [``morte / Morte / Loot / Saha.cs[147]``](https://github.com/isoteemu/morte-mysteria/blob/994a91cd2486e14971001e063660d469aeb6a280/morte/Morte/Loot/Saha.cs#L147) – **EI** todelliseen osumakohtaan, koska sahan hitbox on vain neliö. Käytettäessä ase tärisee, ja sille annetaan kasvatettu massa. Tämä saa vihun sysäytymään taaksepäin, jonka vuoksi seuraavalla framella vihun hyökkiessä sahaa kohden tapahtuu uusi ``CollisionHandler`` -tapahtuman. Keino kiertää, ettei tarvitse jokaisella pelin päivityskerralla tarkistaa, vieläkö saha osuu vihuu vai ei. |
| ![Sieni](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Content/loot/Sieni/sprite.png) [``morte / Morte / Loot / Sieni.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/Loot/Sieni.cs) | Sieni saa hahmon kasvamaan. Erikoisuutena lootitemille on, että sieni pyrkii laatikosta paljastuessaan pelaajaa karkuun [``morte / Morte / Loot / Sieni.cs[41]``](https://github.com/isoteemu/morte-mysteria/blob/994a91cd2486e14971001e063660d469aeb6a280/morte/Morte/Loot/Sieni.cs#L41). Poimiessaan sienen hahmo skaalataan kaksinkertaiseksi käyttäen tweenausta [``morte / Morte / Loot / Sieni.cs[68]``](https://github.com/isoteemu/morte-mysteria/blob/994a91cd2486e14971001e063660d469aeb6a280/morte/Morte/Loot/Sieni.cs#L68) |
| ![Viini](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Content/loot/Viini/sprite.png) [``morte / Morte / Loot / Viini.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/Loot/Viini.cs) | Jos pelin premissi on joutua helvettiin ehtoollisviinien juonnista, ehtoollisviinin juonnista helvetissä joutuu helvettiin - uudestaan. Alkunäytöt kuitenkin opettavat, että pelaaja ehti nukkua pahimman päihtymyksen alta pois, jolle nyt ei kuitenkaan jää aikaa. Poimittuaan viinin, hahmo "kuolee", hitpointit palautetaan oletukseksi, ja hahmo pudotetaan uudestaan tasolle. Kuoleman kohdalle nousee hautakivi [``morte / Morte / Loot / Viini.cs[66]``](https://github.com/isoteemu/morte-mysteria/blob/994a91cd2486e14971001e063660d469aeb6a280/morte/Morte/Loot/Viini.cs#L66). Jotuaan viinin hahmon muoto vaihdetaan soikeaksi (``Jypeli.Shape.Circle``). Johtuen miten hahmoa liikutetaan impulssein, oletusmuotoinen hahmo (``Jypeli.Shape.Hexagon``) pysyy liikkuessaan helposti pystyssä, mutta soikean muotoinen hahmo ei. Jokaiseen öggiäiseen on liitetty oikaisumekanismi, joka ohjaa öggiäisen liike-energiaa liikkumisen sijasta oikaisuun [``morte / Morte / Öggiäinen.cs[149]``](https://github.com/isoteemu/morte-mysteria/blob/994a91cd2486e14971001e063660d469aeb6a280/morte/Morte/%C3%96ggi%C3%A4inen.cs#L149). |

## Vihulaisia

Vihulaiset perivät [``Vihulainen``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/%C3%96ggi%C3%A4iset/Vihulainen.cs) luokan. Luokka määrittelee joitain yhteisiä vakioita, kuten Jypelin  `CollisionIgnoreGroup`. Tämä estää ettei vihulaiset törmäile toisiinsa, mutta reagoivat maailman muihin objekteihin, kuten lootboxeihin ja pelaajan hahmoon. Luokka myös määrittelee vihuille yhteisiä arvoja, kuten hitpointit, oletusliikkumiset hahmoa kohden / hyökkimisloopin ja kuolinkäsittelijän.

Vihulaiset tietävät suuntansa `Vihulainen.Suunta` attribuutilla [``morte / Morte / Öggiäinen.cs[63]``](https://github.com/isoteemu/morte-mysteria/blob/3aef9654884a4ca1be9471a66f284dd0f8f7d6e3/morte/Morte/%C3%96ggi%C3%A4inen.cs#L63). Attribuutin vaihtuessa grafiikat peilataan (`Jypeli.GameObject.MirrorImage()`). Jokaisen hyökkimisloopin alkaessa vihulainen tarkastaa oman sijaintinsa suhteessa pelaajaan, ja asettaa `Suunta` attribuutin joko `Morte.VASEN` (`= -1`) tai `Morte.OIKEA` (`= 1`) [``morte / Morte / Öggiäiset / Vihulainen.cs[149]``](https://github.com/isoteemu/morte-mysteria/blob/3aef9654884a4ca1be9471a66f284dd0f8f7d6e3/morte/Morte/%C3%96ggi%C3%A4iset/Vihulainen.cs#L149). Tätä attribuuttia käytetään liikevektoreiden laskentaa (``X * Suunta => X * -1 tai X * 1``), jolloin vihulaisen ei tarvitse määritellä tarkemmin suunnatakko vasemmalle vai oikealle.

| Vihulainen | Toteutus |
| ---------- | -------- |
| ![Hullu](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Content/vihulaiset/hullu0001.png) [``morte / Morte / Öggiäiset / Hullu.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/%C3%96ggi%C3%A4iset/Hullu.cs) | Hullu on pelin voimakkain vihulainen. Vihulle on annettu suuri massa, suuri määrä hitpointteja mutta hidas liikkumisnopeus. Animaatio on 3 kuvaa, joita vaihdellaan ``Animation`` -luokalla [``morte / Morte / Öggiäiset / Hullu.cs[29]``](https://github.com/isoteemu/morte-mysteria/blob/3aef9654884a4ca1be9471a66f284dd0f8f7d6e3/morte/Morte/%C3%96ggi%C3%A4iset/Hullu.cs#L29) |
| ![Koura](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Content/vihulaiset/koura-auki.png) [``morte / Morte / Öggiäiset / Koura.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/%C3%96ggi%C3%A4iset/Koura.cs) | Koura nousee maasta satunnaisista kohteista, ja pyrkii tarraamaan pelaajaan [``morte / Morte / Öggiäiset / Koura.cs[45]``](https://github.com/isoteemu/morte-mysteria/blob/3aef9654884a4ca1be9471a66f284dd0f8f7d6e3/morte/Morte/%C3%96ggi%C3%A4iset/Koura.cs#L45). Osuessaan pelaajaan - joko noustessaan sopivasta kohden, tai pelajaan kävellessä kouraan - koura vaihtaa animaatioframensa, pysäyttää oman hyökkimislooppinsa, ja estää pelaajan liikkumisen kasvattamalla pelaajan massaa ettei liikkumiseen käytetty liike-energia riitä enää liikuttamaan pelaajaa [``morte / Morte / Öggiäiset / Koura.cs[86]``](https://github.com/isoteemu/morte-mysteria/blob/3aef9654884a4ca1be9471a66f284dd0f8f7d6e3/morte/Morte/%C3%96ggi%C3%A4iset/Koura.cs#L86). Kuollessaan koura implementoi oman kuolinanimaation, ja vähentää pelaajan massaa lisäämänsä määrän verran. |
| ![Mato](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Content/vihulaiset/mato0001.png) [``morte / Morte / Öggiäiset / Käärme.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/%C3%96ggi%C3%A4iset/K%C3%A4%C3%A4rme.cs) | Mato on simppeli vihulainen, mutta erona perusvihuun, käärmeellä on oma liikkumistoteutus. Tasaisen liikkeen sijasta liikkuu nykähdellen kohti pelaajan hahmoa ajoittaisilla impulsseilla [``morte / Morte / Öggiäiset / Käärme.cs[37]``](https://github.com/isoteemu/morte-mysteria/blob/3aef9654884a4ca1be9471a66f284dd0f8f7d6e3/morte/Morte/%C3%96ggi%C3%A4iset/K%C3%A4%C3%A4rme.cs#L37). |
| ![Lokki](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Content/vihulaiset/Lokki/lokki0001.png) [``morte / Morte / Öggiäiset / Lokki.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/%C3%96ggi%C3%A4iset/Lokki.cs) | Lokki lentää taivaalla, ja koostuu lukuisista animaatioframeista siipien liikkeen aikaansaamiseksi. Lokki lisäksi räpsyttää silmiään, joka on toteutettu lisäämällä GameObjectille lapsiobjektina "silmäluomet". Satunnaisten ajanjakson kulttua (0.5 - 3.0s) silmäluomi joko näytetään, tai piilotetaan [``morte / Morte / Öggiäiset / Lokki.cs[75]``](https://github.com/isoteemu/morte-mysteria/blob/3aef9654884a4ca1be9471a66f284dd0f8f7d6e3/morte/Morte/%C3%96ggi%C3%A4iset/Lokki.cs#L75). Liikkumiseen lokki implementoi oman tarkkailijan, joka seuraa onko lokki alle määrätyn korkeuden. Jos on, isketään lokkia uudella yläviistoon kohdistuvalla liikevektorilla, joka yhdessä painovoiman kanssa saa lokin syöksymään kaaressa kohden pelaajan hahmoa [``morte / Morte / Öggiäiset / Lokki.cs[93]``](https://github.com/isoteemu/morte-mysteria/blob/3aef9654884a4ca1be9471a66f284dd0f8f7d6e3/morte/Morte/%C3%96ggi%C3%A4iset/Lokki.cs#L93). |
| ![Pappi](https://raw.githubusercontent.com/isoteemu/morte-mysteria/master/morte/Content/pappi.png) [``morte / Morte / Öggiäiset / Pappi.cs``](https://github.com/isoteemu/morte-mysteria/blob/master/morte/Morte/%C3%96ggi%C3%A4iset/Pappi.cs) | Kukapa ei toimisi itselleen vahingollisesti. Pelaajan hahmo. Hahmolle luodaan lapsi-elementtinä silmät, jotka käyttäen trigonomtriaa (``atan2``) pyöritetään katsomaan hiiren osoitinta [``morte / Morte / Öggiäiset / Pappi.cs[46]``](https://github.com/isoteemu/morte-mysteria/blob/994a91cd2486e14971001e063660d469aeb6a280/morte/Morte/%C3%96ggi%C3%A4iset/Pappi.cs#L46). Asioiden yksinkertaistamiseksi papin keskilinja menee suoraan nenän kohdalta, joten silmien kohdistuksessa voidaan laskutoimitukset vain heijastaa X-akselilla (``VasenSilmä.X = OikeaSilmä.X * -1``).
