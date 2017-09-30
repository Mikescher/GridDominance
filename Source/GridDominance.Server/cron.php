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

	$stmt = $pdo->prepare(loadSQL("recalculate_cron_highscores"));
	executeOrFail($stmt);
	
	if ($config['runlog'])
	{
		$stmt = $pdo->prepare("INSERT INTO runlog_history (action, min_timestamp, max_timestamp, count, duration) (SELECT action, MIN(exectime), MAX(exectime), COUNT(*), SUM(duration) FROM runlog_volatile GROUP BY action)");
		executeOrFail($stmt);
		
		$stmt = $pdo->prepare("DELETE FROM runlog_volatile");
		executeOrFail($stmt);
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