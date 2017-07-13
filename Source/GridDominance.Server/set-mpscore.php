<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamSHAOrError('password');
	$appversion    = getParamStrOrError('app_version');
	$mpscore       = getParamUIntOrError('mpscore');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion, $mpscore]);

	if ($mpscore < 0) outputError(ERRORS::SET_SCORE_INVALID_SCORE, "The score $mpscore is not possible", LogLevel::MESSAGE);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password, $userid);

	//----------

	$user->SetMPScore($mpscore, $appversion);

	logDebug("multiplayerscore changed (update) for user:$userid ($mpscore)");
	outputResultSuccess(['update' => true, 'user' => $user]);
}



try {
	init("set-mpscore");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}