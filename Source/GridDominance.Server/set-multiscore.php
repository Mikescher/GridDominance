<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamSHAOrError('password');
	$appversion    = getParamStrOrError('app_version');
	$data          = getParamStrOrError('data');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion, $data]);

	//---------- step 1: [verify]

	$user = GDUser::QueryOrFail($pdo, $password, $userid);
	$jdata = json_decode($data);

	//---------- step 2: [insert new data]

	$changecount = $user->InsertMultiLevelScore($jdata);

	//---------- step 3: [return]

	if ($changecount > 0) {

		$user->SetScore($user->Score, $appversion);

		logDebug("levelscores batch-changed for user:$userid ($changecount changes) ($user->Score)");
		outputResultSuccess(['update' => true, 'updatecount' => $changecount, 'user' => $user]);
	} else {

		$user->UpdateLastOnline($appversion);

		logDebug("levelscores batch-changed for user:$userid (no changes)");
		outputResultSuccess(['update' => false, 'updatecount' => $changecount, 'user' => $user]);
	}
}



try {
	init("set-multiscore");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}