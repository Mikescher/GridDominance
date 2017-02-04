<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamPPKOrError('password');
	$levelid       = getParamStrOrError('levelid');
	$difficulty    = getParamUIntOrError('difficulty');
	$leveltime     = getParamUIntOrError('leveltime');
	$totalscore    = getParamUIntOrError('totalscore');
	$appversion    = getParamStrOrError('app_version');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion, $levelid, $difficulty, $leveltime, $totalscore]);

	if ($leveltime <= 0) outputError(ERRORS::SET_SCORE_INVALID_TIME, "The time $leveltime is not possible", LogLevel::MESSAGE);
	if (!in_array($difficulty, $config['difficulties'], TRUE)) outputError(ERRORS::SET_SCORE_INVALID_DIFF, "The difficulty $difficulty is not possible", LogLevel::MESSAGE);
	if (!in_array($levelid, $config['levelids'], TRUE)) outputError(ERRORS::SET_SCORE_INVALID_DIFF, "The levelID $levelid is not possible", LogLevel::MESSAGE);
	if ($totalscore < 0) outputError(ERRORS::SET_SCORE_INVALID_SCORE, "The score $totalscore is not possible", LogLevel::MESSAGE);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password, $userid);

	//----------

	$stmt = $pdo->prepare("SELECT best_time FROM level_highscores WHERE userid=:uid AND levelid=:lid AND difficulty=:diff");
	$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
	$stmt->bindValue(':lid', $levelid, PDO::PARAM_STR);
	$stmt->bindValue(':diff', $difficulty, PDO::PARAM_INT);
	executeOrFail($stmt);
	$dbtime = $stmt->fetchColumn();

	if ($dbtime !== FALSE) {

		if ($dbtime <= $leveltime) {
			// better or same value in db
			outputResultSuccess(['update' => false, 'value_db' => $dbtime, 'user' => $user]);
		}

		// existing row in db
		$stmt = $pdo->prepare("UPDATE level_highscores SET best_time=:time, last_changed=CURRENT_TIMESTAMP() WHERE userid=:uid AND levelid=:lid AND difficulty=:diff");
		$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
		$stmt->bindValue(':lid', $levelid, PDO::PARAM_STR);
		$stmt->bindValue(':diff', $difficulty, PDO::PARAM_INT);
		$stmt->bindValue(':time', $leveltime, PDO::PARAM_INT);
		executeOrFail($stmt);
	} else {

		// no row in db
		$stmt = $pdo->prepare("INSERT INTO level_highscores (userid, levelid, difficulty, best_time, last_changed) VALUES (:uid, :lid, :diff, :time, CURRENT_TIMESTAMP())");
		$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
		$stmt->bindValue(':lid', $levelid, PDO::PARAM_STR);
		$stmt->bindValue(':diff', $difficulty, PDO::PARAM_INT);
		$stmt->bindValue(':time', $leveltime, PDO::PARAM_INT);
		executeOrFail($stmt);
	}

	//----------

	$stmt = $pdo->prepare("UPDATE users SET score=:scr, last_online=CURRENT_TIMESTAMP(), last_online_app_version=:av, revision_id=(revision_id+1) WHERE userid=:id");
	$stmt->bindValue(':id', $userid, PDO::PARAM_INT);
	$stmt->bindValue(':av', $appversion, PDO::PARAM_INT);
	$stmt->bindValue(':scr', $totalscore, PDO::PARAM_INT);
	executeOrFail($stmt);

	$user->Score = $totalscore;
	$user->RevID++;

	//----------

	logDebug("levelscore changed for user:$userid Level:$levelid::$difficulty to $leveltime ms ($totalscore)");
	outputResultSuccess(['update' => true, 'value_db' => $leveltime, 'user' => $user]);
}



try {
	init("set-score");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}