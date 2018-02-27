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
	$time          = getParamUIntOrError('time');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion, $levelid, $difficulty, $time]);
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

	$field_lastplayed    = 'd'.$difficulty.'_lastplayed';
	$field_time          = 'd'.$difficulty.'_time';
	$field_playcount     = 'd'.$difficulty.'_played';
	$field_besttime      = 'd'.$difficulty.'_besttime';
	$field_bestuserid    = 'd'.$difficulty.'_bestuserid';
	$field_besttimestamp = 'd'.$difficulty.'_besttimestamp';
	$field_completed     = 'd'.$difficulty.'_completed';


	$stmt = $pdo->prepare('SELECT '.$field_besttime.' FROM userlevels WHERE id = :lid');
	$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
	executeOrFail($stmt);
	$globalbest = $stmt->fetchColumn();
	if ($globalbest === null) $globalbest = -99999;

	if ($dblevel !== FALSE) // level already played (normal)
	{
		$firstplay = ($dblevel[$field_lastplayed] === null);

		if ($firstplay) // also update play count
		{
			$stmt = $pdo->prepare('UPDATE userlevels_highscores SET '.$field_lastplayed.' = NOW() WHERE userid = :uid AND levelid = :lid');
			$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
			$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
			executeOrFail($stmt);

			$stmt = $pdo->prepare('UPDATE userlevels SET '.$field_playcount.' = '.$field_playcount.' + 1 WHERE id=:lid');
			$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
			executeOrFail($stmt);
		}

		$scoreInc = 0;
		if ($dblevel['d0_time']==null && $difficulty <= 0) $scoreInc += $config['diff_scores'][0];
		if ($dblevel['d1_time']==null && $difficulty <= 1) $scoreInc += $config['diff_scores'][1];
		if ($dblevel['d2_time']==null && $difficulty <= 2) $scoreInc += $config['diff_scores'][2];
		if ($dblevel['d3_time']==null && $difficulty <= 3) $scoreInc += $config['diff_scores'][3];

		if ($dblevel[$field_time] !== null) // first clear
		{
			$stmt = $pdo->prepare('UPDATE userlevels_highscores SET '.$field_time.' = :tim WHERE userid = :uid AND levelid = :lid');
			$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
			$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
			$stmt->bindValue(':tim', $time, PDO::PARAM_INT);
			executeOrFail($stmt);

			$user->addSCCMPoints($scoreInc);

			$ishighscore = ($time > $globalbest);
			if ($ishighscore)
			{
				$stmt = $pdo->prepare('UPDATE userlevels SET '.$field_completed.' = '.$field_completed.'+1, '.$field_besttime.'=:tim '.$field_bestuserid.'=:uid, '.$field_besttimestamp.'=NOW() WHERE id = :lid');
				$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
				$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
				$stmt->bindValue(':tim', $time, PDO::PARAM_INT);
				executeOrFail($stmt);
			}
			else
			{
				$stmt = $pdo->prepare('UPDATE userlevels SET '.$field_completed.' = '.$field_completed.'+1 WHERE id = :lid');
				$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
				executeOrFail($stmt);
			}

			outputResultSuccess([ 'firstclear' => $globalbest<0, 'inserted' => true, 'highscore' => $ishighscore, 'user' => $user ]);
		}
		else if ($dblevel[$field_time] > $time) // already cleared - better time
		{
			$stmt = $pdo->prepare('UPDATE userlevels_highscores SET '.$field_time.' = :tim WHERE userid = :uid AND levelid = :lid');
			$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
			$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
			$stmt->bindValue(':tim', $time, PDO::PARAM_INT);
			executeOrFail($stmt);

			$ishighscore = ($time > $globalbest);
			if ($ishighscore)
			{
				$stmt = $pdo->prepare('UPDATE userlevels SET '.$field_completed.' = '.$field_completed.'+1, '.$field_besttime.'=:tim '.$field_bestuserid.'=:uid, '.$field_besttimestamp.'=NOW() WHERE id = :lid');
				$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
				$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
				$stmt->bindValue(':tim', $time, PDO::PARAM_INT);
				executeOrFail($stmt);
			}
			else
			{
				$stmt = $pdo->prepare('UPDATE userlevels SET '.$field_completed.' = '.$field_completed.'+1 WHERE id = :lid');
				$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
				executeOrFail($stmt);
			}

			outputResultSuccess([ 'firstclear' => false, 'inserted' => true, 'highscore' => $ishighscore, 'user' => $user ]);
		}
		else // already cleared - worse time
		{
			outputResultSuccess([ 'firstclear' => false, 'inserted' => false, 'highscore' => false, 'user' => $user ]);
		}
	}
	else // new entry (update-userlevel-played must have failed)
	{
		$stmt = $pdo->prepare('INSERT INTO userlevels_highscores (userid, levelid, '.$field_lastplayed.', '.$field_time.') VALUES (:uid, :lid, NOW(), :tim)');
		$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
		$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
		$stmt->bindValue(':tim', $time, PDO::PARAM_INT);
		executeOrFail($stmt);

		$scoreInc = 0;
		if ($difficulty <= 0) $scoreInc += $config['diff_scores'][0];
		if ($difficulty <= 1) $scoreInc += $config['diff_scores'][1];
		if ($difficulty <= 2) $scoreInc += $config['diff_scores'][2];
		if ($difficulty <= 3) $scoreInc += $config['diff_scores'][3];

		$user->addSCCMPoints($scoreInc);

		$ishighscore = ($time > $globalbest);
		if ($ishighscore)
		{
			$stmt = $pdo->prepare('UPDATE userlevels SET '.$field_playcount.' = '.$field_playcount.'+1, '.$field_completed.' = '.$field_completed.'+1, '.$field_besttime.'=:tim '.$field_bestuserid.'=:uid, '.$field_besttimestamp.'=NOW() WHERE id = :lid');
			$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
			$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
			$stmt->bindValue(':tim', $time, PDO::PARAM_INT);
			executeOrFail($stmt);
		}
		else
		{
			$stmt = $pdo->prepare('UPDATE userlevels SET '.$field_playcount.' = '.$field_playcount.'+1, '.$field_completed.' = '.$field_completed.'+1 WHERE id=:lid');
			$stmt->bindValue(':lid', $levelid, PDO::PARAM_INT);
			executeOrFail($stmt);
		}

		outputResultSuccess([ 'firstclear' => $globalbest<0, 'inserted' => true, 'highscore' => $ishighscore, 'user' => $user ]);
	}
}



try {
	init("update-userlevel-completed");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}