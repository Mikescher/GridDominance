<?php

require 'internals/backend.php';


/**
 *
 */
function run() {
	global $pdo;

	$userid        = getParamIntOrError('userid');
	$password_old  = getParamStrOrError('password_old');
	$password_new  = getParamStrOrError('password_new');
	$appversion    = getParamStrOrError('app_version');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password_old, $password_new, $appversion]);

	//----------

	$stmt = $pdo->prepare("SELECT userid, username, password_hash, is_auto_generated, score FROM users WHERE userid=:id");
	$stmt->bindValue(':id', $userid, PDO::PARAM_INT);
	$stmt->execute();
	$row = $stmt->fetch(PDO::FETCH_ASSOC);

	if ($row === FALSE) outputError(ERRORS::CHANGEPW_INVALID_USERID, "No user with id $userid found", LOGLEVEL::DEBUG);

	$user = GDUser::CreateFromSQL($row);

	if (! $user->verify_password($password_old)) outputError(ERRORS::CHANGEPW_WRONG_PASSWORD, "Wrong password supplied", LOGLEVEL::DEBUG);

	//----------

	$hash = password_hash($password_new, PASSWORD_BCRYPT);
	if (!$hash) throw new Exception('password_hash failure');

	$stmt = $pdo->prepare("UPDATE users SET password_hash=:pw, last_online=CURRENT_TIMESTAMP(), last_online_version=:av WHERE userid=:uid");
	$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
	$stmt->bindValue(':av', $appversion, PDO::PARAM_STR);
	$stmt->bindValue(':pw', $hash, PDO::PARAM_STR);
	$stmt->execute();

	//----------

	logDebug("password for user $userid changed (v: $appversion)");
	outputResultSuccess(['user' => $user]);
}



try {
	init("change-password");
	run();
} catch (Exception $e) {
	logError("InternalError: " . $e->getMessage() . "\n" . $e);
	outputError(Errors::INTERNAL_EXCEPTION, $e->getMessage());
}