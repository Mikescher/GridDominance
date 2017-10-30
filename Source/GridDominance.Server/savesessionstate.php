<?php

require 'internals/backend.php';

function run()
{
	global $pdo;

	$arr = getActiveAndTotalSessionsCount();
	$count_active = $arr[0];
	$count_total  = $arr[1];

	$last =getLastProxyHistoryEntry();

	if ($count_active == $last['sessioncount_active'] && $count_total == $last['sessioncount_total']) {
		outputResultSuccess(['insert' => false]);
	}

	if (is_numeric($count_active) && is_numeric($count_total)) {
		$stmt = $pdo->prepare("INSERT INTO session_history(sessioncount_active, sessioncount_total) VALUES (:ca, :ct) ");
		$stmt->bindValue(':ca', $count_active, PDO::PARAM_INT);
		$stmt->bindValue(':ct', $count_total,  PDO::PARAM_INT);
		executeOrFail($stmt);

		outputResultSuccess(['insert' => true]);
	} else {
		logError("savesessionstate failed cause arr = '" . print_r($arr, true) . "'", true);
	}
}



try {
	init("savesessionstate");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}