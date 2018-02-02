<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamSHAOrError('password');
	$appversion    = getParamStrOrError('app_version');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion]);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password, $userid);

	//----------

	$user->UpdateLastOnline($appversion);

	//----------

	$stmt = $pdo->prepare("SELECT idmap.levelid AS levelid, level_highscores.difficulty AS difficulty, level_highscores.best_time AS best_time FROM level_highscores LEFT JOIN idmap ON level_highscores.shortid=idmap.shortid WHERE userid=:uid");
	$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
	executeOrFail($stmt);

	$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

	//----------

	logDebug("user $userid requested full download (v: $appversion)");
	outputResultSuccess(['user' => $user, 'scores' => $data]);
}



try {
	//set_time_limit(20);
	init("download-data");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}