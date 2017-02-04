<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$userid        = getParamUIntOrError('userid');
	$password_old  = getParamPPKOrError('password_old');
	$password_new  = getParamPPKOrError('password_new');
	$username_new  = getParamStrOrError('username_new');
	$appversion    = getParamStrOrError('app_version');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password_old, $password_new, $username_new, $appversion]);

	$username_new = trim($username_new);

	//----------

	$stmt = $pdo->prepare("SELECT COUNT(*) FROM users WHERE username=:usr");
	$stmt->bindValue(':usr', $username_new, PDO::PARAM_STR);
	executeOrFail($stmt);

	if ($stmt->fetchColumn() > 0) outputError(ERRORS::UPGRADE_USER_DUPLICATE_USERNAME, "username $username_new already exists", LOGLEVEL::DEBUG);
	if ($username_new == 'anonymous') outputError(ERRORS::UPGRADE_USER_DUPLICATE_USERNAME, "username $username_new already exists", LOGLEVEL::DEBUG);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password_old, $userid);

	if (! $user->AutoUser) outputError(ERRORS::UPGRADE_USER_ACCOUNT_ALREADY_SET, "Only auto-accounts can be upgraded to full accounts", LOGLEVEL::DEBUG);

	//----------

	$hash = password_hash($password_new, PASSWORD_BCRYPT);
	if (!$hash) throw new Exception('password_hash failure');

	$stmt = $pdo->prepare("UPDATE users SET username=:usr, password_hash=:pw, is_auto_generated=0, last_online=CURRENT_TIMESTAMP(), last_online_app_version=:av, revision_id=(revision_id+1) WHERE userid=:id");
	$stmt->bindValue(':usr', $username_new, PDO::PARAM_STR);
	$stmt->bindValue(':pw', $hash, PDO::PARAM_STR);
	$stmt->bindValue(':id', $userid, PDO::PARAM_INT);
	$stmt->bindValue(':av', $appversion, PDO::PARAM_INT);
	executeOrFail($stmt);

	$user->Upgrade($username_new, $hash);

	//----------

	logMessage("user upgraded to account ($userid -> $username_new)");
	outputResultSuccess(['user' => $user]);
}



try {
	init("upgrade_user");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}