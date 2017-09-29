<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

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

		echo ($lid . "\n");
	}

	if ($config['runlog'])
	{
		$stmt = $pdo->prepare("INSERT INTO runlog_history (action, min_timestamp, max_timestamp, count, duration, duration_min, duration_max) (SELECT action, MIN(exectime), MAX(exectime), COUNT(*), SUM(duration), MIN(duration), MAX(duration) FROM runlog_volatile GROUP BY action)");
		executeOrFail($stmt);
		
		$stmt = $pdo->prepare("DELETE FROM runlog_volatile");
		executeOrFail($stmt);
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