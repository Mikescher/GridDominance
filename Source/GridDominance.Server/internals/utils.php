<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

require_once 'backend.php';

function getUserCount() {
	global $pdo;

	return $pdo->query('SELECT COUNT(*) FROM users')->fetch(PDO::FETCH_NUM)[0];
}

function getActiveUserCount($days) {
	global $pdo;

	return $pdo->query('SELECT COUNT(*) FROM users WHERE last_online >= now() - INTERVAL ' . $days . ' DAY')->fetch(PDO::FETCH_NUM)[0];
}

function getErrorCount() {
	global $pdo;

	return $pdo->query('SELECT COUNT(*) FROM error_log')->fetch(PDO::FETCH_NUM)[0];
}

function getRemainingErrorCount() {
	global $pdo;

	return $pdo->query('SELECT COUNT(*) FROM error_log WHERE acknowledged = 0')->fetch(PDO::FETCH_NUM)[0];
}

function getEntryCount() {
	global $pdo;

	return $pdo->query('SELECT COUNT(*) FROM level_highscores')->fetch(PDO::FETCH_NUM)[0];
}

function getTotalHighscore() {
	global $pdo;

	return $pdo->query('SELECT MAX(score) FROM users')->fetch(PDO::FETCH_NUM)[0];
}

function getRemainingErrors() {
	global $pdo;

	return $pdo->query('SELECT * FROM error_log LEFT JOIN users ON error_log.userid = users.userid WHERE acknowledged = 0')->fetchAll(PDO::FETCH_ASSOC);
}

function getAllErrors() {
	global $pdo;

	return $pdo->query('SELECT * FROM error_log LEFT JOIN users ON error_log.userid = users.userid')->fetchAll(PDO::FETCH_ASSOC);
}

function getUserErrors($uid) {
	global $pdo;

	$stmt = $pdo->prepare("SELECT * FROM error_log LEFT JOIN users ON error_log.userid = users.userid WHERE users.userid = :uid");
	$stmt->bindValue(':uid', $uid, PDO::PARAM_INT);
	$stmt->execute();

	return $stmt->fetchAll(PDO::FETCH_ASSOC);
}

function getUsers() {
	global $pdo;

	return $pdo->query('SELECT * FROM users')->fetchAll(PDO::FETCH_ASSOC);
}

function getLevelHighscores() {
	global $pdo;

	return $pdo->query('SELECT * FROM cache_levels LEFT JOIN users ON best_userid = users.userid')->fetchAll(PDO::FETCH_ASSOC);
}

function getGlobalHighscores() {
	global $pdo;

	return $pdo->query(loadSQL("get-ranking_global_top100"))->fetchAll(PDO::FETCH_ASSOC);
}

function getAllEntries() {
	global $pdo;

	return $pdo->query('SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid')->fetchAll(PDO::FETCH_ASSOC);
}

function getLevelEntries($lvl) {
	global $pdo;

	$stmt = $pdo->prepare("SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid WHERE levelid= :id");
	$stmt->bindValue(':id', $lvl, PDO::PARAM_STR);
	$stmt->execute();

	return $stmt->fetchAll(PDO::FETCH_ASSOC);
}

function getLevelDiffEntries($lvl, $diff) {
	global $pdo;

	$stmt = $pdo->prepare("SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid WHERE levelid= :id AND difficulty = :diff");
	$stmt->bindValue(':id', $lvl, PDO::PARAM_STR);
	$stmt->bindValue(':diff', $diff, PDO::PARAM_STR);
	$stmt->execute();

	return $stmt->fetchAll(PDO::FETCH_ASSOC);
}

function getUserEntries($uid) {
	global $pdo;

	$stmt = $pdo->prepare("SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid WHERE level_highscores.userid= :uid");
	$stmt->bindValue(':uid', $uid, PDO::PARAM_INT);
	$stmt->execute();

	return $stmt->fetchAll(PDO::FETCH_ASSOC);
}

function getWorldHighscores($worldid) {
	global $pdo;
	global $config;

	$condition = ' WHERE (';
	$ccfirst = true;
	foreach ($config['levelmapping'] as $mapping) {
		if ($mapping[0] == $worldid) {
			if (!$ccfirst) $condition .= ' OR ';
			$ccfirst = false;
			$condition .= 'level_highscores.levelid LIKE \'' . $mapping[1] . '\'';
		}
	}
	if ($ccfirst) $condition .= '0=1';
	$condition .= ') ';

	$stmt = $pdo->prepare(loadReplSQL('get-ranking_local_top100', '#$$CONDITION$$', $condition));
	$stmt->execute();
	return $stmt->fetchAll(PDO::FETCH_ASSOC);
}

function getLastCronTime() {
	global $config;

	$path = str_replace('{action}', 'cron', $config['logfile-debug']);

	if (!file_exists($path)) return "FNF";

	return date('Y-m-d H:i:s', filemtime($path));
}

function listLogFiles() {
	global $config;

	$list = [];

	$path = dirname($config['logfile-normal']);
	$dir = opendir($path);
	while($file = readdir($dir)){
		if ($file == '.' or $file == '..') continue;

		$filepath = $path . '/' . $file;

		$list[] =
			[
				'path' => $filepath,
				'name' => basename($filepath),
				'changedate' => date('Y-m-d H:i:s', filemtime($filepath)),
				'content' => file_get_contents($filepath),
			];
	}
	closedir($dir);

	return $list;
}