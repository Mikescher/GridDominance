<?php

require 'internals/backend.php';


function run() {
	global $pdo;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamSHAOrError('password');
	$appversion    = getParamStrOrError('app_version');
	$levelid       = getParamLongOrError('levelid');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion, $levelid]);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password, $userid);
	$user->UpdateLastOnline($appversion);

	//----------

	$data = GDCustomLevel::getByID($levelid);

	outputResultSuccess([ 'data' => $data ]);
}



try {
	init("query-userlevel-meta");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}