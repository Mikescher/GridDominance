<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$userid            = getParamUIntOrError('userid');
	$password          = getParamSHAOrError('password');
	$appversion        = getParamStrOrError('app_version');
	$devicename        = getParamStrOrError('device_name', true);
	$deviceversion     = getParamStrOrError('device_version', true);
	$unlocked_worlds   = getParamStrOrError('unlocked_worlds', true);
	$device_resolution = getParamStrOrError('device_resolution', true);
	$app_type          = getParamStrOrEmpty('app_type');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion, $devicename, $deviceversion, $unlocked_worlds, $device_resolution]);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password, $userid);

	//----------

	$user->UpdateMeta($appversion, $devicename, $deviceversion, $unlocked_worlds, $device_resolution, $app_type);

	//----------

	logDebug("user $userid send ping (v: $appversion)");
	outputResultSuccess(['user' => $user]);
}



try {
	set_time_limit(10);
	init("ping");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}