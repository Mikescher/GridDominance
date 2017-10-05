<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamSHAOrError('password');
	$appversion    = getParamStrOrError('app_version');
	$data          = getParamDeflOrError('data');

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

	check_commit_signature($signature, [$userid, $password, $appversion, $data, $totalscore, $score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp]);

	//---------- step 1: [verify]

	$user = GDUser::QueryOrFail($pdo, $password, $userid);
	$jdata = decode_scoredata($data);

	//---------- step 2: [insert new data]

	$changecount = $user->InsertMultiLevelScore($jdata);

	//---------- step 3: [update score+time]

	$user->SetScoreAndTime($totalscore, $score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp, $appversion);
	logDebug("score and time changed (update) for user:$userid [[$score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp]]");

	//---------- step 4: [return]

	if ($changecount > 0) {
		logDebug("levelscores batch-changed for user:$userid ($changecount changes) ($user->Score)");
		outputResultSuccess(['update' => true, 'updatecount' => $changecount, 'user' => $user]);
	} else {
		logDebug("levelscores batch-changed for user:$userid (no changes)");
		outputResultSuccess(['update' => false, 'updatecount' => $changecount, 'user' => $user]);
	}
}



try {
	init("set-multiscore-2");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}