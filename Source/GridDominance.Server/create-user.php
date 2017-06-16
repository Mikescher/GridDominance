<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$username          = GDUser::DEFAULT_USERNAME;
	$password          = getParamSHAOrError('password');
	$appversion        = getParamStrOrError('app_version');
	$devicename        = getParamStrOrError('device_name');
	$deviceversion     = getParamStrOrError('device_version');
	$unlocked_worlds   = getParamStrOrError('unlocked_worlds');
	$device_resolution = getParamStrOrError('device_resolution');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$password, $appversion, $devicename, $deviceversion, $unlocked_worlds, $device_resolution]);

	//---------

	$hash = password_hash($password, PASSWORD_BCRYPT);
	if (!$hash) throw new Exception('password_hash failure');

	$stmt = $pdo->prepare("INSERT INTO users(username, password_hash, is_auto_generated, score, device_name, device_version, app_version, unlocked_worlds, device_resolution) VALUES (:un, :pw, 1, 0, :dn, :dv, :av, :uw, :dr)");
	$stmt->bindValue(':un', $username, PDO::PARAM_STR);
	$stmt->bindValue(':pw', $hash, PDO::PARAM_STR);
	$stmt->bindValue(':dn', $devicename, PDO::PARAM_STR);
	$stmt->bindValue(':dv', $deviceversion, PDO::PARAM_STR);
	$stmt->bindValue(':av', $appversion, PDO::PARAM_STR);
	$stmt->bindValue(':uw', $unlocked_worlds, PDO::PARAM_STR);
	$stmt->bindValue(':dr', $device_resolution, PDO::PARAM_STR);
	executeOrFail($stmt);

	$user = GDUser::CreateNew($pdo->lastInsertId(), $username, 0);

	//---------

	logMessage("New user registered $user->ID");
	outputResultSuccess(['user' => $user]);
}



try {
	init("create-user");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}