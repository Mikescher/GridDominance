<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

require_once 'backend.php';

function getUserCount() {
	return sql_query_num('getUserCount', 'SELECT COUNT(*) FROM users WHERE score > 0');
}

function getUserCountWithMPScore() {
	return sql_query_num('getUserCountWithMPScore', 'SELECT COUNT(*) FROM users WHERE mpscore > 0');
}

function getActiveUserCount($days) {
	$days = (int)$days;

	return sql_query_num('getActiveUserCount', 'SELECT COUNT(*) FROM users WHERE score > 0 AND last_online >= now() - INTERVAL ' . $days . ' DAY');
}

function getEntryCount() {
	return sql_query_num('getEntryCount', 'SELECT COUNT(*) FROM level_highscores');
}

function getTotalHighscore() {
	return sql_query_num('getTotalHighscore', 'SELECT MAX(score) FROM users WHERE score > 0');
}

function getNewErrorsOverview() {
	global $config;

	return sql_query_assoc_prep('getNewErrorsOverview', "SELECT *, error_log.app_version AS app_version, users.app_version AS user_app_version FROM error_log LEFT JOIN users ON error_log.userid = users.userid WHERE acknowledged = 0 AND error_log.app_version = :vvv ORDER BY error_log.error_id DESC LIMIT 128",
	[
		[':vvv', $config['latest_version'], PDO::PARAM_STR],
	]);
}

function getErrors($versionfilter, $onlynonack, $idfilter, $page, $pagesize) {
	$cond = " WHERE 1=1";
	if ($versionfilter != "") $cond = $cond . " AND error_log.app_version = :vvv";
	if ($onlynonack) $cond = $cond . " AND error_log.acknowledged = 0";
	if ($idfilter != "") $cond = $cond . " AND error_log.exception_id = :idf";

	return sql_query_assoc_prep('getErrors', "SELECT *, error_log.app_version AS app_version, users.app_version AS user_app_version FROM error_log LEFT JOIN users ON error_log.userid = users.userid $cond ORDER BY error_log.error_id DESC LIMIT :ps OFFSET :po",
	[
		[':po',  $page * $pagesize, PDO::PARAM_INT],
		[':ps',  $pagesize,         PDO::PARAM_INT],
		[':vvv', $versionfilter,    PDO::PARAM_STR],
		[':idf', $idfilter,         PDO::PARAM_STR],
	]);
}

function countErrors($versionfilter, $onlynonack, $idfilter) {
	$cond = " WHERE 1=1";
	if ($versionfilter != "") $cond = $cond . " AND error_log.app_version = :vvv";
	if ($onlynonack) $cond = $cond . " AND error_log.acknowledged = 0";
	if ($idfilter != "") $cond = $cond . " AND error_log.exception_id = :idf";

	return sql_query_num_prep('countErrors', "SELECT COUNT(*) FROM error_log LEFT JOIN users ON error_log.userid = users.userid $cond",
	[
		[':vvv', $versionfilter, PDO::PARAM_STR],
		[':idf', $idfilter,      PDO::PARAM_STR],
	]);
}

function groupErrors($versionfilter, $onlynonack, $idfilter) {
	$cond = " WHERE 1=1";
	if ($versionfilter != "") $cond = $cond . " AND error_log.app_version = :vvv";
	if ($onlynonack) $cond = $cond . " AND error_log.acknowledged = 0";
	if ($idfilter != "") $cond = $cond . " AND error_log.exception_id = :idf";

	return sql_query_assoc_prep('groupErrors', "SELECT error_log.exception_id AS exception_id, COUNT(*) AS count FROM error_log LEFT JOIN users ON error_log.userid = users.userid $cond GROUP BY error_log.exception_id",
	[
		[':vvv', $versionfilter, PDO::PARAM_STR],
		[':idf', $idfilter,      PDO::PARAM_STR],
	]);
}

function getErrorOverviewByID() {
	return sql_query_assoc('getErrorOverviewByID', 'SELECT error_log.exception_id AS exception_id, COUNT(*) AS count_all, (SELECT COUNT(*) FROM error_log AS e2 WHERE e2.exception_id = error_log.exception_id AND e2.acknowledged=0 ) AS count_noack FROM error_log GROUP BY error_log.exception_id');
}

function getNewErrorOverviewByID() {
	return sql_query_assoc('getNewErrorOverviewByID', 'SELECT error_log.exception_id AS exception_id, (SELECT COUNT(*) FROM error_log AS e2 WHERE e2.exception_id = error_log.exception_id) AS count_all, COUNT(*) AS count_noack FROM error_log WHERE error_log.acknowledged=0 GROUP BY error_log.exception_id');
}

function getUserErrors($uid) {
	return sql_query_assoc_prep('getUserErrors', "SELECT *, error_log.app_version AS app_version, users.app_version AS user_app_version FROM error_log LEFT JOIN users ON error_log.userid = users.userid WHERE users.userid = :uid AND score > 0",
	[
		[':uid', $uid, PDO::PARAM_INT],
	]);
}

function getUsers($all, $reg, $days, $page, $pagesize = 1000) {
	$days = (int)$days;

	$cond = "WHERE 1=1";
	if (!$all)      $cond = $cond . " AND score > 0";
	if ($reg)       $cond = $cond . " AND is_auto_generated = 0";
	if ($days >= 0) $cond = $cond . " AND last_online >= now() - INTERVAL $days DAY";

	return sql_query_assoc_prep('getUsers', "SELECT * FROM users $cond LIMIT :ps OFFSET :po",
	[
		[':po', $pagesize * $page, PDO::PARAM_INT],
		[':ps', $pagesize,         PDO::PARAM_INT],
	]);
}

function countUsers($all, $reg, $days) {
	$days = (int)$days;

	$cond = "WHERE 1=1";
	if (!$all)      $cond = $cond . " AND score > 0";
	if ($reg)       $cond = $cond . " AND is_auto_generated = 0";
	if ($days >= 0) $cond = $cond . " AND last_online >= now() - INTERVAL $days DAY";

	return sql_query_num('countUsers', 'SELECT COUNT(*) FROM users ' . $cond);
}

function getLevelHighscores() {
	return sql_query_assoc('getLevelHighscores', 'SELECT * FROM cache_levels LEFT JOIN users ON best_userid = users.userid AND users.score > 0');
}

function getGlobalHighscores($limit = 100, $page = 0) {
	return sql_query_assoc_prep('getGlobalHighscores', loadSQL("get-ranking_global_top"),
	[
		[':qlimit', $limit,         PDO::PARAM_INT],
		[':qpage',  $limit * $page, PDO::PARAM_INT]
	]);
}

function getMultiplayerHighscores($limit = 100, $page = 0) {
	return sql_query_assoc_prep('getMultiplayerHighscores', loadSQL("get-ranking_multiplayer_top"),
	[
		[':qlimit', $limit,         PDO::PARAM_INT],
		[':qpage',  $limit * $page, PDO::PARAM_INT],
	]);
}

function getAllEntries($page, $pagesize) {
	return sql_query_assoc_prep('getAllEntries', "SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid WHERE score > 0 LIMIT :ps OFFSET :po",
	[
		[':po', $pagesize * $page, PDO::PARAM_INT],
		[':ps', $pagesize,         PDO::PARAM_INT],
	]);
}

function getLevelEntries($lvl) {
	return sql_query_assoc_prep('getLevelEntries', "SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid WHERE levelid= :id AND score > 0",
	[
		[':id', $lvl, PDO::PARAM_STR],
	]);
}

function getLevelDiffEntries($lvl, $diff, $limit) {
	return sql_query_assoc_prep('getLevelDiffEntries', "SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid WHERE levelid= :id AND difficulty = :diff AND score > 0 ORDER BY level_highscores.best_time ASC, level_highscores.last_changed ASC LIMIT :lim",
	[
		[':id', $lvl, PDO::PARAM_STR],
		[':diff', $diff, PDO::PARAM_STR],
		[':lim', $limit, PDO::PARAM_INT],
	]);
}

function getUserEntries($uid) {
	return sql_query_assoc_prep('getUserEntries', "SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid WHERE level_highscores.userid= :uid AND score > 0",
	[
		[':uid', $uid, PDO::PARAM_INT],
	]);
}

function getWorldHighscores($worldid, $limit=100, $page=0) {
	return sql_query_assoc_prep('getWorldHighscores', loadReplSQL('get-ranking_local_top', '#$$FIELD$$', worldGuidToSQLField($worldid)),
	[
		[':qlimit', $limit,         PDO::PARAM_INT],
		[':qpage',  $limit * $page, PDO::PARAM_INT],
	]);
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
				'size' => filesize($filepath),
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

function getScoreDistribution() {
	return sql_query_assoc('getScoreDistribution', "SELECT score AS score, COUNT(*) AS count FROM users WHERE score > 0 GROUP BY score");
}

function countUsersByUnlock($u) {
	$any = "\"%\"";
	return sql_query_num_prep('countUsersByUnlock', "SELECT COUNT(*) FROM users WHERE unlocked_worlds LIKE CONCAT($any, :u, $any)",
	[
		[':u', $u, PDO::PARAM_STR],
	]);
}

function countFirstPlaceUsers() {
	return sql_query_num('countFirstPlaceUsers', "SELECT COUNT(*) FROM users GROUP BY score ORDER BY score DESC LIMIT 1");
}

function countZeroScoreUsers() {
	return sql_query_num('countZeroScoreUsers', "SELECT COUNT(*) FROM users WHERE score = 0");
}

function suffixGetParams($id, $value) {
	$url = (isset($_SERVER['HTTPS']) ? "https" : "http") . "://$_SERVER[HTTP_HOST]$_SERVER[REQUEST_URI]";

	$url_parts = parse_url($url);

	if (isset($url_parts['query']))
		parse_str($url_parts['query'], $params);
	else
		$params = [];

	$params[$id] = $value;

	$url_parts['query'] = http_build_query($params);

	return  '?' . $url_parts['query'];
}

function statisticsUserByScoreRange() {
	return sql_query_assoc('statisticsUserByScoreRange', 'SELECT MIN(score) AS score1, MAX(score) AS score2, COUNT(*) AS count FROM users WHERE score>0 GROUP BY ROUND(score/500, 0)');
}

function statisticsUserByDevice() {
	return sql_query_assoc('statisticsUserByDevice', 'SELECT device_name AS name, COUNT(*) AS count FROM users WHERE score>0 GROUP BY device_name');
}

function statisticsUserByOS() {
	return sql_query_assoc('statisticsUserByOS', 'SELECT device_version AS name, COUNT(*) AS count FROM users WHERE score>0 GROUP BY device_version');
}

function statisticsUserByResolution() {
	return sql_query_assoc('statisticsUserByResolution', 'SELECT device_resolution AS name, COUNT(*) AS count FROM users WHERE score>0 GROUP BY device_resolution');
}

function statisticsUserByAppVersion() {
	return sql_query_assoc('statisticsUserByAppVersion', 'SELECT app_version AS name, COUNT(*) AS count FROM users WHERE score>0 GROUP BY app_version');
}

function statisticsUserByUnlocks() {
	global $config;

	$u5 = [];
	$u5[] = ['name' => '{d34db335-0001-4000-7711-000000100001}', 'count' => sql_query_num('statisticsUserByUnlocks_W', "SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000100001}%'")];
	$u5[] = ['name' => '{d34db335-0001-4000-7711-000000100002}', 'count' => sql_query_num('statisticsUserByUnlocks_W', "SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000100002}%'")];
	$u5[] = ['name' => '{d34db335-0001-4000-7711-000000300001}', 'count' => sql_query_num('statisticsUserByUnlocks_W', "SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000300001}%'")];

	foreach (array_unique(array_map(function($k){ return $k[0]; }, $config['levelmapping'])) as $w) {
		$u5[] = ['name' => $w, 'count' => sql_query_num('statisticsUserByUnlocks_W', "SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%" . $w . "%'")];
	}

	usort($u5, function ($a, $b) { return ($a['name'] <=> $b['name']); });

	$_u5 = [];
	foreach ($u5 as $u) {
		$f = false;
		foreach ($_u5 as $_u) $f = $f || ($_u['name'] == $u['name']);
		if (!$f) $_u5 []= $u;
	}
	return $_u5;
}

function statisticsUserByAnon() {
	return sql_query_assoc('statisticsUserByAnon', 'SELECT is_auto_generated AS name, COUNT(*) AS count FROM users WHERE score>0 GROUP BY is_auto_generated');
}

function statisticsUserByAppType() {
	return sql_query_assoc('statisticsUserByAppType', 'SELECT app_type AS name, COUNT(*) AS count FROM users WHERE score>0 GROUP BY app_type');
}

function getManualRecalculatedUserTimes($uid) {
	return sql_query_assoc_prep('getManualRecalculatedUserTimes', loadSQL('manual_calculate_time'),
	[
		[':uid1', $uid, PDO::PARAM_INT],
		[':uid2', $uid, PDO::PARAM_INT],
		[':uid3', $uid, PDO::PARAM_INT],
		[':uid4', $uid, PDO::PARAM_INT],
	]);
}

function getGlobalPlayerRank($uid) {
	return sql_query_assoc_prep('getGlobalPlayerRank', loadSQL('get-ranking_global_playerrank'),
	[
		[':uid', $uid, PDO::PARAM_INT],
	]);
}

function getLocalPlayerRank($uid, $world) {
	return sql_query_assoc_prep('getLocalPlayerRank', loadReplSQL('get-ranking_local_playerrank', '#$$FIELD$$', worldGuidToSQLField($world)),
	[
		[':uid', $uid, PDO::PARAM_INT],
	]);
}

function getUserData($uid) {
	return sql_query_assoc_prep('getUserData', "SELECT * FROM users WHERE userid=:id LIMIT 1",
	[
		[':id', $uid, PDO::PARAM_INT],
	]);
}

function getErrorData($uid) {
	return sql_query_assoc_prep('getErrorData', "SELECT * FROM error_log WHERE error_id=:id LIMIT 1",
	[
		[':id', $uid, PDO::PARAM_INT],
	]);
}
function getLastRunLogCount() {
	return sql_query_num('getLastRunLogCount', "SELECT SUM(count) FROM runlog_history WHERE exectime >= now() - INTERVAL 1 DAY AND action <> 'cron' AND action <> 'admin'");
}

function getLastTimingAverage() {
	return sql_query_num('getLastTimingAverage', "SELECT (SUM(duration)/SUM(count)) FROM runlog_history WHERE exectime >= now() - INTERVAL 1 DAY AND action <> 'cron' AND action <> 'admin'");
}

function getRunLogActionList() {
	return sql_query_assoc('getRunLogActionList', "SELECT action FROM runlog_history GROUP BY action");
}

function getRunLog($action) {
	return sql_query_assoc_prep('getRunLogActionList', "SELECT * FROM runlog_history WHERE action = :ac ORDER BY exectime DESC LIMIT 50",
	[
		[':ac', $action, PDO::PARAM_STR],
	]);
}

function getRunLogCountVolatile() {
	return sql_query_num('getRunLogCountVolatile', "SELECT COUNT(*) FROM runlog_volatile");
}

function getRunLogCountHistory() {
	return sql_query_num('getRunLogCountHistory', "SELECT COUNT(*) AS cnt FROM runlog_history GROUP BY action ORDER by cnt DESC LIMIT 1");
}