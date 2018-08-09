<?php

$salainen_avain = 'Make like a 3, and get out of here!';

/*
MySQL [morte]> DESCRIBE Scores;
+-------+-----------------+------+-----+-------------------+----------------+
| Field | Type            | Null | Key | Default           | Extra          |
+-------+-----------------+------+-----+-------------------+----------------+
| id    | int(8) unsigned | NO   | PRI | NULL              | auto_increment |
| name  | varchar(32)     | NO   |     | NULL              |                |
| score | int(5) unsigned | NO   |     | 0                 |                |
| ts    | timestamp       | NO   |     | CURRENT_TIMESTAMP |                |
+-------+-----------------+------+-----+-------------------+----------------+

MySQL [morte]> select * from Scores;
+----+-----------+-------+---------------------+
| id | name      | score | ts                  |
+----+-----------+-------+---------------------+
|  1 | EKA!!111! |   666 | 2018-04-09 02:40:28 |
+----+-----------+-------+---------------------+

scores.php?n=foo&s=score&m=


*/

define('SIJOITUSTEN_MÄÄRÄ', 5);

define('TOIMINTO_PARHAAT',  'top');
define('TOIMINTO_LISÄÄ',    'add');
define('TOIMINTO_SIJOITUS', 'rank');

define('KENTTÄ_NIMI', 'n');
define('KENTTÄ_TULOS', 's');
define('KENTTÄ_TOIMINTO', 'm');
define('KENTTÄ_MÄÄRÄ', 'l');
define('KENTTÄ_VERSIO', 'v'); // Ei käytetty
define('KENTTÄ_TOKEN', 't');  

if($_SERVER['SERVER_NAME'] == 'localhost')
    $dsn = 'mysql:host=localhost;dbname=morte';
else
    $dsn = getenv('MYSQL_DSN');

$db_user = getenv('MYSQL_USER');
$db_password = getenv('MYSQL_PASSWORD');

$db = new PDO($dsn, $db_user, $db_password);


/**
 * Lisää tulos. Palauttaa lisätyn rivin.
 */
function lisää_tulos($name, $score) {
    // TODO: Lisää versio
    global $db;


    $i = $db->prepare("INSERT INTO Scores (name, score) VALUES (:name, :score)");
    $i->bindValue(":name", $name);
    $i->bindValue(":score", $score, PDO::PARAM_INT);

    $i->execute() or die("/* ".$i->errorInfo()[2]." */");

    $r = $db->prepare("
        SELECT *, (
            SELECT COUNT(*)
            FROM Scores
            WHERE score > :score
            ) + 1 AS sijoitus
        FROM Scores
        WHERE id = :id
    ");
    $r->bindValue(":id", $db->lastInsertId(), PDO::PARAM_INT);
    $r->bindValue(":score", $score, PDO::PARAM_INT);
    $r->execute() or die("/* ".$r->errorInfo()[2]." */");
    return $r->fetch();

}

/**
 * Hae leaderbords
 */
function hae_tulokset($limit = SIJOITUSTEN_MÄÄRÄ, $sijoitus = 0, $ts = null) {
    global $db;

    $limit = max(0, $limit);

    // TODO: Olisi fiksua myös tarkistaa, ettei sijoitus ole yli mahdollisen sijoituksen.
    $offset = max(0, $sijoitus - floor($limit/2) - 1);

    $prepare_ts = ($ts == null) ? 'NOW()' : ':ts';

    $q = $db->prepare("
        SELECT *
        FROM Scores
        WHERE ts <= $prepare_ts
        ORDER BY score DESC, ts ASC
        LIMIT :from, :limit");

    if($ts != null)
        $q->bindValue(":ts", $ts);

    $q->bindValue(":from", $offset, PDO::PARAM_INT);
    $q->bindValue(":limit", $limit, PDO::PARAM_INT);

    $q->execute();
    
    $scores = array();
    $i = $offset+1;

    while ($row = $q->fetch()) {
        $scores[] = array(
            "Position" => $i,
            "Name" => $row['name'],
            "Score" => intval($row['score'])
        );
        $i++;
    }

    return $scores;

}

function hae_sijoitus($score) {
    global $db;
    $q = $db->prepare("
        SELECT COUNT(*)
        FROM Scores
        WHERE score > :score
    ");
    $q->execute(array(":score" => $score));

    return $q->fetch()[0];

    /*
    SELECT  uo.*,
    (
    SELECT  COUNT(*)
    FROM    Scores ui
    WHERE   (ui.score, -ui.ts) >= (uo.score, -uo.ts)
    ) AS rank
    FROM    Scores uo
    WHERE   name = '$name';
    */
}

function tarkista_avain() {
    $dotettu_hash = md5($salainen_avain+"|"+@$_REQUEST[KENTTÄ_NIMI]+"|"+@$_REQUEST[KENTTÄ_TULOS]);

    if($odotettu_hash == $_REQUEST['t'])
        return true;
    else
        return false;
    
}

///
/// * MAIN
///

$data = array();

// Hae operaatiot
$op = $_REQUEST[KENTTÄ_TOIMINTO];

switch($op) {
    case TOIMINTO_PARHAAT:
        $data += hae_tulokset(SIJOITUSTEN_MÄÄRÄ);
        break;

    case TOIMINTO_LISÄÄ:
        // Tallennetaan vain 8bit merkistö, jypeli tukee hyyyyvin rajoittuneesti merkistöjä.
        $nimi = (string) $_REQUEST[KENTTÄ_NIMI];
        $nimi = preg_replace('/[\x00-\x1F\x7F]/', '', $nimi);

        // Rajoita tulos fiksuihin lukuihin. Max on mysql:n Integer -maksimi.
        $tulos = (int) $_REQUEST[KENTTÄ_TULOS];

        if($tulos <= 0) die("/* Virheellinen tulos */");
        $tulos = max(0,  $tulos);
        $tulos = min($tulos, 2147483647);

        $tulos_data = lisää_tulos($nimi, $tulos);

        $data += hae_tulokset(
            SIJOITUSTEN_MÄÄRÄ,
            $tulos_data['sijoitus'],
            $tulos_data['ts']
        );

        break;

    case TOIMINTO_SIJOITUS:

        $tulos = (int) $_REQUEST[KENTTÄ_TULOS];
        $sijoitus = hae_sijoitus($tulos);

        $data += hae_tulokset(
            SIJOITUSTEN_MÄÄRÄ,
            $sijoitus
        );
        break;

    default:
        $data += array("/* En tiedä mitä haluat minusta: $op */");
}


header("Content-Type: application/json");

echo(json_encode($data));


