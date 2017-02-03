<?php

abstract class LOGLEVEL {
	const NO_LOGGING = -1;
	const DEBUG      = 0;    // in debug-log log file
	const MESSAGE    = 1;    // normal logging
	const ERROR      = 2;    // fatal - send mail 2 me
}

function logMessage($msg, $id='LOG') {
	global $config;

	logToFile($config['logfile-normal'], $config['maxsize-logfile-normal'], $msg, $id);

	logDebug($msg, $id);
}

function logDebug($msg, $id='DBG') {
	global $config;

	logToFile($config['logfile-debug'], $config['maxsize-logfile-debug'], $msg, $id);
}

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
		$subject = "SFServer has encountered an Error at " . date("Y-m-d h:i:s") . "] ";

		$content = "";

		$content .= 'HTTP_HOST: '            . ParamServerOrUndef('HTTP_HOST')            . "\n";
		$content .= 'REQUEST_URI: '          . ParamServerOrUndef('REQUEST_URI')          . "\n";
		$content .= 'TIME: '                 . date('Y-m-d h:i:s')                        . "\n";
		$content .= 'REMOTE_ADDR: '          . ParamServerOrUndef('REMOTE_ADDR')          . "\n";
		$content .= 'HTTP_X_FORWARDED_FOR: ' . ParamServerOrUndef('HTTP_X_FORWARDED_FOR') . "\n";
		$content .= 'HTTP_USER_AGENT: '      . ParamServerOrUndef('HTTP_USER_AGENT')      . "\n";
		$content .= 'MESSAGE:'               . "\n" . $msg                                . "\n";

		sendMail($subject, $content, $config['email-error-target'], $config['email-error-sender']);
	} catch (Exception $e) {
		$exc = $e;
	}

	if ($exc !== NULL) {
		logMessage("log error failed hard: " . $exc->getMessage() . "\n" . $exc->getTraceAsString(), 'ERR');
	}
}

function logToFile(string $filename, int $maxsize, string $msg, string $id) {
	global $action_name;

	$filename = str_replace("{action}", $action_name, $filename);

	$fn1 = $filename;
	$fn2 = $fn1 . '.old';

	$fs = @filesize($fn1);

	if ($fs && $fs > $maxsize) {
		if (file_exists($fn2)) @unlink($fn2);
		copy($fn1, $fn2);
		@unlink($fn1);
	}


	$pd = date('Y-m-d h:i:s');
	$ra = str_pad(ParamServerOrUndef('REMOTE_ADDR'), 16);
	$str = "$id [$pd]-$ra $msg";

	$fd = fopen($fn1, "a");
	fwrite($fd, $str . "\n");
	fclose($fd);
}

function ParamServerOrUndef(string $idx) {
	if (isset($_SERVER[$idx]))
		return $_SERVER[$idx];
	else
		return 'NOT_SET';
}

function logDynamic(int $logLevel, string $message) {
	if ($logLevel == LOGLEVEL::DEBUG)   logDebug($message);
	if ($logLevel == LOGLEVEL::MESSAGE) logMessage($message);
	if ($logLevel == LOGLEVEL::ERROR)   logError($message);
}