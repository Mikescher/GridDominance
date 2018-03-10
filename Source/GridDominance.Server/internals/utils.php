<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

require_once 'backend.php';

function getUserCount() {
	return sql_query_num('getUserCount', 'SELECT COUNT(*) FROM users WHERE score > 0');
}

function getUserCountWithMPScore() {
	return sql_query_num('getUserCountWithMPScore', 'SELECT COUNT(*) FROM users WHERE mpscore > 0');
}

function getUserCountWithSCCMScore() {
	return sql_query_num('getUserCountWithSCCMScore', 'SELECT COUNT(*) FROM users WHERE score_sccm > 0');
}

function getUserCountWithStarsScore() {
	return sql_query_num('getUserCountWithStarsScore', 'SELECT COUNT(*) FROM users WHERE score_stars > 0');
}

function getActiveUserCount($days) {
	$days = (int)$days;

	return sql_query_num('getActiveUserCount', 'SELECT COUNT(*) FROM users WHERE score > 0 AND last_online >= now() - INTERVAL ' . $days . ' DAY');
}

function getEntryCount() {
	return sql_query_num('getEntryCount', 'SELECT COUNT(1) FROM level_highscores');
}

function guessEntryCount() {
	return sql_query_num('guessEntryCount', "SELECT TABLE_ROWS FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'level_highscores'");
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

function getUsers($all, $reg, $days, $device, $dversion, $resolution, $appversion, $apptype, $page, $pagesize = 1000) {
	$days = (int)$days;

	$cond = "WHERE 1=1";
	if (!$all)             $cond = $cond . " AND score > 0";
	if ($reg === TRUE)     $cond = $cond . " AND is_auto_generated = 0";
	if ($reg === FALSE)    $cond = $cond . " AND is_auto_generated = 1";
	if ($days >= 0)        $cond = $cond . " AND last_online >= now() - INTERVAL $days DAY";
	if ($device != '')     $cond = $cond . " AND device_name = :dn";
	if ($dversion != '')   $cond = $cond . " AND device_version = :dv";
	if ($resolution != '') $cond = $cond . " AND device_resolution = :dr";
	if ($appversion != '') $cond = $cond . " AND app_version = :av";
	if ($apptype != '')    $cond = $cond . " AND app_type = :at";

	return sql_query_assoc_prep('getUsers', "SELECT * FROM users $cond LIMIT :ps OFFSET :po",
	[
		[':po', $pagesize * $page, PDO::PARAM_INT],
		[':ps', $pagesize,         PDO::PARAM_INT],
		[':dn', $device,           PDO::PARAM_STR],
		[':dv', $dversion,         PDO::PARAM_STR],
		[':dr', $resolution,       PDO::PARAM_STR],
		[':av', $appversion,       PDO::PARAM_STR],
		[':at', $apptype,          PDO::PARAM_STR],
	]);
}

function countUsers($all, $reg, $days, $device, $dversion, $resolution, $appversion, $apptype) {
	$days = (int)$days;

	$cond = "WHERE 1=1";
	if (!$all)             $cond = $cond . " AND score > 0";
	if ($reg === TRUE)     $cond = $cond . " AND is_auto_generated = 0";
	if ($reg === FALSE)    $cond = $cond . " AND is_auto_generated = 1";
	if ($days >= 0)        $cond = $cond . " AND last_online >= now() - INTERVAL $days DAY";
	if ($device != '')     $cond = $cond . " AND device_name = :dn";
	if ($dversion != '')   $cond = $cond . " AND device_version = :dv";
	if ($resolution != '') $cond = $cond . " AND device_resolution = :dr";
	if ($appversion != '') $cond = $cond . " AND app_version = :av";
	if ($apptype != '')    $cond = $cond . " AND app_type = :at";

	return sql_query_num_prep('countUsers', 'SELECT COUNT(*) FROM users ' . $cond,
	[
		[':dn', $device,           PDO::PARAM_STR],
		[':dv', $dversion,         PDO::PARAM_STR],
		[':dr', $resolution,       PDO::PARAM_STR],
		[':av', $appversion,       PDO::PARAM_STR],
		[':at', $apptype,          PDO::PARAM_STR],
	]);
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

function getSCCMHighscores($limit = 100, $page = 0) {
	return sql_query_assoc_prep('getSCCMHighscores', loadSQL("get-ranking_sccm_top"),
	[
		[':qlimit', $limit,         PDO::PARAM_INT],
		[':qpage',  $limit * $page, PDO::PARAM_INT],
	]);
}

function getStarsHighscores($limit = 100, $page = 0) {
	return sql_query_assoc_prep('getStarsHighscores', loadSQL("get-ranking_stars_top"),
	[
		[':qlimit', $limit,         PDO::PARAM_INT],
		[':qpage',  $limit * $page, PDO::PARAM_INT],
	]);
}

function getSCCMRank($userid) {
	return sql_query_num_prep('getSCCMRank', loadSQL("get-ranking_sccm_playerrank"),
		[
			[':uid', $userid, PDO::PARAM_INT],
		]);
}

function getStarsRank($userid) {
	return sql_query_num_prep('getStarsRank', loadSQL("get-ranking_stars_playerrank"),
		[
			[':uid', $userid, PDO::PARAM_INT],
		]);
}

function getAllEntries($page, $pagesize) {
	return sql_query_assoc_prep('getAllEntries', "SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid LEFT JOIN idmap ON level_highscores.shortid=idmap.shortid WHERE score > 0 LIMIT :ps OFFSET :po",
	[
		[':po', $pagesize * $page, PDO::PARAM_INT],
		[':ps', $pagesize,         PDO::PARAM_INT],
	]);
}

function getLevelEntries($lvl) {
	return sql_query_assoc_prep('getLevelEntries', "SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid LEFT JOIN idmap ON level_highscores.shortid=idmap.shortid WHERE levelid= :id AND score > 0",
	[
		[':id', $lvl, PDO::PARAM_STR],
	]);
}

function getLevelDiffEntries($lvl, $diff, $limit) {
	return sql_query_assoc_prep('getLevelDiffEntries', "SELECT * FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid LEFT JOIN idmap ON level_highscores.shortid=idmap.shortid WHERE levelid= :id AND difficulty = :diff AND score > 0 ORDER BY level_highscores.best_time ASC, level_highscores.last_changed ASC LIMIT :lim",
	[
		[':id', $lvl, PDO::PARAM_STR],
		[':diff', $diff, PDO::PARAM_STR],
		[':lim', $limit, PDO::PARAM_INT],
	]);
}

function getUserEntries($uid) {
	return sql_query_assoc_prep('getUserEntries', "SELECT level_highscores.*, users.*, idmap.levelid FROM level_highscores LEFT JOIN users ON level_highscores.userid = users.userid LEFT JOIN idmap ON level_highscores.shortid = idmap.shortid WHERE level_highscores.userid= :uid AND score > 0",
	[
		[':uid', $uid, PDO::PARAM_INT],
	]);
}

function getWorldHighscores($worldid, $limit=100, $page=0) {
	return sql_query_assoc_prep('getWorldHighscores', loadReplSQL('get-ranking_local_top',
	[
		['FIELD_SCORE', 'users.score_' . worldGuidToSQLField($worldid)],
		['FIELD_TIME', 'users.time_' . worldGuidToSQLField($worldid)],
	]),
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

		if (is_dir($filepath)) continue;

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

	return "WnF";
}

function getScoreDistribution($partitionsize) {
	return sql_query_assoc_prep('getScoreDistribution', "SELECT q.xscore AS score, count FROM (SELECT CEIL(score/:ps1)*:ps2 AS xscore, COUNT(*) AS count FROM users WHERE score > 0 GROUP BY xscore) AS q",
	[
		[':ps1', $partitionsize, PDO::PARAM_INT],
		[':ps2', $partitionsize, PDO::PARAM_INT],
	]);
}

function getNewUsersDistribution() {
	return sql_query_assoc('getNewUsersDistribution', "SELECT date(creation_time) AS date, COUNT(*) AS count FROM users WHERE score>0 AND creation_time >= now() - INTERVAL 1 YEAR GROUP BY date(creation_time)");
}

function getNewUsersToday() {
	return sql_query_num('getNewUsersToday', "SELECT COUNT(*) AS count FROM users WHERE score>0 AND DATE(creation_time) = DATE(now())");
}

function getEntryChangedDistribution() {
	return sql_query_assoc('getEntryChangedDistribution', "SELECT date(last_changed) AS date, COUNT(*) AS count FROM level_highscores WHERE last_changed >= now() - INTERVAL 1 YEAR GROUP BY date(last_changed)");
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
	$u5[] = ['name' => '{d34db335-0001-4000-7711-000000300002}', 'count' => sql_query_num('statisticsUserByUnlocks_W', "SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000300001}%'")];

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

function statisticsUserByWorld() {
	return
	[
		[
			'world' => '{d34db335-0001-4000-7711-000000100001}',
			'count' => sql_query_num('statisticsUserByWorld_0', "SELECT COUNT(*) FROM users WHERE (score_w1+score_w2+score_w3+score_w4)<>score")
		],
		[
			'world' => '{d34db335-0001-4000-7711-000000200001}',
			'count' => sql_query_num('statisticsUserByWorld_1', "SELECT COUNT(*) FROM users WHERE score_w1>0")
		],
		[
			'world' => '{d34db335-0001-4000-7711-000000200002}',
			'count' => sql_query_num('statisticsUserByWorld_2', "SELECT COUNT(*) FROM users WHERE score_w2>0")
		],
		[
			'world' => '{d34db335-0001-4000-7711-000000200003}',
			'count' => sql_query_num('statisticsUserByWorld_3', "SELECT COUNT(*) FROM users WHERE score_w3>0")
		],
		[
			'world' => '{d34db335-0001-4000-7711-000000200004}',
			'count' => sql_query_num('statisticsUserByWorld_4', "SELECT COUNT(*) FROM users WHERE score_w4>0")
		],
	];
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
	return sql_query_assoc_prep('getLocalPlayerRank', loadReplSQL('get-ranking_local_playerrank',
	[
		['FIELD_SCORE', 'users.score_' . worldGuidToSQLField($world)],
		['FIELD_TIME', 'users.time_' . worldGuidToSQLField($world)],
	]),
	[
		[':uid', $uid, PDO::PARAM_INT],
	]);
}

function getUserData($uid) {
	return sql_query_assoc_prep('getUserData', "SELECT * FROM users WHERE userid=:id LIMIT 1",
	[
		[':id', $uid, PDO::PARAM_INT],
	])[0];
}

function getErrorData($uid) {
	return sql_query_assoc_prep('getErrorData', "SELECT * FROM error_log WHERE error_id=:id LIMIT 1",
	[
		[':id', $uid, PDO::PARAM_INT],
	])[0];
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
	return sql_query_assoc_prep('getRunLog', "SELECT * FROM runlog_history WHERE action = :ac AND exectime >= now() - INTERVAL 28 DAY ORDER BY exectime DESC",
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

function getActiveAndTotalSessionsCount() {
	if (! file_exists("/var/log/gdapi_log/proxystate.json")) return ["?", "?"];

	$string = file_get_contents("/var/log/gdapi_log/proxystate.json");
	$json = json_decode($string, true);

	$c = 0;
	foreach ($json['sessions'] as $entry) {
		if ($entry['act']) $c++;
	}

	return [$c, count($json['sessions'])];
}

function getProxyHistory() {
	return sql_query_assoc('getProxyHistory', "SELECT * FROM (SELECT * FROM session_history ORDER BY id DESC LIMIT 500) AS a ORDER BY a.ID ASC");
}

function getLastProxyHistoryEntry() {
	return sql_query_assoc('getLastProxyHistoryEntry', "SELECT * FROM session_history ORDER BY id DESC LIMIT 1")[0];
}

function countZombies() {
	return sql_query_num('countZombies', "SELECT COUNT(*) FROM ( SELECT * FROM users AS u0 WHERE score=0 AND is_auto_generated=1 AND mpscore=0 AND last_online < NOW() - INTERVAL 100 DAY AND ping_counter<=2 AND 0 = (select count(*) FROM error_log where error_log.userid = u0.userid) AND 0 = (select count(*) FROM cache_levels where cache_levels.best_userid = u0.userid) AND 0 = (select count(*) FROM level_highscores where level_highscores.userid = u0.userid) ) AS taball");
}

function getLastStats($rows) { return sql_query_assoc("getLastStats", "SELECT * FROM stats_history ORDER BY exectime DESC LIMIT ".$rows); }

function getPuchaseDelta() {
	$d = getLastStats(3);

	$now = $d[0]['user_amazon'] + $d[0]['user_android_full'] + $d[0]['user_ios'] + $d[0]['user_winphone'];
	$old = $d[2]['user_amazon'] + $d[2]['user_android_full'] + $d[2]['user_ios'] + $d[2]['user_winphone'];

	return '+' . ($now - $old);
}

function getUnlockDelta() {
	$d = getLastStats(3);

	$now = $d[0]['unlocks_w1'] + $d[0]['unlocks_w2'] + $d[0]['unlocks_w3'] + $d[0]['unlocks_w4'] + $d[0]['unlocks_mp'] + $d[0]['unlocks_sccm'];
	$old = $d[2]['unlocks_w1'] + $d[2]['unlocks_w2'] + $d[2]['unlocks_w3'] + $d[2]['unlocks_w4'] + $d[2]['unlocks_mp'] + $d[0]['unlocks_sccm'];

	return '+' . ($now - $old);
}

function getPuchaseTotal() {
	$d = getLastStats(3);

	return $d[0]['user_amazon'] + $d[0]['user_android_full'] + $d[0]['user_ios'] + $d[0]['user_winphone'];
}

function getUnlockTotal() {
	$d = getLastStats(3);

	return $d[0]['unlocks_w1'] + $d[0]['unlocks_w2'] + $d[0]['unlocks_w3'] + $d[0]['unlocks_w4'] + $d[0]['unlocks_mp'];
}

function getUserSCCMEntries($uid) {
	return sql_query_assoc_prep('getUserSCCMEntries', "SELECT * FROM userlevels_highscores where userid = :uid",
	[
		[':uid', $uid, PDO::PARAM_INT],
	]);
}

function getLevelSCCMEntries($lid) {
	return sql_query_assoc_prep('getUserSCCMEntries', "SELECT uh.*, uu.username FROM userlevels_highscores AS uh LEFT JOIN users AS uu ON uh.userid=uu.userid where uh.levelid = :lid",
	[
		[':lid', $lid, PDO::PARAM_INT],
	]);
}

function getManualRecalculatedScoreStars($uid)
{
	return sql_query_num_prep('getManualRecalculatedScoreSCCM', 'SELECT COUNT(uh.userid) FROM userlevels_highscores AS uh LEFT JOIN userlevels AS ul ON uh.levelid=ul.id WHERE uh.starred=1 AND ul.userid=:uid',
	[
		[':uid', $uid, PDO::PARAM_INT],
	]);
}

function getManualRecalculatedScoreSCCM($uid)
{
	global $config;
	/*
	SELECT max_diff AS diff, COUNT(max_diff) AS levelcount FROM
		(
			SELECT

			GREATEST
			(
				(CASE WHEN d0_time IS NULL THEN -1 ELSE 0 END),
				(CASE WHEN d1_time IS NULL THEN -1 ELSE 1 END),
				(CASE WHEN d2_time IS NULL THEN -1 ELSE 2 END),
				(CASE WHEN d3_time IS NULL THEN -1 ELSE 3 END)
			) AS max_diff

			FROM userlevels_highscores

			WHERE userid=:uid
		) AS score_greatest

		WHERE max_diff <> -1

		GROUP BY max_diff
	 */
	$scoresummary = sql_query_assoc_prep('getManualRecalculatedScoreSCCM', 'SELECT max_diff AS diff, COUNT(max_diff) AS levelcount FROM(SELECT GREATEST((CASE WHEN d0_time IS NULL THEN -1 ELSE 0 END),(CASE WHEN d1_time IS NULL THEN -1 ELSE 1 END),(CASE WHEN d2_time IS NULL THEN -1 ELSE 2 END),(CASE WHEN d3_time IS NULL THEN -1 ELSE 3 END)) AS max_diff FROM userlevels_highscores WHERE userid=:uid) AS score_greatest WHERE max_diff <> -1 GROUP BY max_diff',
	[
		[':uid', $uid, PDO::PARAM_INT],
	]);

	$newscore = 0;
	foreach ($scoresummary as $ss) {
		for ($i=$ss['diff']; $i >=0; $i--) $newscore += $ss['levelcount'] * $config['diff_scores'][$i];
	}

	return $newscore;
}

function getUsernameOrEmpty($uid)
{
	if ($uid === null) return "N/A";

	$n = sql_query_assoc_prep('getUsernameOrEmpty', 'SELECT username from users WHERE userid=:uid',
	[
		[':uid', $uid, PDO::PARAM_INT],
	]);

	if ($n === FALSE) return "N/A";
	if ($n === NULL) return "N/A";
	if (empty($n)) return "N/A";
	if (empty($n[0])) return "N/A";

	return $n[0]['username'];
}

function GetSCCMLevelCount()
{
	return sql_query_num('GetSCCMLevelCount', 'SELECT COUNT(*) FROM userlevels WHERE upload_timestamp IS NOT NULL');
}

function GetSCCMLevelCountAll()
{
	return sql_query_num('GetSCCMLevelCount', 'SELECT COUNT(*) FROM userlevels');
}

function GetSCCMLevelInfo($levelid)
{
	return sql_query_assoc_prep('GetSCCMLevelInfo', 'SELECT ul.*, ux.username, (ul.stars / POW(TIMESTAMPDIFF(HOUR, ul.upload_timestamp, NOW()),1.8)) AS hot_ranking FROM userlevels AS ul LEFT JOIN users AS ux ON ux.userid=ul.userid WHERE id=:lid',
	[
		[':lid', $levelid, PDO::PARAM_INT],
	])[0];
}

function GetSCCMLevelMetadataRecalculated($levelid)
{			
	$recalc = []; 
	$recalc['d0_completed']  = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT COUNT(d0_time) FROM userlevels_highscores WHERE levelid=:lid', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d1_completed']  = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT COUNT(d1_time) FROM userlevels_highscores WHERE levelid=:lid', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d2_completed']  = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT COUNT(d2_time) FROM userlevels_highscores WHERE levelid=:lid', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d3_completed']  = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT COUNT(d3_time) FROM userlevels_highscores WHERE levelid=:lid', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d0_played']     = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT COUNT(d0_lastplayed) FROM userlevels_highscores WHERE levelid=:lid', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d1_played']     = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT COUNT(d1_lastplayed) FROM userlevels_highscores WHERE levelid=:lid', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d2_played']     = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT COUNT(d2_lastplayed) FROM userlevels_highscores WHERE levelid=:lid', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d3_played']     = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT COUNT(d3_lastplayed) FROM userlevels_highscores WHERE levelid=:lid', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d0_bestuserid'] = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT userid FROM userlevels_highscores WHERE levelid=:lid AND d0_time IS NOT NULL ORDER BY d0_time ASC LIMIT 1', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d1_bestuserid'] = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT userid FROM userlevels_highscores WHERE levelid=:lid AND d1_time IS NOT NULL ORDER BY d1_time ASC LIMIT 1', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d2_bestuserid'] = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT userid FROM userlevels_highscores WHERE levelid=:lid AND d2_time IS NOT NULL ORDER BY d2_time ASC LIMIT 1', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d3_bestuserid'] = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT userid FROM userlevels_highscores WHERE levelid=:lid AND d3_time IS NOT NULL ORDER BY d3_time ASC LIMIT 1', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d0_besttime']   = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT MIN(d0_time) FROM userlevels_highscores WHERE levelid=:lid AND d0_time IS NOT NULL', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d1_besttime']   = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT MIN(d1_time) FROM userlevels_highscores WHERE levelid=:lid AND d1_time IS NOT NULL', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d2_besttime']   = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT MIN(d2_time) FROM userlevels_highscores WHERE levelid=:lid AND d2_time IS NOT NULL', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['d3_besttime']   = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT MIN(d3_time) FROM userlevels_highscores WHERE levelid=:lid AND d3_time IS NOT NULL', [[':lid', $levelid, PDO::PARAM_INT]]);
	$recalc['stars']         = sql_query_num_prep('GetSCCMLevelMetadataRecalculated', 'SELECT COUNT(*) FROM userlevels_highscores WHERE levelid=:lid AND starred=1', [[':lid', $levelid, PDO::PARAM_INT]]);
	return $recalc;
}

function GetSCCMLevelByNew($max, $page, $all)
{
	global $config;
	return sql_query_assoc_prep('GetSCCMLevelByNew', loadReplSQL($all ? "get-all-userlevel-by-new" : "get-userlevel-by-new", [['HOT_FACTOR', $config['hot_factor']]]),
	[
		[':lim', $max,       PDO::PARAM_INT],
		[':off', $page*$max, PDO::PARAM_INT],
	]);
}

function GetSCCMLevelByUser($uid)
{
	return sql_query_assoc_prep('GetSCCMLevelByUser', 'SELECT * FROM userlevels WHERE userid=:uid',
		[
			[':uid', $uid,       PDO::PARAM_INT],
		]);
}

function GetSCCMLevelSize()
{
	return sql_query_num('GetSCCMLevelSize', 'SELECT SUM(filesize) FROM userlevels WHERE upload_timestamp IS NOT NULL');
}