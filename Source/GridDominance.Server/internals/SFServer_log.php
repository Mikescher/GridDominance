<?php

abstract class LOGLEVEL {
	const NO_LOGGING = -1;
	const DEBUG      = 0;    // in debug-log log file
	const MESSAGE    = 1;    // normal logging
	const ERROR      = 2;    // fatal - send mail 2 me
}

function logMessage($msg, $id='LOG') {
	global $config;

	$fn1 = $config['logfile-normal'];
	$fn2 = $fn1 . '.old';

	$fs = @filesize($fn1);

	if ($fs && $fs > $config['maxsize-logfile-normal']) {
		if (file_exists($fn2))@unlink($fn2);
		copy($fn1, $fn2);
		@unlink($fn1);
	}

	$fd = fopen($fn1, "a");

	$pd = date('Y-m-d h:i:s');
	$ra = str_pad(isset($_SERVER['REMOTE_ADDR']) ? $_SERVER['REMOTE_ADDR'] : 'UNSET', 16);
	$str = "$id [$pd]-$ra $msg";

	fwrite($fd, $str . "n");

	fclose($fd);

	logDebug($msg, $id);
}

function logDebug($msg, $id='DBG') {
	global $config;

	$fn1 = $config['logfile-debug'];
	$fn2 = $fn1 . '.old';

	$fs = @filesize($fn1);

	if ($fs && $fs > $config['maxsize-logfile-debug']) {
		if (file_exists($fn2))@unlink($fn2);
		copy($fn1, $fn2);
		@unlink($fn1);
	}

	$fd = fopen($fn1, "a");

	$pd = date('Y-m-d h:i:s');
	$ra = str_pad(isset($_SERVER['REMOTE_ADDR']) ? $_SERVER['REMOTE_ADDR'] : 'UNSET', 16);
	$str = "$id [$pd]-$ra $msg";

	fwrite($fd, $str . "\n");

	fclose($fd);
}

function logError($msg) {
	global $config;

	logMessage($msg, 'ERR');

	$subject = "SFServer has encountered an Error at " . date("YYYY-mm-dd hh:ii:ss", mktime()) . "] ";

	$content = "";

	$content .= 'HTTP_HOST: '            . $_SERVER['HTTP_HOST']            . "\n";
	$content .= 'REQUEST_URI: '          . $_SERVER['REQUEST_URI']          . "\n";
	$content .= 'TIME: '                 . date('Y-m-d h:i:s')              . "\n";
	$content .= 'REMOTE_ADDR: '          . $_SERVER['REMOTE_ADDR']          . "\n";
	$content .= 'HTTP_X_FORWARDED_FOR: ' . $_SERVER['HTTP_X_FORWARDED_FOR'] . "\n";
	$content .= 'HTTP_USER_AGENT: '      . $_SERVER['HTTP_USER_AGENT']      . "\n";
	$content .= 'MESSAGE:'               . "\n" . $msg                      . "\n";

	mail($config['email-error-target'], $subject, $content, 'From: ' . $config['email-error-sender']);
}

function logDynamic(int $logLevel, string $message) {
	if ($logLevel == LOGLEVEL::DEBUG)   logDebug($message);
	if ($logLevel == LOGLEVEL::MESSAGE) logMessage($message);
	if ($logLevel == LOGLEVEL::ERROR)   logError($message);
}