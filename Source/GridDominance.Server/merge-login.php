<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$old_userid    = getParamUIntOrError('old_userid');
	$old_password  = getParamSHAOrError('old_password');
	$appversion    = getParamStrOrError('app_version');
	$username      = getParamStrOrError('new_username');
	$password      = getParamSHAOrError('new_password');
	$mergedata     = getParamStrOrError('merge_data');

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

	check_commit_signature($signature, [$old_userid, $old_password, $appversion, $username, $password, $mergedata, $score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp]);

	//--------- step 1: [verify]

	$olduser = GDUser::QueryOrFail($pdo, $old_password, $old_userid); // old (anon) user
	$user    = GDUser::QueryOrFailByName($pdo, $password, $username); // new user

	$jdata = json_decode($mergedata);

	//--------- step 2: [get data of new_user]

	$changecount = $user->InsertMultiLevelScore($jdata);

	//---------- step 3: [update newuser]

	$user->SetScoreAndTime($totalscore, $score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp, $appversion);
	logDebug("score and time changed (update) for user:$user->ID [[$score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp]]");

	//---------- step 4: [download data]

	$finished_data = $user->GetAllLevelScoreEntries();

	//---------- step 5: [delete old user]

	$olduser->Delete();

	//---------- step 6: [return]

	logMessage("Account merge sucessful ($old_userid into $user->ID) with $changecount changes. Old account purged.");
	outputResultSuccess(['user' => $user, 'updatecount' => $changecount, 'scores' => $finished_data]);
}



try {
	init("merge-login");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}