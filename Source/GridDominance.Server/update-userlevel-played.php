<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamSHAOrError('password');
	$appversion    = getParamStrOrError('app_version');
	$levelid       = getParamLongOrError('levelid');
	$difficulty    = getParamUIntOrError('difficulty');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion, $levelid, $difficulty]);
	if (!in_array($difficulty, $config['difficulties'], TRUE)) outputError(ERRORS::UPDATEUSERLEVEL_INVALID_DIFF, "The difficulty $difficulty is not possible", LogLevel::ERROR);

	//----------

	$user = GDUser::QueryOrFail($pdo, $password, $userid);
	$user->UpdateLastOnline($appversion);

	//----------

	$stmt = $pdo->prepare('SELECT * FROM userlevels_highscores WHERE userid = :uid AND levelid = :lid');
	$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
	$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
	executeOrFail($stmt);
	$dblevel = $stmt->fetch(PDO::FETCH_ASSOC);

	$field_lastplayed = 'd'.$difficulty.'_lastplayed';
	$field_playcount  = 'd'.$difficulty.'_played';

	if ($dblevel !== FALSE) // level already played
	{
		if ($dblevel[$field_lastplayed] !== null) // level+diff already played
		{
			outputResultSuccess([ 'updated' => false, 'inserted' => false, 'user' => $user ]);
		}
		else
		{
			$stmt = $pdo->prepare('UPDATE userlevels_highscores SET '.$field_lastplayed.' = NOW() WHERE userid = :uid AND levelid = :lid');
			$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
			$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
			executeOrFail($stmt);

			$stmt = $pdo->prepare('UPDATE userlevels SET '.$field_playcount.' = '.$field_playcount.' + 1 WHERE id=:lid');
			$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
			executeOrFail($stmt);

			// already played other diff
			outputResultSuccess([ 'updated' => true, 'inserted' => false, 'user' => $user ]);
		}
	}
	else // new entry
	{
		$stmt = $pdo->prepare('INSERT INTO userlevels_highscores (userid, levelid, '.$field_lastplayed.') VALUES (:uid, :lid, NOW())');
		$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
		$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
		executeOrFail($stmt);

		$stmt = $pdo->prepare('UPDATE userlevels SET '.$field_playcount.' = '.$field_playcount.' + 1 WHERE id=:lid');
		$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
		executeOrFail($stmt);

		// already played otehr diff
		outputResultSuccess([ 'updated' => true, 'inserted' => true, 'user' => $user ]);
	}
}



try {
	init("update-userlevel-played");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}