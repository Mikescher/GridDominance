<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

	$userid         = getParamUIntOrError('userid');
	$category       = getParamStrOrError('category');
	$queryparameter = getParamStrOrError('param', true);
	$pagination     = getParamIntOrError('pagination');
	$appversion     = getParamStrOrError('app_version');
	$decappversion  = getParamIntOrError('app_version_dec');

	$signature     = getParamStrOrError('msgk');

	check_commit_signature($signature, [$userid, $category, $queryparameter, $pagination, $appversion, $decappversion]);

	//----------

	if ($category === '@user')
	{
		$stmt = $pdo->prepare(loadSQL("query-userlevel-user"));
		$stmt->bindValue(':uid', $queryparameter,   PDO::PARAM_INT);
		$stmt->bindValue(':dvs', $decappversion,    PDO::PARAM_INT);
		$stmt->bindValue(':lim', 128,               PDO::PARAM_INT);
		$stmt->bindValue(':off', 128 * $pagination, PDO::PARAM_INT);
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		outputResultSuccess(['data' => $data]);
	}
	else if ($category === '@top')
	{

		$stmt = $pdo->prepare(loadSQL("query-userlevel-top"));
		$stmt->bindValue(':dvs', $decappversion,   PDO::PARAM_INT);
		$stmt->bindValue(':lim', 32,               PDO::PARAM_INT);
		$stmt->bindValue(':off', 32 * $pagination, PDO::PARAM_INT);
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		outputResultSuccess(['data' => $data]);
	}
	else if ($category === '@new')
	{
		$stmt = $pdo->prepare(loadSQL("query-userlevel-new"));
		$stmt->bindValue(':dvs', $decappversion,   PDO::PARAM_INT);
		$stmt->bindValue(':lim', 32,               PDO::PARAM_INT);
		$stmt->bindValue(':off', 32 * $pagination, PDO::PARAM_INT);
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		outputResultSuccess(['data' => $data]);
	}
	else if ($category === '@hot') // https://news.ycombinator.com/item?id=1781013
	{
		$stmt = $pdo->prepare(loadReplSQL("query-userlevel-hot",
			[
				['HOT_FACTOR', $config['hot_factor']]
			]));
		$stmt->bindValue(':dvs', $decappversion,   PDO::PARAM_INT);
		$stmt->bindValue(':lim', 32,               PDO::PARAM_INT);
		$stmt->bindValue(':off', 32 * $pagination, PDO::PARAM_INT);
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		outputResultSuccess(['data' => $data]);
	}
	else if ($category === '@search')
	{
		$stmt = $pdo->prepare(loadSQL("query-userlevel-search"));
		$stmt->bindValue(':dvs',        $decappversion,                    PDO::PARAM_INT);
		$stmt->bindValue(':lim',        32,                                PDO::PARAM_INT);
		$stmt->bindValue(':off',        32 * $pagination,                  PDO::PARAM_INT);
		$stmt->bindValue(':search1',     $queryparameter,                   PDO::PARAM_STR);
		$stmt->bindValue(':search2',     $queryparameter,                   PDO::PARAM_STR);
		$stmt->bindValue(':search3', formatForSqlLike($queryparameter), PDO::PARAM_STR);
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		outputResultSuccess(['data' => $data]);
	}
	else
	{
		outputError(ERRORS::LEVELQUERY_UNNKOWN_CAT, "Unknown category $category", LOGLEVEL::ERROR);
	}

	//----------

	outputResultSuccess([]);
}



try {
	init("query-userlevels");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}