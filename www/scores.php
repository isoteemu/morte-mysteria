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



$TOIMINTO = [
    'TULOS' => 0,
    'SIJOITUS' => 1,
    'PARHAAT' => 2
];

$db = function() {
    $dsn = getenv('MYSQL_DSN');
    $user = getenv('MYSQL_USER');
    $password = getenv('MYSQL_PASSWORD');

    return new PDO($dns, $user, $password);
};
/**
 * Lisää tulos. Palauttaa lisätyn rivin.
 */
function lisää_score(string $name, int $score) {
    // TODO: Lisää versio
    global $db;

    echo("/* Lisätään uusi*/");
    $i = $db->prepare("INSERT INTO Scores (name, score,) VALUES (:name, :score)");
    $i->bindValue(":name", $name);
    $i->bindValue(":score", $score);
    $i->execute();

    $r = $db->prepare("
        SELECT *
        FROM Scores
        WHERE id >= :id
    ");
    $r->bindValue(":id", $db->lastInsertId());

    return $r->execute()->fetchArray()[0];
}

function hae_tulokset() {
    global $db;

    $r = $db->query("SELECT * from Scores ORDER BY score DESC, time ASC");

    $scores = array();
    $i = 1;

    while ($row = $r->fetchArray()) {
            $scores[] = array(
                    "Position" => $i,
                    "Name" => $row['name'],
                    "Score" => $row['score']
            );
            $i++;
    }

}

function hae_sijoitus($score) {
    $r = $db->query("
        SELECT COUNT(*)
        FROM Scores
        WHERE score >= :score
    ");

    return $r->fetchArray()[0];

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

function hae_tulos(int $sijoitus, $aikaleima) {
    global $db;
    $q = $db->prepare("SELECT * FROM scores WHERE ts >= :ts ORDER BY score DESC LIMIT :offset, 1");
    $q->bindValue(":ts", $aikaleima);
    $q->bindValue(":offset", $sijoitus - 1);
}


$data = array();

$operaatiot = $_REQUEST['m'];
foreach($operaatiot as $op) {
    switch($op) {
        case $TOIMINTO['TULOS'] :

    }
}

$odotettu_hash = md5($salainen_avain+"|"+@$_REQUEST['name']+"|"+@$_REQUEST['score']);


header("Content-Type: application/json");


/*
$expected_hash = md5($username . $score . $privateKey);
if($expected_hash == $hash) {
*/
#var_dump($scores);
echo(json_encode($scores));


