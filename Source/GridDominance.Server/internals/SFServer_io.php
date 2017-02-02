<?php

abstract class ERRORS {
	/* ======== 99 INTERNAL ========= */
	const INTERNAL_EXCEPTION      = 99099;
	const MISSING_PARAMETER       = 99001;
	const PARAMETER_HASH_MISMATCH = 99002;
	const INVALID_PARAMETER       = 99003;
	const USER_BY_ID_NOT_FOUND    = 99004;
	const WRONG_PASSWORD          = 99005;

	/* ======== 10 CREATE-USER ========= */

	/* ======== 11 UPGRADE-USER ========= */
	const UPGRADE_USER_DUPLICATE_USERNAME  = 10001;
	const UPGRADE_USER_ACCOUNT_ALREADY_SET = 11002;

	/* ======== 12 PING ========= */

	/* ======== 13 CHANGE-PASSWORD ========= */
}

function is_int_str(string $value): bool {
	return ((is_int($value) || ctype_digit($value)) && (int)$value > 0 );
}

function getParamStrOrError(string $name): string {
	if(! isset($_GET[$name]))   outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set", LOGLEVEL::DEBUG);

	$v = $_GET[$name];

	if ($v === null)  outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set", LOGLEVEL::DEBUG);
	if ($v === false) outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set", LOGLEVEL::DEBUG);
	if (empty($v))    outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is empty", LOGLEVEL::DEBUG);

	return $v;
}

function getParamIntOrError(string $name): string {
	$v = getParamStrOrError($name);

	if (!is_int_str($v)) outputError(ERRORS::INVALID_PARAMETER, "The parameter $name (=$v) is not an integer", LOGLEVEL::DEBUG);

	return $v;
}

function outputError(int $errorid, string $message, int $logLevel = LOGLEVEL::NO_LOGGING) {
	echo json_encode(['result'=>'error', 'id'=>$errorid, 'message'=>$message]);

	logDynamic($logLevel, $message);

	exit (-1);
}

function outputResultSuccess(array $data) {
	echo json_encode(['result'=>'success', 'data'=>$data]);
	exit (0);
}