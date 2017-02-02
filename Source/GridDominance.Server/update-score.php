<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$userid        = getParamIntOrError('userid');
	$password      = getParamStrOrError('password');
	$levelid       = getParamStrOrError('levelid');
	$difficulty    = getParamIntOrError('difficulty');
	$leveltime     = getParamIntOrError('leveltime');
	$totalscore    = getParamIntOrError('totalscore');
	$appversion    = getParamStrOrError('app_version');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $levelid, $difficulty, $leveltime, $totalscore, $appversion]);

	if ($leveltime <= 0)

	//----------

	$user = GDUser::QueryOrFail($pdo, $password, $userid);

	//----------

	$stmt = $pdo->prepare("SELECT best_time FROM level_highscores WHERE userid=:uid AND levelid=:lid");
	$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
	$stmt->bindValue(':lid', $levelid, PDO::PARAM_STR);
	$stmt->execute();
	$dbtime = $stmt->fetchColumn();

	if ($dbtime !== FALSE) {

		if ($dbtime < $leveltime) {
			// better value in db
			outputResultSuccess(['update' => false, 'value_db' => $dbtime, 'user' => $user]);
		}

		// existing row in db
		$stmt = $pdo->prepare("UPDATE level_highscores SET userid=:uid, levelid=:lid, difficulty=:diff, best_time=:time, last_changed=CURRENT_TIMESTAMP()");
		$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
		$stmt->bindValue(':lid', $levelid, PDO::PARAM_STR);
		$stmt->bindValue(':diff', $difficulty, PDO::PARAM_INT);
		$stmt->bindValue(':time', $leveltime, PDO::PARAM_INT);
		$stmt->execute();
	} else {

		// no row in db
		$stmt = $pdo->prepare("INSERT INTO level_highscores (userid, levelid, difficulty, best_time, last_changed) VALUES (:uid, :lid, :diff, :time, CURRENT_TIMESTAMP())");
		$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
		$stmt->bindValue(':lid', $levelid, PDO::PARAM_STR);
		$stmt->bindValue(':diff', $difficulty, PDO::PARAM_INT);
		$stmt->bindValue(':time', $leveltime, PDO::PARAM_INT);
		$stmt->execute();
	}

	//----------

	$stmt = $pdo->prepare("UPDATE users SET score=:scr, last_online=CURRENT_TIMESTAMP(), last_online_version=:av, revision_id=(revision_id+1) WHERE userid=:id");
	$stmt->bindValue(':id', $userid, PDO::PARAM_INT);
	$stmt->bindValue(':av', $appversion, PDO::PARAM_INT);
	$stmt->bindValue(':scr', $totalscore, PDO::PARAM_INT);
	$stmt->execute();

	$user->Score = $totalscore;
	$user->RevID++;

	//----------

	logDebug("levelscore changed for user:$userid Level:$levelid::$difficulty to $leveltime ms ($totalscore)");
	outputResultSuccess(['update' => true, 'value_db' => $leveltime, 'user' => $user]);
}



try {
	init("update-score");
	run();
} catch (Exception $e) {
	logError("InternalError: " . $e->getMessage() . "\n" . $e);
	outputError(Errors::INTERNAL_EXCEPTION, $e->getMessage());
}