<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

abstract class LOGLEVEL {
	const NO_LOGGING = -1;
	const DEBUG      = 0;    // in debug-log log file
	const MESSAGE    = 1;    // normal logging
	const ERROR      = 2;    // fatal - send mail 2 me
}

/**
 * @param string $msg
 * @param string $id
 */
function logMessage($msg, $id='LOG') {
	global $config;

	logToFile($config['logfile-normal'], $config['maxsize-logfile-normal'], $msg, $id);

	logDebug($msg, $id);
}

/**
 * @param string $msg
 * @param string $id
 */
function logDebug($msg, $id='DBG') {
	global $config;

	if ($config['debug']) logToFile($config['logfile-debug'], $config['maxsize-logfile-debug'], $msg, $id);
}

function logCron($msg, $id='CRN') {
	global $config;

	logToFile($config['logfile-cron'], $config['maxsize-logfile-normal'], $msg, $id);
}

function logErrorInfo($msg) {
	global $config;

	logToFile($config['logfile-error'], $config['maxsize-logfile-error'], $msg, 'ERR_INFO');
}

/**
 * @param string $msg
 */
function logError($msg) {
	global $config;

	$exc = NULL;

	try	{
		logMessage($msg, 'ERR');
	} catch (Exception $e) {
		$exc = $e;
	}

	try	{
		logToFile($config['logfile-error'], $config['maxsize-logfile-error'], $msg, 'ERR');
	} catch (Exception $e) {
		$exc = $e;
	}


	try	{
		$subject = "SFServer has encountered an Error at " . date("Y-m-d H:i:s") . "] ";

		$content = "";

		$content .= 'HTTP_HOST: '            . ParamServerOrUndef('HTTP_HOST')            . "\n";
		$content .= 'REQUEST_URI: '          . ParamServerOrUndef('REQUEST_URI')          . "\n";
		$content .= 'TIME: '                 . date('Y-m-d H:i:s')                        . "\n";
		$content .= 'REMOTE_ADDR: '          . ParamServerOrUndef('REMOTE_ADDR')          . "\n";
		$content .= 'HTTP_X_FORWARDED_FOR: ' . ParamServerOrUndef('HTTP_X_FORWARDED_FOR') . "\n";
		$content .= 'HTTP_USER_AGENT: '      . ParamServerOrUndef('HTTP_USER_AGENT')      . "\n";
		$content .= 'MESSAGE:'               . "\n" . $msg                                . "\n";

		sendMail($subject, $content, $config['email-error-target'], $config['email-error-sender']);
	} catch (Exception $e) {
		$exc = $e;
	}

	if ($exc !== NULL) {
		try	{
			logMessage("log error failed hard: " . $exc->getMessage() . "\n" . $exc->getTraceAsString(), 'ERR');
		} catch (Exception $e2) {
			try	{
				sendMail('FATAL log error', "Cannot log error, and can't even log the log error\n".$exc->getMessage()."\n".$e2->getMessage(), $config['email-error-target'], $config['email-error-sender']);
			} catch (Exception $e3) {
				// ok - i give up
			}
		}
	}
}

/**
 * @param string $filename
 * @param int $maxsize
 * @param string $msg
 * @param string $id
 */
function logToFile($filename, $maxsize, $msg, $id) {
	global $action_name;

	$filename = str_replace("{action}", $action_name, $filename);

	if (DIRECTORY_SEPARATOR != '/') $filename = str_replace("/", DIRECTORY_SEPARATOR, $filename);

	if (!file_exists(dirname($filename))) @mkdir(dirname($filename), 0777, true);


	$fn1 = $filename;
	if (DIRECTORY_SEPARATOR != '/') $fn1 = str_replace("/", DIRECTORY_SEPARATOR, $fn1);

	$fn2 = $fn1 . '.old';

	$fs = false;
	if (file_exists($fn1)) $fs = @filesize($fn1);

	if ($fs && $fs > $maxsize) {
		if (file_exists($fn2)) @unlink($fn2);
		copy($fn1, $fn2);
		@unlink($fn1);
	}


	$pd = date('Y-m-d H:i:s');
	$ra = str_pad(ParamServerOrUndef('REMOTE_ADDR'), 16);
	$str = "$id [$pd]-$ra $msg";

	$fd = fopen($fn1, "a");
	fwrite($fd, $str . "\n");
	fclose($fd);
}

/**
 * @param string $idx
 * @return string
 */
function ParamServerOrUndef($idx) {
	return isset($_SERVER[$idx]) ? $_SERVER[$idx] : 'NOT_SET';
}

/**
 * @param $logLevel
 * @param $message
 */
function logDynamic($logLevel, $message) {
	if ($logLevel == LOGLEVEL::DEBUG)   logDebug($message);
	if ($logLevel == LOGLEVEL::MESSAGE) logMessage($message);
	if ($logLevel == LOGLEVEL::ERROR)   logError($message);
}