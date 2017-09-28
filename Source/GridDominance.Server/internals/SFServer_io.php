<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

abstract class ERRORS {
	/* ======== 99 INTERNAL ========= */
	const INTERNAL_EXCEPTION      = 99099;
	const MISSING_PARAMETER       = 99001;
	const PARAMETER_HASH_MISMATCH = 99002;
	const INVALID_PARAMETER       = 99003;
	const USER_BY_ID_NOT_FOUND    = 99004;
	const WRONG_PASSWORD          = 99005;
	const USER_BY_NAME_NOT_FOUND  = 99006;

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

	/* ======== 14 MERGE-LOGIN ========= */
	const MERGE_INVALID_TIME  = 14001;
	const MERGE_INVALID_LVLID = 14002;
	const MERGE_INVALID_DIFF  = 14003;
}

/**
 * @param string $value
 * @return bool
 */
function is_uint_str($value) {
	return ctype_digit(strval($value)) && strlen($value) > 0;
}

/**
 * @param string $value
 * @return bool
 */
function is_int_str($value) {
	if (strlen($value) > 0 && $value[0] === '-') $value = substr($value, 1);
	return ctype_digit(strval($value)) && strlen($value) > 0;
}

/**
 * @param string $name
 * @param bool $allowEmpty
 * @return string
 */
function getParamStrOrError($name, $allowEmpty = false) {
	$v = null;

	if( isset($_GET[$name])) $v = $_GET[$name];

	if ($v === null) { if( isset($_POST[$name])) $v = $_POST[$name]; }

	if ($v === null) {
		$opt = getopt('', [$name . '::']);
		if (isset($opt[$name])) $v = $opt[$name];
	}


	if ($v === null)               outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set", LOGLEVEL::ERROR);
	if ($v === false)              outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is not set", LOGLEVEL::ERROR);
	if (!$allowEmpty && empty($v)) outputError(ERRORS::MISSING_PARAMETER, "The parameter $name is empty", LOGLEVEL::ERROR);

	return $v;
}

/**
 * @param string $name
 * @return string
 */
function getParamStrOrEmpty($name) {
	$v = null;

	if( isset($_GET[$name])) $v = $_GET[$name];

	if ($v === null) { if( isset($_POST[$name])) $v = $_POST[$name]; }

	if ($v === null) {
		$opt = getopt('', [$name . '::']);
		if (isset($opt[$name])) $v = $opt[$name];
	}


	if ($v === null)  return '';
	if ($v === false) return '';
	if (empty($v))    return '';

	return $v;
}

/**
 * @param string $name
 * @return string
 */
function getParamSHAOrError($name) {
	$v = strtoupper(getParamStrOrError($name));

	if (strlen($v) !== 64) outputError(ERRORS::INVALID_PARAMETER, "The parameter $name is not in the correct format", LOGLEVEL::ERROR);

	return $v;
}

/**
 * @param string $name
 * @param bool $allowEmpty
 * @return string
 */
function getParamB64OrError($name, $allowEmpty = false) {
	$v = getParamStrOrError($name, $allowEmpty);

	// modified Base64  @see https://en.wikipedia.org/wiki/Base64#URL_applications
	$rv = str_replace("-", "+", $v);
	$rv = str_replace("_", "/", $rv);
	$rv = str_replace(".", "=", $rv);

	$rv = base64_decode($rv, TRUE);

	if ($rv === FALSE) outputError(ERRORS::INVALID_PARAMETER, "The parameter $name (=$v) is not base64 encoded", LOGLEVEL::ERROR);

	return $rv;
}

/**
 * @param string $name
 * @param bool $allowEmpty
 * @return string
 */
function getParamDeflOrError($name, $allowEmpty = false) {
	$v = getParamStrOrError($name, $allowEmpty);

	// modified Base64  @see https://en.wikipedia.org/wiki/Base64#URL_applications
	$rv = str_replace("-", "+", $v);
	$rv = str_replace("_", "/", $rv);
	$rv = str_replace(".", "=", $rv);

	$rv = base64_decode($rv, TRUE);

	if ($rv === FALSE) outputError(ERRORS::INVALID_PARAMETER, "The parameter $name (=$v) is not base64 encoded", LOGLEVEL::ERROR);

	$dv = gzinflate($rv);

	if ($dv === FALSE) outputError(ERRORS::INVALID_PARAMETER, "The parameter $name (=$v) is not deflated", LOGLEVEL::ERROR);

	return $dv;
}

/**
 * @param string $name
 * @return string
 */
function getParamDeflOrRaw($name) {
	$v = getParamStrOrEmpty($name);

	// modified Base64  @see https://en.wikipedia.org/wiki/Base64#URL_applications
	$rv = str_replace("-", "+", $v);
	$rv = str_replace("_", "/", $rv);
	$rv = str_replace(".", "=", $rv);

	$rv = base64_decode($rv, TRUE);

	if ($rv === FALSE) return "Base64 decode failed" . "\n" . $rv;

	$dv = gzinflate($rv);

	if ($dv === FALSE) return "GZip deflate failed" . "\n" . $rv;

	return $dv;
}

/**
 * @param string $name
 * @return string
 */
function getParamUIntOrError($name) {
	$v = getParamStrOrError($name, true);

	if (!is_uint_str($v)) outputError(ERRORS::INVALID_PARAMETER, "The parameter $name (=$v) is not an integer", LOGLEVEL::ERROR);

	return (int)$v;
}

/**
 * @param string $name
 * @return string
 */
function getParamIntOrError($name) {
	$v = getParamStrOrError($name, true);

	if (!is_int_str($v)) outputError(ERRORS::INVALID_PARAMETER, "The parameter $name (=$v) is not an integer", LOGLEVEL::ERROR);

	return (int)$v;
}

/**
 * @param string $name
 * @return string
 */
function getParamIntOrRaw($name) {
	$v = getParamStrOrEmpty($name);

	if (!is_int_str($v)) return $v;

	return (int)$v;
}

/**
 * @param int $errorid
 * @param string $message
 * @param int $logLevel
 */
function outputError($errorid, $message, $logLevel = LOGLEVEL::NO_LOGGING) {
	global $config;
	global $start_time;

	$d = ['result'=>'error', 'errorid'=>$errorid, 'errormessage'=>$message];
	if ($config['debug']) $d['runtime'] = round((microtime(true) - $start_time), 6);
	echo json_encode($d);

	if ($logLevel == LOGLEVEL::ERROR && $errorid == ERRORS::MISSING_PARAMETER && empty($_GET) && empty($_POST)) {
		// no error-mail on dummy request
		$logLevel = LOGLEVEL::WARN;
	}

	logDynamic($logLevel, $message);

	finish();
	exit (-1);
}

/**
 * @param int $errorid
 * @param string $message
 * @param Exception $e
 * @param int $logLevel
 */
function outputErrorException($errorid, $message, $e, $logLevel = LOGLEVEL::NO_LOGGING) {
	global $config;
	global $start_time;

	$d = ['result'=>'error', 'errorid'=>$errorid, 'errormessage'=>$message . ': ' . $e->getMessage()];
	if ($config['debug']) $d['runtime'] = round((microtime(true) - $start_time), 6);
	echo json_encode($d);

	logDynamic($logLevel, $message . ': ' . $e->getMessage() . "\n" . $e->getTraceAsString());

	finish();
	exit (-1);
}

/**
 * @param $data
 */
function outputResultSuccess($data) {
	global $config;
	global $start_time;

	$d = array_merge(['result'=>'success'], $data);
	if ($config['debug']) $d['runtime'] = round((microtime(true) - $start_time), 6);
	echo json_encode($d);

	finish();
	exit (0);
}

/**
 * @param string $subject
 * @param string $content
 * @param string $to
 * @param string $from
 */
function sendMail($subject, $content, $to, $from) {
	mail($to, $subject, $content, 'From: ' . $from);
}