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

	$stmt = $pdo->prepare("UPDATE users SET last_online=CURRENT_TIMESTAMP(), last_online_app_version=:av, ping_counter=ping_counter+1 WHERE userid=:uid");
	$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
	$stmt->bindValue(':av', $appversion, PDO::PARAM_STR);
	executeOrFail($stmt);

	//----------

	logDebug("user $userid send ping (v: $appversion)");
	outputResultSuccess(['user' => $user]);
}



try {
	init("ping");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}