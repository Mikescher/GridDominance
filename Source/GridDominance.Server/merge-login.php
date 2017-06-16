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

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$old_userid, $old_password, $appversion, $username, $password, $mergedata]);

	//--------- step 1: [verify]

	$olduser = GDUser::QueryOrFail($pdo, $old_password, $old_userid); // old (anon) user
	$user    = GDUser::QueryOrFailByName($pdo, $password, $username); // new user

	$jdata = json_decode($mergedata);

	//--------- step 2: [get data of new_user]

	$changecount = $user->InsertMultiLevelScore($jdata);

	//---------- step 3: [update newuser]

	$user->SetScore($user->Score, $appversion);

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