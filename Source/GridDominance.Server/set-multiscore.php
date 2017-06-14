<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamSHAOrError('password');
	$appversion    = getParamStrOrError('app_version');
	$data          = getParamStrOrError('data');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $password, $appversion, $data]);

	//---------- step 1: [verify]

	$user = GDUser::QueryOrFail($pdo, $password, $userid);
	$jdata = json_decode($data);

	//---------- step 2: [get online data]

	$stmt = $pdo->prepare("SELECT levelid, difficulty, best_time FROM level_highscores WHERE userid=:uid");
	$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
	executeOrFail($stmt);

	$old_data = $stmt->fetchAll(PDO::FETCH_ASSOC);

	//---------- step 3: [insert new data]

	$changecount = 0;
	foreach($jdata as $new_data_row) {
		$levelid       = $new_data_row->levelid;
		$difficulty    = $new_data_row->difficulty;
		$leveltime     = $new_data_row->leveltime;

		if ($leveltime <= 0) { logMessage("The time $leveltime is not possible", "WARN"); continue; }
		if (!in_array($difficulty, $config['difficulties'], TRUE)) { logMessage("The difficulty $difficulty is not possible", "WARN"); continue; }
		if (!in_array($levelid, $config['levelids'], TRUE)) { logMessage("The levelID $levelid is not possible", "WARN"); continue; }

		$old_data_row = FALSE;
		foreach($old_data as $d) { if ($d['levelid'] == $levelid && $d['difficulty'] == $difficulty) $old_data_row = $d; }
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

	//---------- step 4: [return]

	if ($changecount > 0) {
		$stmt = $pdo->prepare("UPDATE users SET score=:scr, last_online=CURRENT_TIMESTAMP(), last_online_app_version=:av, revision_id=(revision_id+1) WHERE userid=:id");
		$stmt->bindValue(':id', $userid, PDO::PARAM_INT);
		$stmt->bindValue(':av', $appversion, PDO::PARAM_INT);
		$stmt->bindValue(':scr', $user->Score, PDO::PARAM_INT);
		executeOrFail($stmt);
		$user->RevID++;

		logDebug("levelscores batch-changed for user:$userid ($changecount changes) ($user->Score)");
		outputResultSuccess(['update' => true, 'updatecount' => $changecount, 'user' => $user]);
	} else {
		$stmt = $pdo->prepare("UPDATE users SET last_online=CURRENT_TIMESTAMP(), last_online_app_version=:av WHERE userid=:id");
		$stmt->bindValue(':id', $userid, PDO::PARAM_INT);
		$stmt->bindValue(':av', $appversion, PDO::PARAM_INT);
		executeOrFail($stmt);

		logDebug("levelscores batch-changed for user:$userid ($changecount changes)");
		outputResultSuccess(['update' => false, 'updatecount' => $changecount, 'user' => $user]);
	}
}



try {
	init("set-multiscore");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}