<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	//----------

	$stmt = $pdo->prepare("SELECT level_highscores.userid AS userid, users.username AS username, users.score AS totalscore, SUM(level_highscores.best_time) AS totaltime FROM level_highscores INNER JOIN users ON users.userid = level_highscores.userid GROUP BY level_highscores.userid ORDER BY totalscore DESC, totaltime ASC, userid ASC LIMIT 100");
	executeOrFail($stmt);

	$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

	//----------

	logDebug("get-ranking request from " . ParamServerOrUndef('REMOTE_ADDR'));
	outputResultSuccess(["ranking" => $data]);
}

try {
	init("get-ranking");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}