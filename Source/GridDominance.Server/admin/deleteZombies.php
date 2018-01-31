<?php require_once '../internals/backend.php'; ?>
<?php require_once '../internals/utils.php'; ?>
<?php require_once 'common/libadmin.php'; ?>
<?php init("admin"); ?>

<?php

try {

	global $pdo;
	global $config;

	ob_end_flush();

	$stmt = $pdo->prepare("	SELECT * FROM users AS u0 WHERE score=0 AND is_auto_generated=1 AND mpscore=0 AND last_online < NOW() - INTERVAL 100 DAY AND ping_counter<=2 AND 0 = (select count(*) FROM error_log where error_log.userid = u0.userid) AND 0 = (select count(*) FROM cache_levels where cache_levels.best_userid = u0.userid) AND 0 = (select count(*) FROM level_highscores where level_highscores.userid = u0.userid)");
	$stmt->execute();

	$rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

	foreach ($rows as $row) {

		echo "DELETE " . $row['userid'] . "<br/>";

		$stmt = $pdo->prepare("DELETE FROM users WHERE userid = " . $row['userid']);
		$stmt->execute();
	}

} catch (Exception $e) {

	echo $e->getMessage();

}