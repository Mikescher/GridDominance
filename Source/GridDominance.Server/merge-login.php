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
	$mergedata     = getParamStrOrError('merge_data');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$old_userid, $old_password, $appversion, $username, $password, $mergedata]);

	//--------- step 1: [verify]

	GDUser::QueryOrFail($pdo, $old_password, $old_userid); // old (anon) user

	$user = GDUser::QueryOrFailByName($pdo, $password, $username); // new user
	$userid = $user->ID;
	$jdata = json_decode($mergedata);

	//--------- step 2: [get data of new_user]

	$stmt = $pdo->prepare("SELECT levelid, difficulty, best_time FROM level_highscores WHERE userid=:uid");
	$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
	executeOrFail($stmt);

	$old_data = $stmt->fetchAll(PDO::FETCH_ASSOC);

	//---------- step 3: [merge data into new_user]

	$changecount = 0;
	foreach($jdata as $new_data_row) {
		$levelid       = $new_data_row->levelid;
		$difficulty    = $new_data_row->difficulty;
		$leveltime     = $new_data_row->leveltime;

		if ($leveltime <= 0) { logMessage("The time $leveltime is not possible", "WARN"); continue; }
		if (!in_array($difficulty, $config['difficulties'], TRUE)) { logMessage("The difficulty $difficulty is not possible", "WARN"); continue; }
		if (!in_array($levelid, $config['levelids'], TRUE)) { logMessage("The levelID $levelid is not possible", "WARN"); continue; }

		$old_data_row = FALSE;
		foreach ($old_data as $d) { if ($d['levelid'] == $levelid && $d['difficulty'] == $difficulty) $old_data_row = $d; }
		if ($old_data_row !== FALSE) {

			if ($old_data_row['best_time'] > $leveltime) {
				// existing row in db
				$stmt = $pdo->prepare("UPDATE level_highscores SET best_time=:time, last_changed=CURRENT_TIMESTAMP() WHERE userid=:uid AND levelid=:lid AND difficulty=:diff");
				$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
				$stmt->bindValue(':lid', $levelid, PDO::PARAM_STR);
				$stmt->bindValue(':diff', $difficulty, PDO::PARAM_INT);
				$stmt->bindValue(':time', $leveltime, PDO::PARAM_INT);
				executeOrFail($stmt);
				logDebug("levelscore changed (part of batch) for user:$userid Level:$levelid::$difficulty to $leveltime ms");
				$changecount++;
			}

		} else {

			// no row in db
			$stmt = $pdo->prepare("INSERT INTO level_highscores (userid, levelid, difficulty, best_time, last_changed) VALUES (:uid, :lid, :diff, :time, CURRENT_TIMESTAMP())");
			$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
			$stmt->bindValue(':lid', $levelid, PDO::PARAM_STR);
			$stmt->bindValue(':diff', $difficulty, PDO::PARAM_INT);
			$stmt->bindValue(':time', $leveltime, PDO::PARAM_INT);
			executeOrFail($stmt);
			$user->Score += $config['diff_scores'][$difficulty];
			logDebug("levelscore added (part of batch) for user:$userid Level:$levelid::$difficulty to $leveltime ms ($user->Score)");
			$changecount++;
		}
	}

	//---------- step 4: [download data]

	$stmt = $pdo->prepare("SELECT levelid, difficulty, best_time FROM level_highscores WHERE userid=:uid");
	$stmt->bindValue(':uid', $user->ID, PDO::PARAM_INT);
	executeOrFail($stmt);

	$finished_data = $stmt->fetchAll(PDO::FETCH_ASSOC);

	//---------- step 5: [update newuser]

	$stmt = $pdo->prepare("UPDATE users SET score=:scr, last_online=CURRENT_TIMESTAMP(), last_online_app_version=:av, revision_id=(revision_id+1) WHERE userid=:id");
	$stmt->bindValue(':id', $user->ID, PDO::PARAM_INT);
	$stmt->bindValue(':av', $appversion, PDO::PARAM_INT);
	$stmt->bindValue(':scr', $user->Score, PDO::PARAM_INT);
	executeOrFail($stmt);

	$user->RevID++;

	//---------- step 6: [delete old user]

	$stmt = $pdo->prepare("DELETE FROM users WHERE userid=:uid");
	$stmt->bindValue(':uid', $old_userid, PDO::PARAM_INT);
	executeOrFail($stmt);

	$stmt = $pdo->prepare("DELETE FROM level_highscores WHERE userid=:uid");
	$stmt->bindValue(':uid', $old_userid, PDO::PARAM_INT);
	executeOrFail($stmt);

	//---------- step 7: [return]

	logMessage("Account merge sucessful ($old_userid into $userid) with $changecount changes. Old account purged.");
	outputResultSuccess(['user' => $user, 'updatecount' => $changecount, 'scores' => $finished_data]);
}



try {
	init("merge-login");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}