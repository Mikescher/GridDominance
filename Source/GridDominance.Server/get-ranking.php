<?php

require 'internals/backend.php';

function run() {
	global $pdo;

	$userid        = getParamIntOrError('userid');
	$worldid       = getParamStrOrError('world_id');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $worldid]);

	//----------

	if ($worldid == '*') {
		$stmt = $pdo->prepare(loadSQL("get-ranking_global_top"));
		$stmt->bindValue(':qlimit', 100);
		$stmt->bindValue(':qpage', 0);
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		$stmt = $pdo->prepare(loadSQL("get-ranking_global_playerrank"));
		$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
		executeOrFail($stmt);
		$rank = $stmt->fetchAll(PDO::FETCH_ASSOC);
	} else if ($worldid == '@') {
		$stmt = $pdo->prepare(loadSQL("get-ranking_multiplayer_top"));
		$stmt->bindValue(':qlimit', 100);
		$stmt->bindValue(':qpage', 0);
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		$stmt = $pdo->prepare(loadSQL("get-ranking_multiplayer_playerrank"));
		$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
		executeOrFail($stmt);
		$rank = $stmt->fetchAll(PDO::FETCH_ASSOC);
	} else {

		$stmt = $pdo->prepare(loadReplSQL('get-ranking_local_top', '#$$FIELD$$', worldGuidToSQLField($worldid)));
		$stmt->bindValue(':qlimit', 100);
		$stmt->bindValue(':qpage', 0);
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		$stmt = $pdo->prepare(loadReplSQL('get-ranking_local_playerrank', '#$$FIELD$$', worldGuidToSQLField($worldid)));
		$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
		executeOrFail($stmt);
		$rank = $stmt->fetchAll(PDO::FETCH_ASSOC);
	}

	//----------

	logDebug("get-ranking request from " . ParamServerOrUndef('REMOTE_ADDR'));
	outputResultSuccess(["ranking" => $data, "personal" => $rank]);
}

try {
	init("get-ranking");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}