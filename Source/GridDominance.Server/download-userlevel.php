<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamSHAOrError('password');
	$levelid       = getParamLongOrError('levelid');
	$appversion    = getParamStrOrError('app_version');
	$decappversion = getParamLongOrError('app_version_dec');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $levelid, $appversion, $decappversion]);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password, $userid);

	//----------

	$stmt = $pdo->prepare("SELECT * FROM userlevels WHERE id = :lid");
	$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
	executeOrFail($stmt);
	$dblevel = $stmt->fetch(PDO::FETCH_ASSOC);

	if ($dblevel === FALSE) outputError(ERRORS::LEVELDOWNLOAD_LEVELID_NOT_FOUND, "No level with id $levelid found", LOGLEVEL::ERROR);

	if ($dblevel['upload_version'] === null) outputError(ERRORS::LEVELDOWNLOAD_LEVELID_NOT_FOUND, "Level $levelid was already uploaded", LOGLEVEL::ERROR);

	$levelid = "{B16B00B5-0001-4001-0000-".str_pad(strtoupper(dechex($levelid)), 12, '0', STR_PAD_LEFT).'}';

	$content = file_get_contents($config['userlevel_directory'].$levelid);

	$b64content = base64_encode($content);

	logDebug("user $user->ID downloaded level $levelid");
	outputResultSuccess([ 'levelid' => $levelid, 'content' => $b64content]);
}



try {
	init("download-userlevel");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}