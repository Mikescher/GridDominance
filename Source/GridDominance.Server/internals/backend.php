<?php

require_once __DIR__ . '/../vendor/autoload.php';
require_once 'SFServer.php';

$config = require 'config.php';

set_error_handler(function($errno, $errstr, $errfile, $errline, array $errcontext) {
	// error was suppressed with the @-operator
	if (0 === error_reporting()) return false;
	throw new ErrorException($errstr, 0, $errno, $errfile, $errline);
});

/**
 * @var $pdo PDO
 */
$pdo = null;

try {
	init();
} catch (Exception $e) {
	logError("InternalError: " . $e->getMessage() . "\n" . $e);
	outputError(Errors::INTERNAL_EXCEPTION, $e->getMessage());
}


function init() {
	global $config;
	global $pdo;

	if (!$config['debug']) error_reporting(E_STRICT);

	$dsn = 'mysql:host=' . $config['database_host'] . ';dbname=' . $config['database_name'] . ';charset=utf8';
	$opt = [
		PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
		PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
		PDO::ATTR_EMULATE_PREPARES   => false,
	];
	$pdo = new PDO($dsn, $config['database_user'], $config['database_pass'], $opt);
}
