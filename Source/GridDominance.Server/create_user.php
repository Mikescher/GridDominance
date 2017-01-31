<?php

require_once 'internals/backend.php';


function run() {
	global $pdo;

	$username      = 'anonymous';
	$password      = getParamStrOrError('password');
	$devicename    = getParamStrOrError('device_name');
	$deviceversion = getParamStrOrError('device_version');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$password]);

	//---------

	$hash = password_hash($password, PASSWORD_BCRYPT);
	if (!$hash) throw new Exception('password_hash failure');

	$stmt = $pdo->prepare("INSERT INTO users(username, password_hash, is_auto_generated, score, creation_device_name, creation_device_version) VALUES (:un, :pw, 1, 0, :dn, :dv)");
	$stmt->bindValue(':usr', $username, PDO::PARAM_STR);
	$stmt->bindValue(':pw', $hash, PDO::PARAM_STR);
	$stmt->bindValue(':dn', $devicename, PDO::PARAM_STR);
	$stmt->bindValue(':dv', $deviceversion, PDO::PARAM_STR);
	$succ = $stmt->execute();
	if (!$succ) throw new Exception('SQL for insert user failed');

	$user = GDUser::Create($pdo->lastInsertId(), $username);

	//---------

	outputResultSuccess(['userid' => $user->ID, 'username' => $username]);
	logMessage("New user registered $user->ID");
}



try {
	run();
} catch (Exception $e) {
	logError("InternalError: " . $e->getMessage() . "\n" . $e);
	outputError(Errors::INTERNAL_EXCEPTION, $e->getMessage());
}