<?php

class ERRORS {
	const INTERNAL_EXCEPTION      = 99000;
	const MISSING_PARAMETER       = 99001;
	const PARAMETER_HASH_MISMATCH = 99002;

	const CREATE_USER_DUPLICATE_USERNAME = 10002;

}

function getParamStrOrError(string $name) {
	if(! isset($_GET[$name]))   outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set");

	$v = $_GET[$name];

	if ($v === null)  outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set");
	if ($v === false) outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set");
	if (empty($v))    outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set");

	return $v;
}

function outputError(int $errorid, string $message) {
	echo json_encode(['result'=>'error', 'id'=>$errorid, 'message'=>$message]);
	exit (-1);
}

function outputResultSuccess(array $data) {
	echo json_encode(['result'=>'success', 'data'=>$data]);
	exit (0);
}