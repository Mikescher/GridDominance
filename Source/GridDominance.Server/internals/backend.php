<?php

require_once __DIR__ . '/../vendor/autoload.php';
require_once 'SFServer.php';

$config = require 'config.php';

set_error_handler(function($errno, $errstr, $errfile, $errline, array $errcontext) {
	// error was suppressed with the @-operator
	if (0 === error_reporting()) return false;
	throw new ErrorException($errstr, 0, $errno, $errfile, $errline);
});

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

// =====================================================================================================================

function createAutoUser(string $username, string $password, string $devicename, string $deviceversion): GDUser {
	global $pdo;

	$stmt = $pdo->prepare("SELECT COUNT(*) FROM users WHERE username=:usr");
	$stmt->bindValue(':usr', $username, PDO::PARAM_STR);
	$stmt->execute();

	if ($stmt->fetchColumn() > 0) outputError(ERRORS::CREATE_USER_DUPLICATE_USERNAME, "username $username already exists", LOGLEVEL::DEBUG);

	$hash = password_hash($password, PASSWORD_BCRYPT);
	if (!$hash) throw new Exception('password_hash failure');

	$stmt = $pdo->prepare("INSERT INTO users(username, password_hash, is_auto_generated, score, creation_device_name, creation_device_version) VALUES (:un, :pw, 1, 0, :dn, :dv)");
	$stmt->bindValue(':usr', $username, PDO::PARAM_STR);
	$stmt->bindValue(':pw', $hash, PDO::PARAM_STR);
	$stmt->bindValue(':dn', $devicename, PDO::PARAM_STR);
	$stmt->bindValue(':dv', $deviceversion, PDO::PARAM_STR);
	$succ = $stmt->execute();
	if (!$succ) throw new Exception('SQL for insert user failed');


	$result = new GDUser();
	$result->Username = $username;
	$result->ID = $pdo->lastInsertId();

	return $result;
}