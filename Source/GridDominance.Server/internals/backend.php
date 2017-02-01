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

	if (!$config['debug']) error_reporting(E_STRICT);

	$dsn = 'mysql:host=' . $config['database_host'] . ';dbname=' . $config['database_name'] . ';charset=utf8';
	$opt = [
		PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
		PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
		PDO::ATTR_EMULATE_PREPARES   => false,
	];
	$pdo = new PDO($dsn, $config['database_user'], $config['database_pass'], $opt);
}
