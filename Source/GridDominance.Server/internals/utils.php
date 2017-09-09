<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

require_once 'backend.php';

function getUserCount() {
	global $pdo;

	return $pdo->query('SELECT COUNT(*) FROM users WHERE score > 0')->fetch(PDO::FETCH_NUM)[0];
}

function getActiveUserCount($days) {
	global $pdo;

	return $pdo->query('SELECT COUNT(*) FROM users WHERE score > 0 AND last_online >= now() - INTERVAL ' . $days . ' DAY')->fetch(PDO::FETCH_NUM)[0];
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

	return $pdo->query('SELECT MAX(score) FROM users WHERE score > 0')->fetch(PDO::FETCH_NUM)[0];
}

function getRemainingErrors() {
	global $pdo;

	return $pdo->query('SELECT *, error_log.app_version AS app_version, users.app_version AS user_app_version FROM error_log LEFT JOIN users ON error_log.userid = users.userid WHERE acknowledged = 0')->fetchAll(PDO::FETCH_ASSOC);
}

function getAllErrors() {
	global $pdo;

	return $pdo->query('SELECT *, error_log.app_version AS app_version, users.app_version AS user_app_version FROM error_log LEFT JOIN users ON error_log.userid = users.userid')->fetchAll(PDO::FETCH_ASSOC);
}

function getUserErrors($uid) {
	global $pdo;

	$stmt = $pdo->prepare("SELECT *, error_log.app_version AS app_version, users.app_version AS user_app_version FROM error_log LEFT JOIN users ON error_log.userid = users.userid WHERE users.userid = :uid AND score > 0");
	$stmt->bindValue(':uid', $uid, PDO::PARAM_INT);
	$stmt->execute();

	return $stmt->fetchAll(PDO::FETCH_ASSOC);
}

function getUsers($all) {
	global $pdo;

	if ($all)
		return $pdo->query('SELECT * FROM users')->fetchAll(PDO::FETCH_ASSOC);
	else
		return $pdo->query('SELECT * FROM users WHERE score > 0')->fetchAll(PDO::FETCH_ASSOC);
}

function getActiveUsers($days, $all) {
	global $pdo;

	if ($all)
		return $pdo->query("SELECT * FROM users WHERE last_online >= now() - INTERVAL $days DAY")->fetchAll(PDO::FETCH_ASSOC);
	else
		return $pdo->query("SELECT * FROM users WHERE last_online >= now() - INTERVAL $days DAY AND score > 0")->fetchAll(PDO::FETCH_ASSOC);
}

function getLevelHighscores() {
	global $pdo;

	return $pdo->query('SELECT * FROM cache_levels LEFT JOIN users ON best_userid = users.userid AND users.score > 0')->fetchAll(PDO::FETCH_ASSOC);
}

function getGlobalHighscores() {
	global $pdo;

	return $pdo->query(loadSQL("get-ranking_global_top100"))->fetchAll(PDO::FETCH_ASSOC);
}

function getMultiplayerHighscores() {
	global $pdo;

	return $pdo->query("SELECT * FROM users WHERE is_auto_generated=0 AND score > 0 ORDER BY mpscore DESC LIMIT 100")->fetchAll(PDO::FETCH_ASSOC);
}

function getAllEntries() {
	global $pdo;

	return $pdo->query('SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid WHERE score > 0')->fetchAll(PDO::FETCH_ASSOC);
}

function getLevelEntries($lvl) {
	global $pdo;

	$stmt = $pdo->prepare("SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid WHERE levelid= :id AND score > 0");
	$stmt->bindValue(':id', $lvl, PDO::PARAM_STR);
	$stmt->execute();

	return $stmt->fetchAll(PDO::FETCH_ASSOC);
}

function getLevelDiffEntries($lvl, $diff) {
	global $pdo;

	$stmt = $pdo->prepare("SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid WHERE levelid= :id AND difficulty = :diff AND score > 0");
	$stmt->bindValue(':id', $lvl, PDO::PARAM_STR);
	$stmt->bindValue(':diff', $diff, PDO::PARAM_STR);
	$stmt->execute();

	return $stmt->fetchAll(PDO::FETCH_ASSOC);
}

function getUserEntries($uid) {
	global $pdo;

	$stmt = $pdo->prepare("SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid WHERE level_highscores.userid= :uid AND score > 0");
	$stmt->bindValue(':uid', $uid, PDO::PARAM_INT);
	$stmt->execute();

	return $stmt->fetchAll(PDO::FETCH_ASSOC);
}

function getWorldHighscores($worldid) {
	global $pdo;

	$stmt = $pdo->prepare(loadReplSQL('get-ranking_local_top100', '#$$FIELD$$', worldGuidToSQLField($worldid)));
	$stmt->execute();
	return $stmt->fetchAll(PDO::FETCH_ASSOC);
}

function getLastCronTime() {
	global $config;

	$path = str_replace('{action}', 'cron', $config['logfile-cron']);

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

function worldGuidToSQLField($worldid)
{
	global $config;

	if ($worldid == $config['worldid_1'] ) return "w1";
	if ($worldid == $config['worldid_2'] ) return "w2";
	if ($worldid == $config['worldid_3'] ) return "w3";
	if ($worldid == $config['worldid_4'] ) return "w4";

	throw new Exception("Unknown WorldID: " . $worldid);
}