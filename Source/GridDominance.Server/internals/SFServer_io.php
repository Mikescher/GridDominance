<?php

abstract class ERRORS {
	const INTERNAL_EXCEPTION      = 99000;
	const MISSING_PARAMETER       = 99001;
	const PARAMETER_HASH_MISMATCH = 99002;

	const CREATE_USER_DUPLICATE_USERNAME = 10002;

}

function getParamStrOrError(string $name) {
	if(! isset($_GET[$name]))   outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set", LOGLEVEL::DEBUG);

	$v = $_GET[$name];

	if ($v === null)  outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set", LOGLEVEL::DEBUG);
	if ($v === false) outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set", LOGLEVEL::DEBUG);
	if (empty($v))    outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is empty", LOGLEVEL::DEBUG);

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