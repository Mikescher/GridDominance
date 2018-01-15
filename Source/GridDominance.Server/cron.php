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

	foreach ($config['levelids'] as $lid) {

		foreach ($config['difficulties'] as $diff) {
			$stmt = $pdo->prepare("SELECT COUNT(*) FROM level_highscores WHERE levelid=:lid AND difficulty=:dif");
			$stmt->bindValue(':lid', $lid, PDO::PARAM_STR);
			$stmt->bindValue(':dif', $diff, PDO::PARAM_INT);
			executeOrFail($stmt);
			$compcount = $stmt->fetchColumn();

			if ($compcount > 0) {
				$stmt = $pdo->prepare("REPLACE INTO cache_levels (levelid, difficulty, best_time, best_userid, best_last_changed, completion_count) (SELECT levelid, difficulty, best_time, userid, last_changed, :coc FROM level_highscores WHERE levelid=:lid AND difficulty=:dif ORDER BY best_time ASC, last_changed ASC LIMIT 1)");
				$stmt->bindValue(':lid', $lid, PDO::PARAM_STR);
				$stmt->bindValue(':dif', $diff, PDO::PARAM_INT);
				$stmt->bindValue(':coc', $compcount, PDO::PARAM_INT);
				executeOrFail($stmt);
			} else {
				$stmt = $pdo->prepare("DELETE FROM cache_levels WHERE levelid=:lid AND difficulty=:dif");
				$stmt->bindValue(':lid', $lid, PDO::PARAM_STR);
				$stmt->bindValue(':dif', $diff, PDO::PARAM_INT);
				executeOrFail($stmt);
			}
		}

		echo ("[" . date("Y-m-d H:i:s") . "]  " . $lid . "  <br/>\n");
	}

	if ($config['runlog'])
	{
		$stmt = $pdo->prepare("INSERT INTO runlog_history (action, min_timestamp, max_timestamp, count, duration, duration_min, duration_max, duration_avg, duration_median) (SELECT action, MIN(exectime), MAX(exectime), COUNT(*), SUM(duration), MIN(duration), MAX(duration), AVG(duration), MEDIAN(duration) FROM runlog_volatile GROUP BY action)");
		executeOrFail($stmt);

		echo ("[" . date("Y-m-d H:i:s") . "]  " . "Runlog(1)" . "  <br/>\n");
		
		$stmt = $pdo->prepare("DELETE FROM runlog_volatile");
		executeOrFail($stmt);

		echo ("[" . date("Y-m-d H:i:s") . "]  " . "Runlog(2)" . "  <br/>\n");


		$stmt = $pdo->prepare("INSERT INTO stats_history (active_users_per_day, user_amazon, user_android_full, user_android_iab, user_ios, user_winphone, unlocks_w1, unlocks_w2, unlocks_w3, unlocks_w4, unlocks_mp, user_topscore) " .
			"VALUES  " .
			"( " .
			"(SELECT COUNT(*) FROM users WHERE score > 0 AND last_online >= now() - INTERVAL 1 DAY), " .
			"(SELECT COUNT(*) FROM users WHERE app_type = 'Android.Amazon'), " .
			"(SELECT COUNT(*) FROM users WHERE app_type = 'Android.Full'), " .
			"(SELECT COUNT(*) FROM users WHERE app_type = 'Android.IAB'), " .
			"(SELECT COUNT(*) FROM users WHERE app_type = 'IOS.Full'), " .
			"(SELECT COUNT(*) FROM users WHERE app_type = 'WinPhone.UWP.Full'), " .
			"(SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000200001}%'), " .
			"(SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000200002}%'), " .
			"(SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000200003}%'), " .
			"(SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000200004}%'), " .
			"(SELECT COUNT(*) AS count FROM users WHERE score>0 AND unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000300001}%'), " .
			"(SELECT COUNT(*) FROM users WHERE score = (SELECT MAX(score) FROM users)) " .
			")");
		executeOrFail($stmt);

		echo ("[" . date("Y-m-d H:i:s") . "]  " . "Runlog(3)" . "  <br/>\n");
	}

	$delta = (int)((microtime(true) - $time_start)*1000);
	$min = round($delta/(1000*60), 2);
	logDebug("Cronjob succesful executed in $delta ms.");
	logCron("Cronjob succesful executed in $delta ms (= $min min).");

	outputResultSuccess(['time' => $delta]);
}



try {
	init("cron");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}