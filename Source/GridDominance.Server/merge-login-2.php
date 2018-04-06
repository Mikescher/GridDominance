<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

	$old_userid    = getParamUIntOrError('old_userid');
	$old_password  = getParamSHAOrError('old_password');
	$appversion    = getParamStrOrError('app_version');
	$username      = getParamStrOrError('new_username');
	$password      = getParamSHAOrError('new_password');
	$mergedata     = getParamDeflOrError('merge_data');

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

	check_commit_signature($signature, [$old_userid, $old_password, $appversion, $username, $password, $mergedata, $totalscore, $score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp]);

	//--------- step 1: [verify]

	$olduser = GDUser::QueryOrFail($pdo, $old_password, $old_userid); // old (anon) user
	$user    = GDUser::QueryOrFailByName($pdo, $password, $username); // new user

	$jdata = json_decode($mergedata);

	//--------- step 2: [get data of new_user]

	$changecount = $user->InsertMultiLevelScore($jdata);

	//---------- step 3: [update newuser]

	$user->SetScoreAndTime($totalscore, $score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp, $appversion);
	logDebug("score and time changed (update) for user:$user->ID [[$score_w1, $score_w2, $score_w3, $score_w4, $totaltime, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp]]");

	//---------- step 3.5: [test for bullshit]

	if ($old_userid == $user->ID)
	{
		// fast return because this is basically a NO-OP

		$finished_data = $user->GetAllLevelScoreEntries();

		logError("Account merge is no-op from $old_userid to $user->ID.");
		outputResultSuccess(['user' => $user, 'updatecount' => $changecount, 'scores' => $finished_data]);
		return;
	}

	//---------- step 4: [migrate customlevelscores]

	$stmt = $pdo->prepare('SELECT * FROM userlevels_highscores WHERE userid = :uid');
	$stmt->bindValue(':uid', $olduser->ID, PDO::PARAM_INT);
	executeOrFail($stmt);
	$oldentries = $stmt->fetchAll(PDO::FETCH_ASSOC);

	foreach ($oldentries as $entry)
	{
		$stmt = $pdo->prepare(loadSQL("merge-userlevels-highscores-single"));
		$stmt->bindValue(':uid',  $user->ID,               PDO::PARAM_INT);
		$stmt->bindValue(':lid',  $entry['levelid'],       PDO::PARAM_INT);
		$stmt->bindValue(':d0t1', $entry['d0_time'],       PDO::PARAM_INT);
		$stmt->bindValue(':d0p1', $entry['d0_lastplayed'], PDO::PARAM_STR);
		$stmt->bindValue(':d1t1', $entry['d1_time'],       PDO::PARAM_INT);
		$stmt->bindValue(':d1p1', $entry['d1_lastplayed'], PDO::PARAM_STR);
		$stmt->bindValue(':d2t1', $entry['d2_time'],       PDO::PARAM_INT);
		$stmt->bindValue(':d2p1', $entry['d2_lastplayed'], PDO::PARAM_STR);
		$stmt->bindValue(':d3t1', $entry['d3_time'],       PDO::PARAM_INT);
		$stmt->bindValue(':d3p1', $entry['d3_lastplayed'], PDO::PARAM_STR);
		$stmt->bindValue(':d0t2', $entry['d0_time'],       PDO::PARAM_INT);
		$stmt->bindValue(':d0p2', $entry['d0_lastplayed'], PDO::PARAM_STR);
		$stmt->bindValue(':d1t2', $entry['d1_time'],       PDO::PARAM_INT);
		$stmt->bindValue(':d1p2', $entry['d1_lastplayed'], PDO::PARAM_STR);
		$stmt->bindValue(':d2t2', $entry['d2_time'],       PDO::PARAM_INT);
		$stmt->bindValue(':d2p2', $entry['d2_lastplayed'], PDO::PARAM_STR);
		$stmt->bindValue(':d3t2', $entry['d3_time'],       PDO::PARAM_INT);
		$stmt->bindValue(':d3p2', $entry['d3_lastplayed'], PDO::PARAM_STR);
		executeOrFail($stmt);

		$stmt = $pdo->prepare('DELETE FROM userlevels_highscores WHERE userid=:uid AND levelid=:lid');
		$stmt->bindValue(':uid', $olduser->ID,      PDO::PARAM_INT);
		$stmt->bindValue(':lid', $entry['levelid'], PDO::PARAM_INT);
		executeOrFail($stmt);

		$stmt = $pdo->prepare('UPDATE userlevels SET d0_bestuserid=:uidnew WHERE d0_bestuserid=:uidold');
		$stmt->bindValue(':uidnew', $user->ID,    PDO::PARAM_INT);
		$stmt->bindValue(':uidold', $olduser->ID, PDO::PARAM_INT);
		executeOrFail($stmt);

		$stmt = $pdo->prepare('UPDATE userlevels SET d1_bestuserid=:uidnew WHERE d1_bestuserid=:uidold');
		$stmt->bindValue(':uidnew', $user->ID,    PDO::PARAM_INT);
		$stmt->bindValue(':uidold', $olduser->ID, PDO::PARAM_INT);
		executeOrFail($stmt);

		$stmt = $pdo->prepare('UPDATE userlevels SET d2_bestuserid=:uidnew WHERE d2_bestuserid=:uidold');
		$stmt->bindValue(':uidnew', $user->ID,    PDO::PARAM_INT);
		$stmt->bindValue(':uidold', $olduser->ID, PDO::PARAM_INT);
		executeOrFail($stmt);

		$stmt = $pdo->prepare('UPDATE userlevels SET d3_bestuserid=:uidnew WHERE d3_bestuserid=:uidold');
		$stmt->bindValue(':uidnew', $user->ID,    PDO::PARAM_INT);
		$stmt->bindValue(':uidold', $olduser->ID, PDO::PARAM_INT);
		executeOrFail($stmt);
	}

	//---------- step 5: [recalc sccm scores]

	$stmt = $pdo->prepare(loadSQL("recalc-sccm-score-for-user"));
	$stmt->bindValue(':uid', $user->ID,      PDO::PARAM_INT);
	executeOrFail($stmt);
	$scoresummary = $stmt->fetchAll(PDO::FETCH_ASSOC);

	$newscore = 0;
	foreach ($scoresummary as $ss) {
		for ($i=$ss['diff']; $i >=0; $i--) $newscore += $ss['levelcount'] * $config['diff_scores'][$i];
	}

	$user->SetSCCMScore($newscore);

	//---------- step 6: [download data]

	$finished_data = $user->GetAllLevelScoreEntries();

	//---------- step 7: [delete old user]

	$olduser->Delete();

	//---------- step 8: [return]

	logMessage("Account merge sucessful ($old_userid into $user->ID) with $changecount changes. Old account ($old_userid) purged.");
	outputResultSuccess(['user' => $user, 'updatecount' => $changecount, 'scores' => $finished_data]);
}



try {
	//set_time_limit(45);
	init("merge-login-2");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}