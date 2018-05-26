<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

set_error_handler(function($errno, $errstr, $errfile, $errline) {
	if (0 === error_reporting()) return false;
	throw new ErrorException($errstr, 0, $errno, $errfile, $errline);
});

//require_once __DIR__ . '/../vendor/autoload.php';

require_once 'SFServer.php';
require_once 'utils.php';
require_once 'GDUser.php';
require_once 'GDCustomLevel.php';

/** @var $config array */
$config = require 'config.php';

/** @var $pdo PDO */
$pdo = null;

/** @var $action_name String */
$action_name = "UNDEF";

/** @var $start_time float */
$start_time = 0;


/**
 * @param string $action
 * @param bool $notransaction
 */
function init($action, $notransaction = false) {
	global $config;
	global $pdo;
	global $action_name;
	global $start_time;

	$action_name = $action;
	$start_time = microtime(true);

	if ($config['debug']) sleep($config['ping_emulation']);

	if ($config['debug']) {
		error_reporting(E_STRICT);
		ini_set('display_errors', 1);
	} else {
		ini_set('display_errors', 0);
		ini_set('log_errors', 1);
	}

	$pdo = connectOrFail($config['database_host'], $config['database_name'], $config['database_user'], $config['database_pass'], $notransaction);
	$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

	if (!$notransaction) $pdo->beginTransaction();
}

function finish($err, $dorunlog = true) {
	global $config;
	global $pdo;
	global $action_name;
	global $start_time;

	if ($pdo !== null && $pdo->inTransaction())
	{
		if ($err) $pdo->rollBack();
		else $pdo->commit();
	}

	if ($config['runlog'] && !$err && $dorunlog) {
		try{
			$d = (int)((microtime(true) - $start_time) * 1000 * 1000);

			$stmt = $pdo->prepare("INSERT INTO runlog_volatile(action, duration) VALUES (:a, :d)");
			$stmt->bindValue(':a', $action_name, PDO::PARAM_STR);
			$stmt->bindValue(':d', $d, PDO::PARAM_INT);
			$stmt->execute();
		} catch (Exception $e) {
			logError("Error in insert runlog: " . $e->getMessage() . "\n" . $e->getTraceAsString(), 'ERR');
		}
	}
}