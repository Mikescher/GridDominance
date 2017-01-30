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
}



try {
	run();
} catch (Exception $e) {
	outputError(Errors::INTERNAL_EXCEPTION, $e->getMessage());
}