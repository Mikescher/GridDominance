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

	$stmt = $pdo->prepare("INSERT INTO userlevels(userid) VALUES (:uid)");
	$stmt->bindValue(':uid', $user->ID, PDO::PARAM_INT);
	executeOrFail($stmt);
	$liid = $pdo->lastInsertId();
	//----------

	logDebug("user $user->ID reserved levelid $liid");
	outputResultSuccess(['levelid' => $liid]);
}



try {
	init("get-newlevelid");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}