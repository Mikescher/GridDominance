<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	//----------

	$stmt = $pdo->prepare("SELECT levelid, difficulty, best_time, best_last_changed, best_userid, completion_count FROM cache_levels");
	executeOrFail($stmt);

	$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

	//----------

	logDebug("highscore request from " . ParamServerOrUndef('REMOTE_ADDR'));
	outputResultSuccess(["highscores" => $data]);
}

try {
	set_time_limit(20);
	init("get-highscores");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}