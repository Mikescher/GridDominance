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

	$stmt = $pdo->prepare("SELECT levelid, difficulty, best_time FROM level_highscores WHERE userid=:uid");
	$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
	executeOrFail($stmt);

	$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

	//----------

	logDebug("user $userid requested full download (v: $appversion)");
	outputResultSuccess(['user' => $user, 'scores' => $data]);
}



try {
	init("download-data");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}