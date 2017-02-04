<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

	set_time_limit(10 * 60); // 10min
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
				$stmt = $pdo->prepare("SELECT userid, best_time, last_changed FROM level_highscores WHERE levelid=:lid AND difficulty=:dif ORDER BY best_time ASC, last_changed ASC LIMIT 1");
				$stmt->bindValue(':lid', $lid, PDO::PARAM_STR);
				$stmt->bindValue(':dif', $diff, PDO::PARAM_INT);
				executeOrFail($stmt);
				$compdata = $stmt->fetch(PDO::FETCH_ASSOC);

				$stmt = $pdo->prepare("REPLACE INTO cache_levels (levelid, difficulty, best_time, best_userid, best_last_changed, completion_count) VALUES (:lid, :dif, :btm, :bid, :blc, :coc)");
				$stmt->bindValue(':lid', $lid, PDO::PARAM_STR);
				$stmt->bindValue(':dif', $diff, PDO::PARAM_INT);
				$stmt->bindValue(':btm', $compdata['best_time'], PDO::PARAM_INT);
				$stmt->bindValue(':bid', $compdata['userid'], PDO::PARAM_INT);
				$stmt->bindValue(':blc', $compdata['last_changed'], PDO::PARAM_STR);
				$stmt->bindValue(':coc', $compcount, PDO::PARAM_INT);
				executeOrFail($stmt);
			} else {
				$stmt = $pdo->prepare("DELETE FROM cache_levels WHERE levelid=:lid AND difficulty=:dif");
				$stmt->bindValue(':lid', $lid, PDO::PARAM_STR);
				$stmt->bindValue(':dif', $diff, PDO::PARAM_INT);
				executeOrFail($stmt);
			}
		}
	}

	$delta = (int)((microtime(true) - $time_start)*1000);
	logMessage("Cronjob succesful executed in $delta ms.");

	outputResultSuccess(['time' => $delta]);
}



try {
	init("cron");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}