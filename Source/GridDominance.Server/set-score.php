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
	$totalscore    = getParamUIntOrError('totalscore');
	$appversion    = getParamStrOrError('app_version');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion, $levelid, $difficulty, $leveltime, $totalscore]);

	if ($leveltime <= 0) outputError(ERRORS::SET_SCORE_INVALID_TIME, "The time $leveltime is not possible", LogLevel::MESSAGE);
	if (!in_array($difficulty, $config['difficulties'], TRUE)) outputError(ERRORS::SET_SCORE_INVALID_DIFF, "The difficulty $difficulty is not possible", LogLevel::MESSAGE);
	if (!in_array($levelid, $config['levelids'], TRUE)) outputError(ERRORS::SET_SCORE_INVALID_DIFF, "The levelID $levelid is not possible", LogLevel::MESSAGE);
	if ($totalscore < 0) outputError(ERRORS::SET_SCORE_INVALID_SCORE, "The score $totalscore is not possible", LogLevel::MESSAGE);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password, $userid);

	//----------

	$r = $user->InsertLevelScore($levelid, $difficulty, $leveltime);

	if ($r[0] == 1) {

		// better or same value in db
		logDebug("levelscore _not_ changed for user:$userid Level:$levelid::$difficulty to $r[1] ($leveltime) ms ($totalscore)");
		outputResultSuccess(['update' => false, 'value_db' => $r[1], 'user' => $user]);

	} else if ($r[0] == 2) {

		// existing row in db
		$user->SetScore($totalscore, $appversion);
		logDebug("levelscore changed (update) for user:$userid Level:$levelid::$difficulty to $leveltime ms ($totalscore)");
		outputResultSuccess(['update' => true, 'value_db' => $leveltime, 'user' => $user]);

	} else if ($r[0] == 3) {

		// no row in db
		$user->SetScore($totalscore, $appversion);
		logDebug("levelscore changed (insert) for user:$userid Level:$levelid::$difficulty to $leveltime ms ($totalscore)");
		outputResultSuccess(['update' => true, 'value_db' => $leveltime, 'user' => $user]);

	} else {

		throw new Exception('Unknown return value: $r[0]');

	}
}



try {
	init("set-score");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}