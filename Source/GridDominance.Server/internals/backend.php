<?php

set_error_handler(function($errno, $errstr, $errfile, $errline) {
	if (0 === error_reporting()) return false;
	throw new ErrorException($errstr, 0, $errno, $errfile, $errline);
});

require_once __DIR__ . '/../vendor/autoload.php';
require_once 'SFServer.php';
require_once 'GDUser.php';

/** @var $config array */
$config = require 'config.php';

/** @var $pdo PDO */
$pdo = null;

/** @var $action_name String */
$action_name = "UNDEF";


function init(string $action) {
	global $config;
	global $pdo;
	global $action_name;

	$action_name = $action;

	if ($config['debug']) {
		error_reporting(E_STRICT);
		ini_set('display_errors', 1);
	} else {
		ini_set('display_errors', 0);
		ini_set('log_errors', 1);
	}

	$pdo = connectOrFail($config['database_host'], $config['database_name'], $config['database_user'], $config['database_pass']);
}
