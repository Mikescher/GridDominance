<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamSHAOrError('password');
	$appversion    = getParamStrOrError('app_version');

	$totalscore    = getParamUIntOrError('s0');
	$score_w1      = getParamUIntOrError('s1');
	$score_w2      = getParamUIntOrError('s2');
	$score_w3      = getParamUIntOrError('s3');
	$score_w4      = getParamUIntOrError('s4');
	$totaltime     = getParamUIntOrError('t0');
	$time_w1       = getParamUIntOrError('t1');
	$time_w2       = getParamUIntOrError('t2');
	$time_w3       = getParamUIntOrError('t3');
	$time_w4       = getParamUIntOrError('t4');
	$score_mp      = getParamUIntOrError('sx');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion, $totalscore, $score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp]);

	if ($score_mp < 0) outputError(ERRORS::SET_SCORE_INVALID_SCORE, "The score $score_mp is not possible", LogLevel::MESSAGE);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password, $userid);

	//----------

	$user->SetScoreAndTime($totalscore, $score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp, $appversion);
	logDebug("score and time changed (update) for user:$userid [[$score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp]]");

	outputResultSuccess(['update' => true, 'user' => $user]);
}



try {
	//set_time_limit(45);
	init("set-mpscore");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}