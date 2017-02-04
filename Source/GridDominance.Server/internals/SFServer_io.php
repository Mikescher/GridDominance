<?php

abstract class ERRORS {
	/* ======== 99 INTERNAL ========= */
	const INTERNAL_EXCEPTION      = 99099;
	const MISSING_PARAMETER       = 99001;
	const PARAMETER_HASH_MISMATCH = 99002;
	const INVALID_PARAMETER       = 99003;
	const USER_BY_ID_NOT_FOUND    = 99004;
	const WRONG_PASSWORD          = 99005;
	const SQL_FAILED              = 99006;

	/* ======== 11 UPGRADE-USER ========= */
	const UPGRADE_USER_DUPLICATE_USERNAME  = 10001;
	const UPGRADE_USER_ACCOUNT_ALREADY_SET = 11002;

	/* ======== 12 SET-SCORE ========= */
	const SET_SCORE_INVALID_TIME  = 12001;
	const SET_SCORE_INVALID_SCORE = 12002;
	const SET_SCORE_INVALID_LVLID = 12003;
	const SET_SCORE_INVALID_DIFF  = 12004;

	/* ======== 13 SET-SCORE ========= */
	const CRON_INTERNAL_ERR  = 13001;
}

/**
 * @param string $value
 * @return bool
 */
function is_int_str($value) {
	return ((is_int($value) || ctype_digit($value)) && (int)$value > 0 );
}

/**
 * @param string $name
 * @return string
 */
function getParamStrOrError($name) {
	if(! isset($_GET[$name]))   outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set", LOGLEVEL::DEBUG);

	$v = $_GET[$name];

	if ($v === null)  outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set", LOGLEVEL::DEBUG);
	if ($v === false) outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set", LOGLEVEL::DEBUG);
	if (empty($v))    outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is empty", LOGLEVEL::DEBUG);

	return $v;
}

/**
 * @param string $name
 * @return string
 */
function getParamPPKOrError($name) {
	$v = getParamStrOrError($name);

	$rv = str_replace("-", "+", $v);
	$rv = str_replace("_", "/", $rv);
	$rv = str_replace(".", "=", $rv);

	$rv = decrypt_rsa($rv);

	if ($rv === false) outputError(ERRORS::INVALID_PARAMETER, "The parameter $name is not correctly encrypted", LOGLEVEL::DEBUG);

	return $rv;
}

/**
 * @param string $name
 * @return string
 */
function getParamB64OrError($name) {
	$v = getParamStrOrError($name);

	// modified Base64  @see https://en.wikipedia.org/wiki/Base64#URL_applications
	$rv = str_replace("-", "+", $v);
	$rv = str_replace("_", "/", $rv);
	$rv = str_replace(".", "=", $rv);

	$rv = base64_decode($rv, TRUE);

	if ($rv === FALSE) outputError(ERRORS::INVALID_PARAMETER, "The parameter $name (=$v) is not base64 encoded", LOGLEVEL::DEBUG);

	return $rv;
}

/**
 * @param string $name
 * @return string
 */
function getParamUIntOrError($name) {
	$v = getParamStrOrError($name);

	if (!is_int_str($v)) outputError(ERRORS::INVALID_PARAMETER, "The parameter $name (=$v) is not an integer", LOGLEVEL::DEBUG);

	return (int)$v;
}

/**
 * @param int $errorid
 * @param string $message
 * @param int $logLevel
 */
function outputError($errorid, $message, $logLevel = LOGLEVEL::NO_LOGGING) {
	echo json_encode(['result'=>'error', 'id'=>$errorid, 'message'=>$message]);

	logDynamic($logLevel, $message);

	exit (-1);
}

/**
 * @param int $errorid
 * @param string $message
 * @param Exception $e
 * @param int $logLevel
 */
function outputErrorException($errorid, $message, $e, $logLevel = LOGLEVEL::NO_LOGGING) {
	echo json_encode(['result'=>'error', 'id'=>$errorid, 'message'=>$message . ': ' . $e->getMessage()]);

	logDynamic($logLevel, $message . ': ' . $e->getMessage() . "\n" . $e->getTraceAsString());

	exit (-1);
}

/**
 * @param $data
 */
function outputResultSuccess($data) {
	echo json_encode(array_merge(['result'=>'success'], $data));
	exit (0);
}

/**
 * @param string $subject
 * @param string $content
 * @param string $to
 * @param string $from
 */
function sendMail($subject, $content, $to, $from) {
	global $config;

	if ($config['debug']) return;

	mail($to, $subject, $content, 'From: ' . $from);
}