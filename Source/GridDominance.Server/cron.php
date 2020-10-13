<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

	if ($config['debug']) ob_end_flush();

	set_time_limit(30 * 60); // 30min
	$time_start = microtime(true);

	$secret      = getParamStrOrError('cronsecret');

	if ($secret !== $config['cron-secret']) outputError(ERRORS::CRON_INTERNAL_ERR, "", LOGLEVEL::ERROR);

	foreach ($config['levelmapping'] as $klid => $arrlid) {

		foreach ($config['difficulties'] as $diff) {
			$stmt = $pdo->prepare("SELECT COUNT(*) FROM level_highscores WHERE shortid=:sid AND difficulty=:dif");
			$stmt->bindValue(':sid', $arrlid[2], PDO::PARAM_INT);
			$stmt->bindValue(':dif', $diff, PDO::PARAM_INT);
			executeOrFail($stmt);
			$compcount = $stmt->fetchColumn();

			if ($compcount > 0) {
				$stmt = $pdo->prepare("REPLACE INTO cache_levels (levelid, difficulty, best_time, best_userid, best_last_changed, completion_count) (SELECT :lid, difficulty, best_time, userid, last_changed, :coc FROM level_highscores WHERE shortid=:sid AND difficulty=:dif ORDER BY best_time ASC, last_changed ASC LIMIT 1)");
				$stmt->bindValue(':lid', $arrlid[1], PDO::PARAM_STR);
				$stmt->bindValue(':sid', $arrlid[2], PDO::PARAM_INT);
				$stmt->bindValue(':dif', $diff, PDO::PARAM_INT);
				$stmt->bindValue(':coc', $compcount, PDO::PARAM_INT);
				executeOrFail($stmt);
			} else {
				$stmt = $pdo->prepare("DELETE FROM cache_levels WHERE levelid=:lid AND difficulty=:dif");
				$stmt->bindValue(':lid', $arrlid[1], PDO::PARAM_STR);
				$stmt->bindValue(':dif', $diff, PDO::PARAM_INT);
				executeOrFail($stmt);
			}
		}

		echo ("[" . date("Y-m-d H:i:s") . "]  " . $arrlid[1]. "  <br/>\n");
	}

	if ($config['runlog'])
	{
		// median windows functions needs mariadb
		// https://mariadb.com/kb/en/median/
		$stmt = $pdo->prepare("INSERT INTO runlog_history (action, min_timestamp, max_timestamp, count, duration, duration_min, duration_max, duration_avg, duration_median) (SELECT action, MIN(exectime), MAX(exectime), COUNT(*), SUM(duration), MIN(duration), MAX(duration), AVG(duration), median_calc FROM (SELECT *, (MEDIAN(duration) OVER (PARTITION BY action)) AS median_calc FROM runlog_volatile) AS t1 GROUP BY action)");
		executeOrFail($stmt);

		echo ("[" . date("Y-m-d H:i:s") . "]  " . "Runlog(1)" . "  <br/>\n");

		$stmt = $pdo->prepare("DELETE FROM runlog_volatile");
		executeOrFail($stmt);

		echo ("[" . date("Y-m-d H:i:s") . "]  " . "Runlog(2)" . "  <br/>\n");

		$avcomp = "(app_version = '" . $config['latest_version'] . "')";
		if ($config['latest_version_alt'] != NULL) $avcomp = "(app_version = '" . $config['latest_version'] . "' OR  app_version = '" . $config['latest_version_alt'] . "')";

		$stmt = $pdo->prepare("INSERT INTO stats_history (active_users_per_day, user_amazon, user_android_full, user_android_iab, user_ios, user_winphone, unlocks_w1, unlocks_w2, unlocks_w3, unlocks_w4, unlocks_mp, unlocks_sccm, user_topscore, user_current_version, user_old_version) " .
			"VALUES  " .
			"( " .
			"(SELECT COUNT(*) FROM users WHERE score > 0 AND last_online >= now() - INTERVAL 1 DAY), " .
			"(SELECT COUNT(*) FROM users WHERE app_type = 'Android.Amazon' AND score > 0), " .
			"(SELECT COUNT(*) FROM users WHERE app_type = 'Android.Full' AND score > 0), " .
			"(SELECT COUNT(*) FROM users WHERE app_type = 'Android.IAB' AND score > 0), " .
			"(SELECT COUNT(*) FROM users WHERE app_type = 'IOS.Full' AND score > 0), " .
			"(SELECT COUNT(*) FROM users WHERE app_type = 'WinPhone.UWP.Full' AND score > 0), " .
			"(SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000200001}%'), " .
			"(SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000200002}%'), " .
			"(SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000200003}%'), " .
			"(SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000200004}%'), " .
			"(SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000300001}%'), " .
			"(SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000300002}%'), " .
			"(SELECT COUNT(*) FROM users WHERE score = (SELECT MAX(score) FROM users)), " .
			"(SELECT COUNT(*) FROM users WHERE last_online > Now() - INTERVAL 1 DAY AND $avcomp), " .
			"(SELECT COUNT(*) FROM users WHERE last_online > Now() - INTERVAL 1 DAY AND !($avcomp)) " .
			")");
		executeOrFail($stmt);

		echo ("[" . date("Y-m-d H:i:s") . "]  " . "Runlog(3)" . "  <br/>\n");
	}

	// ProxyServer health check
	if (!isProxyActive())
	{
		logError("::ProxyHealthCheck:: Proxy server is not active - proxystate too old or not found.");
	}

	$delta = (int)((microtime(true) - $time_start)*1000);
	$min = round($delta/(1000*60), 2);
	logDebug("Cronjob succesful executed in $delta ms.");
	logCron("Cronjob succesful executed in $delta ms (= $min min).");

	outputResultSuccess(['time' => $delta]);
}



try {
	init("cron", true);
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}