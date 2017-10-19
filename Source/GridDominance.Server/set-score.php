<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamSHAOrError('password');
	$levelid       = getParamStrOrError('levelid');
	$difficulty    = getParamUIntOrError('difficulty');
	$leveltime     = getParamUIntOrError('leveltime');
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

	check_commit_signature($signature, [$userid, $password, $appversion, $levelid, $difficulty, $leveltime, $totalscore, $score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp]);

	if ($leveltime <= 0) outputError(ERRORS::SET_SCORE_INVALID_TIME, "The time $leveltime is not possible", LogLevel::MESSAGE);
	if (!in_array($difficulty, $config['difficulties'], TRUE)) outputError(ERRORS::SET_SCORE_INVALID_DIFF, "The difficulty $difficulty is not possible", LogLevel::MESSAGE);
	if (!in_array($levelid, $config['levelids'], TRUE)) outputError(ERRORS::SET_SCORE_INVALID_DIFF, "The levelID $levelid is not possible", LogLevel::MESSAGE);
	if ($totalscore < 0) outputError(ERRORS::SET_SCORE_INVALID_SCORE, "The score $totalscore is not possible", LogLevel::MESSAGE);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password, $userid);

	//----------

	$r = $user->InsertLevelScore($levelid, $difficulty, $leveltime);

	//----------

	$user->SetScoreAndTime($totalscore, $score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp, $appversion);
	logDebug("score and time changed (update) for user:$userid [[$score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp]]");

	if ($r[0] == 1) {

		// better or same value in db
		logDebug("levelscore _not_ changed for user:$userid Level:$levelid::$difficulty to $r[1] ($leveltime) ms ($totalscore)");
		outputResultSuccess(['update' => false, 'value_db' => $r[1], 'user' => $user]);

	} else if ($r[0] == 2) {

		// existing row in db
		logDebug("levelscore changed (update) for user:$userid Level:$levelid::$difficulty to $leveltime ms ($totalscore)");
		outputResultSuccess(['update' => true, 'value_db' => $leveltime, 'user' => $user]);

	} else if ($r[0] == 3) {

		// no row in db
		logDebug("levelscore changed (insert) for user:$userid Level:$levelid::$difficulty to $leveltime ms ($totalscore)");
		outputResultSuccess(['update' => true, 'value_db' => $leveltime, 'user' => $user]);

	} else {

		throw new Exception('Unknown return value: $r[0]');

	}
}



try {
	//set_time_limit(20);
	init("set-score");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}