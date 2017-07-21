<?php

require 'internals/backend.php';

function run() {
	global $pdo;
	global $config;

	$userid        = getParamIntOrError('userid');
	$worldid       = getParamStrOrError('world_id');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $worldid]);

	//----------

	if ($worldid == '*') {
		$stmt = $pdo->prepare(loadSQL("get-ranking_global_top100"));
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		$stmt = $pdo->prepare(loadSQL("get-ranking_global_playerrank"));
		$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
		executeOrFail($stmt);
		$rank = $stmt->fetchAll(PDO::FETCH_ASSOC);
	} else if ($worldid == '@') {
		$stmt = $pdo->prepare(loadSQL("get-ranking_multiplayer_top100"));
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		$stmt = $pdo->prepare(loadSQL("get-ranking_multiplayer_playerrank"));
		$stmt->bindValue(':uid', $userid, PDO::PARAM_INT);
		executeOrFail($stmt);
		$rank = $stmt->fetchAll(PDO::FETCH_ASSOC);
	} else {
		$condition = ' WHERE (';
		$ccfirst = true;
		foreach ($config['levelmapping'] as $mapping) {
			if ($mapping[0] == $worldid) {
				if (!$ccfirst) $condition .= ' OR ';
				$ccfirst = false;
				$condition .= 'level_highscores.levelid LIKE \'' . $mapping[1] . '\'';
			}
		}
		if ($ccfirst) $condition .= '0=1';
		$condition .= ') ';

		$stmt = $pdo->prepare(loadReplSQL('get-ranking_local_top100', '#$$CONDITION$$', $condition));
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		$stmt = $pdo->prepare(loadReplSQL('get-ranking_local_playerrank', '#$$CONDITION$$', $condition));
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