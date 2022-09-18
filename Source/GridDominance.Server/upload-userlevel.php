<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamSHAOrError('password');
	$appversion    = getParamStrOrError('app_version');
	$decappversion = getParamLongOrError('app_version_dec');
	$levelid       = getParamLongOrError('levelid');
	$name          = getParamStrOrError('name');
	$gwidth        = getParamStrOrError('gwidth');
	$gheight       = getParamStrOrError('gheight');
	$authortime    = getParamStrOrError('authortime');
	$binhash       = getParamStrOrError('binhash');
	$bindata       = getParamDeflBinaryOrError('bindata');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion, $decappversion, $levelid, $name, $gwidth, $gheight, $authortime, $binhash]);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password, $userid);

	//----------

	$user->UpdateLastOnline($appversion);

	//----------

	if ($user->AutoUser)
	{
		outputError(ERRORS::LEVELUPLOAD_ANONUSER, "Only registered users can create levels", LOGLEVEL::ERROR);
	}

	//----------

	$stmt = $pdo->prepare("SELECT * FROM userlevels WHERE id = :lid");
	$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
	executeOrFail($stmt);
	$dblevel = $stmt->fetch(PDO::FETCH_ASSOC);

	$stmt = $pdo->prepare("SELECT * FROM userlevels WHERE LOWER(name)=LOWER(:nam) AND id <> :lid AND upload_version IS NOT NULL LIMIT 1");
	$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
	$stmt->bindValue(':nam', $name,    PDO::PARAM_STR);
	executeOrFail($stmt);
	$namecollision = $stmt->fetch(PDO::FETCH_ASSOC);

	if ($dblevel === FALSE) outputError(ERRORS::LEVELUPLOAD_LEVELID_NOT_FOUND, "No level with id $levelid found", LOGLEVEL::ERROR);

	if ($dblevel['userid'] !== $userid) outputError(ERRORS::LEVELUPLOAD_WRONG_USERID, "Level $levelid was created from different user (".$dblevel['userid']." !== ".$userid.")", LOGLEVEL::ERROR);

	if ($dblevel['upload_version'] !== null) outputError(ERRORS::LEVELUPLOAD_ALREADY_UPLOADED, "Level $levelid was already uploaded", LOGLEVEL::ERROR);

	if (strlen($bindata)>$config['userlevel_maxsize']) outputError(ERRORS::LEVELUPLOAD_FILE_TOO_BIG, "Uploaded Level $levelid from $userid too big (".strlen($bindata).")", LOGLEVEL::ERROR);

	if (strlen($name) === 0 || strlen($name)>32) outputError(ERRORS::LEVELUPLOAD_INVALID_NAME, "Name '$name' is not allowed as an levelname", LOGLEVEL::ERROR);

	$realhash = strtoupper(hash('sha256', $bindata));
	if ($realhash !== $binhash) outputError(ERRORS::LEVELUPLOAD_HASH_MISMATCH, "Hash '$realhash' <> '$binhash'", LOGLEVEL::ERROR);

	if ($namecollision !== FALSE) outputError(ERRORS::LEVELUPLOAD_DUPLICATENAME, "Name '$name' is already user", LOGLEVEL::DEBUG);



	$stmt = $pdo->prepare("UPDATE userlevels SET name=:nam, upload_timestamp=NOW(), upload_version=:vrs, upload_decversion=:vdc, datahash=:hsh, filesize=:fsz, grid_width=:ggw, grid_height=:ggh, author_time=:att WHERE id=:lid");
	$stmt->bindValue(':lid', $levelid,         PDO::PARAM_INT);
	$stmt->bindValue(':nam', $name,            PDO::PARAM_STR);
	$stmt->bindValue(':vrs', $appversion,      PDO::PARAM_STR);
	$stmt->bindValue(':vdc', $decappversion,   PDO::PARAM_INT);
	$stmt->bindValue(':hsh', $binhash,         PDO::PARAM_STR);
	$stmt->bindValue(':fsz', strlen($bindata), PDO::PARAM_INT);
	$stmt->bindValue(':ggw', $gwidth,          PDO::PARAM_INT);
	$stmt->bindValue(':ggh', $gheight,         PDO::PARAM_INT);
	$stmt->bindValue(':att', $authortime,      PDO::PARAM_INT);
	executeOrFail($stmt);

	$filename = "{B16B00B5-0001-4001-0000-".str_pad(strtoupper(dechex($levelid)), 12, '0', STR_PAD_LEFT).'}';

	$success = file_put_contents($config['userlevel_directory'].$filename, $bindata);

	if ($success === FALSE)
	{
		outputError(ERRORS::INTERNAL_EXCEPTION, "Write Levelfile failed for $filename", LOGLEVEL::ERROR);
	}

	//----------

	logDebug("user $user->ID uploaded level $levelid ($binhash)");
	outputResultSuccess([]);
}



try {
	init("upload-userlevel");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}