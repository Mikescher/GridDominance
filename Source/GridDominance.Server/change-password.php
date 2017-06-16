<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$userid        = getParamUIntOrError('userid');
	$password_old  = getParamSHAOrError('password_old');
	$password_new  = getParamSHAOrError('password_new');
	$appversion    = getParamStrOrError('app_version');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password_old, $appversion, $password_new]);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password_old, $userid);

	if ($user->AutoUser) outputError(ERRORS::UPGRADE_USER_ACCOUNT_ALREADY_SET, "Cannot change password of auto user account", LOGLEVEL::DEBUG);

	//----------

	$hash = password_hash($password_new, PASSWORD_BCRYPT);
	if (!$hash) throw new Exception('password_hash failure');

	$user->ChangePassword($hash, $appversion);

	//----------

	logDebug("password for user $userid changed (v: $appversion)");
	outputResultSuccess(['user' => $user]);
}



try {
	init("change-password");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}