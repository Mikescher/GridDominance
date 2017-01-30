<?php

require_once 'internals/backend.php';


function run() {
	$username      = getParamStrOrError('username');
	$password      = getParamStrOrError('password');
	$devicename    = getParamStrOrError('device_name');
	$deviceversion = getParamStrOrError('device_version');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$username, $password]);

	$user = createAutoUser($username, $password, $devicename, $deviceversion);

	outputResultSuccess(['userid' => $user->ID]);
	logMessage("New user registered ($user->ID) $username");
}



try {
	run();
} catch (Exception $e) {
	logError("InternalError: " . $e->getMessage() . "\n" . $e);
	outputError(Errors::INTERNAL_EXCEPTION, $e->getMessage());
}