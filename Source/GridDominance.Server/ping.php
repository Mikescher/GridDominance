<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$userid        = getParamIntOrError('userid');
	$password      = getParamStrOrError('password');
	$appversion    = getParamStrOrError('app_version');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion]);

	//----------

	$stmt = $pdo->prepare("SELECT userid, username, password_hash, is_auto_generated, score FROM users WHERE userid=:id");
	$stmt->bindValue(':id', $userid, PDO::PARAM_INT);
	$stmt->execute();
	$row = $stmt->fetch(PDO::FETCH_ASSOC);

	if ($row === FALSE) outputError(ERRORS::PING_INVALID_USERID, "No user with id $userid found", LOGLEVEL::DEBUG);

	$user = GDUser::CreateFromSQL($row);

	if (! $user->verify_password($password)) outputError(ERRORS::PING_WRONG_PASSWORD, "Wrong password supplied", LOGLEVEL::DEBUG);

	//----------

	$stmt = $pdo->prepare("UPDATE users SET last_online=CURRENT_TIMESTAMP(), last_online_version=:av WHERE userid=:uid");
	$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
	$stmt->bindValue(':av', $appversion, PDO::PARAM_STR);
	$stmt->execute();

	//----------

	logDebug("user $userid send ping (v: $appversion)");
	outputResultSuccess(['user' => $user]);
}



try {
	init("ping");
	run();
} catch (Exception $e) {
	logError("InternalError: " . $e->getMessage() . "\n" . $e);
	outputError(Errors::INTERNAL_EXCEPTION, $e->getMessage());
}