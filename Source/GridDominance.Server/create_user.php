<?php

require_once 'internals/backend.php';


function run() {
	$username      = getParamStrOrError('username');
	$password      = getParamStrOrError('password');
	$devicename    = getParamStrOrError('device_name');
	$deviceversion = getParamStrOrError('device_version');

	$commit_hash   = getParamStrOrError('hash');

	check_commit_hash($commit_hash, [$username, $password]);

	$user = createAutoUser($username, $password, $devicename, $deviceversion);

	outputResultSuccess(['userid' => $user->ID]);
}



try {
	run();
} catch (Exception $e) {
	outputError(Errors::INTERNAL_EXCEPTION, $e->getMessage());
}