<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$username      = getParamStrOrError('username');
	$password      = getParamSHAOrError('password');
	$appversion    = getParamStrOrError('app_version');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$username, $password, $appversion]);

	//----------

	$user = GDUser::QueryOrFailByName($pdo, $password, $username);

	//----------

	$user->UpdateLastOnline($appversion);

	//----------

	logDebug("user $user->ID verified credentials (v: $appversion)");
	outputResultSuccess(['user' => $user]);
}



try {
	init("verify");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}