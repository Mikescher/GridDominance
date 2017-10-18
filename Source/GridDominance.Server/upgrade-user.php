<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$userid        = getParamUIntOrError('userid');
	$password_old  = getParamSHAOrError('password_old');
	$password_new  = getParamSHAOrError('password_new');
	$username_new  = getParamStrOrError('username_new');
	$appversion    = getParamStrOrError('app_version');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password_old, $appversion, $password_new, $username_new]);

	$username_new = trim($username_new);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password_old, $userid);

	if (! $user->AutoUser) outputError(ERRORS::UPGRADE_USER_ACCOUNT_ALREADY_SET, "Only auto-accounts can be upgraded to full accounts", LOGLEVEL::DEBUG);

	//----------

	$stmt = $pdo->prepare("SELECT COUNT(*) FROM users WHERE username=:usr");
	$stmt->bindValue(':usr', $username_new, PDO::PARAM_STR);
	executeOrFail($stmt);

	if ($stmt->fetchColumn() > 0) outputError(ERRORS::UPGRADE_USER_DUPLICATE_USERNAME, "username $username_new already exists", LOGLEVEL::DEBUG);
	if ($username_new == 'anonymous') outputError(ERRORS::UPGRADE_USER_DUPLICATE_USERNAME, "username $username_new already exists", LOGLEVEL::DEBUG);

	//----------

	$hash = password_hash($password_new, PASSWORD_BCRYPT);
	if (!$hash) throw new Exception('password_hash failure');

	$user->Upgrade($username_new, $hash, $appversion);

	//----------

	logMessage("user upgraded to account ($userid -> $username_new)");
	outputResultSuccess(['user' => $user]);
}



try {
	set_time_limit(45);
	init("upgrade-user");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}