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
		/*
			SELECT
				ul.id, ul.name, ul.userid, ux.username,
				ul.upload_timestamp, ul.datahash, ul.upload_version,
				ul.grid_width, ul.grid_height, ul.stars,
				ul.d0_completed, ul.d0_played, ul.d0_bestuserid, u0.username AS d0_bestusername, ul.d0_besttime, ul.d0_besttimestamp,
				ul.d1_completed, ul.d1_played, ul.d1_bestuserid, u1.username AS d1_bestusername, ul.d1_besttime, ul.d1_besttimestamp,
				ul.d2_completed, ul.d2_played, ul.d2_bestuserid, u2.username AS d2_bestusername, ul.d2_besttime, ul.d2_besttimestamp,
				ul.d3_completed, ul.d3_played, ul.d3_bestuserid, u3.username AS d3_bestusername, ul.d3_besttime, ul.d3_besttimestamp

			FROM userlevels AS ul

			LEFT JOIN users AS ux ON ux.userid=ul.userid

			LEFT JOIN users AS u0 ON u0.userid=ul.d0_bestuserid
			LEFT JOIN users AS u1 ON u1.userid=ul.d1_bestuserid
			LEFT JOIN users AS u2 ON u2.userid=ul.d2_bestuserid
			LEFT JOIN users AS u3 ON u3.userid=ul.d3_bestuserid

			WHERE
				ul.upload_timestamp IS NOT NULL AND
				ul.userid=:uid AND
				ul.upload_decversion >= :dvs

			ORDER BY upload_timestamp DESC

			LIMIT  :lim
			OFFSET :off
		 */
		$stmt = $pdo->prepare('SELECT ul.id, ul.name, ul.userid, ux.username, ul.upload_timestamp, ul.datahash, ul.upload_version, ul.grid_width, ul.grid_height, ul.stars, ul.d0_completed, ul.d0_played, ul.d0_bestuserid, u0.username AS d0_bestusername, ul.d0_besttime, ul.d0_besttimestamp, ul.d1_completed, ul.d1_played, ul.d1_bestuserid, u1.username AS d1_bestusername, ul.d1_besttime, ul.d1_besttimestamp, ul.d2_completed, ul.d2_played, ul.d2_bestuserid, u2.username AS d2_bestusername, ul.d2_besttime, ul.d2_besttimestamp, ul.d3_completed, ul.d3_played, ul.d3_bestuserid, u3.username AS d3_bestusername, ul.d3_besttime, ul.d3_besttimestamp FROM userlevels AS ul LEFT JOIN users AS ux ON ux.userid=ul.userid LEFT JOIN users AS u0 ON u0.userid=ul.d0_bestuserid LEFT JOIN users AS u1 ON u1.userid=ul.d1_bestuserid LEFT JOIN users AS u2 ON u2.userid=ul.d2_bestuserid LEFT JOIN users AS u3 ON u3.userid=ul.d3_bestuserid WHERE ul.upload_timestamp IS NOT NULL AND ul.userid=:uid AND ul.upload_decversion >= :dvs ORDER BY upload_timestamp DESC LIMIT :lim OFFSET :off');
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
		/*
			SELECT
				ul.id, ul.name, ul.userid, ux.username,
				ul.upload_timestamp, ul.datahash, ul.upload_version,
				ul.grid_width, ul.grid_height, ul.stars,
				ul.d0_completed, ul.d0_played, ul.d0_bestuserid, u0.username AS d0_bestusername, ul.d0_besttime, ul.d0_besttimestamp,
				ul.d1_completed, ul.d1_played, ul.d1_bestuserid, u1.username AS d1_bestusername, ul.d1_besttime, ul.d1_besttimestamp,
				ul.d2_completed, ul.d2_played, ul.d2_bestuserid, u2.username AS d2_bestusername, ul.d2_besttime, ul.d2_besttimestamp,
				ul.d3_completed, ul.d3_played, ul.d3_bestuserid, u3.username AS d3_bestusername, ul.d3_besttime, ul.d3_besttimestamp

			FROM userlevels AS ul

			LEFT JOIN users AS ux ON ux.userid=ul.userid

			LEFT JOIN users AS u0 ON u0.userid=ul.d0_bestuserid
			LEFT JOIN users AS u1 ON u1.userid=ul.d1_bestuserid
			LEFT JOIN users AS u2 ON u2.userid=ul.d2_bestuserid
			LEFT JOIN users AS u3 ON u3.userid=ul.d3_bestuserid

			WHERE
				ul.upload_timestamp IS NOT NULL AND
				ul.upload_decversion >= :dvs

			ORDER BY
				stars DESC,
				upload_timestamp DESC

			LIMIT  :lim
			OFFSET :off
		 */
		$stmt = $pdo->prepare('SELECT ul.id, ul.name, ul.userid, ux.username, ul.upload_timestamp, ul.datahash, ul.upload_version, ul.grid_width, ul.grid_height, ul.stars, ul.d0_completed, ul.d0_played, ul.d0_bestuserid, u0.username AS d0_bestusername, ul.d0_besttime, ul.d0_besttimestamp, ul.d1_completed, ul.d1_played, ul.d1_bestuserid, u1.username AS d1_bestusername, ul.d1_besttime, ul.d1_besttimestamp, ul.d2_completed, ul.d2_played, ul.d2_bestuserid, u2.username AS d2_bestusername, ul.d2_besttime, ul.d2_besttimestamp, ul.d3_completed, ul.d3_played, ul.d3_bestuserid, u3.username AS d3_bestusername, ul.d3_besttime, ul.d3_besttimestamp FROM userlevels AS ul LEFT JOIN users AS ux ON ux.userid=ul.userid LEFT JOIN users AS u0 ON u0.userid=ul.d0_bestuserid LEFT JOIN users AS u1 ON u1.userid=ul.d1_bestuserid LEFT JOIN users AS u2 ON u2.userid=ul.d2_bestuserid LEFT JOIN users AS u3 ON u3.userid=ul.d3_bestuserid WHERE ul.upload_timestamp IS NOT NULL AND ul.upload_decversion >= :dvs ORDER BY stars DESC, upload_timestamp DESC LIMIT :lim OFFSET :off');
		$stmt->bindValue(':dvs', $decappversion,   PDO::PARAM_INT);
		$stmt->bindValue(':lim', 32,               PDO::PARAM_INT);
		$stmt->bindValue(':off', 32 * $pagination, PDO::PARAM_INT);
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		outputResultSuccess(['data' => $data]);
	}
	else if ($category === '@new')
	{
		/*
			SELECT
				ul.id, ul.name, ul.userid, ux.username,
				ul.upload_timestamp, ul.datahash, ul.upload_version,
				ul.grid_width, ul.grid_height, ul.stars,
				ul.d0_completed, ul.d0_played, ul.d0_bestuserid, u0.username AS d0_bestusername, ul.d0_besttime, ul.d0_besttimestamp,
				ul.d1_completed, ul.d1_played, ul.d1_bestuserid, u1.username AS d1_bestusername, ul.d1_besttime, ul.d1_besttimestamp,
				ul.d2_completed, ul.d2_played, ul.d2_bestuserid, u2.username AS d2_bestusername, ul.d2_besttime, ul.d2_besttimestamp,
				ul.d3_completed, ul.d3_played, ul.d3_bestuserid, u3.username AS d3_bestusername, ul.d3_besttime, ul.d3_besttimestamp

			FROM userlevels AS ul

			LEFT JOIN users AS ux ON ux.userid=ul.userid

			LEFT JOIN users AS u0 ON u0.userid=ul.d0_bestuserid
			LEFT JOIN users AS u1 ON u1.userid=ul.d1_bestuserid
			LEFT JOIN users AS u2 ON u2.userid=ul.d2_bestuserid
			LEFT JOIN users AS u3 ON u3.userid=ul.d3_bestuserid

			WHERE
				ul.upload_timestamp IS NOT NULL AND
				ul.upload_decversion >= :dvs

			ORDER BY
				ul.upload_timestamp DESC

			LIMIT  :lim
			OFFSET :off
		 */
		$stmt = $pdo->prepare('SELECT ul.id, ul.name, ul.userid, ux.username, ul.upload_timestamp, ul.datahash, ul.upload_version, ul.grid_width, ul.grid_height, ul.stars, ul.d0_completed, ul.d0_played, ul.d0_bestuserid, u0.username AS d0_bestusername, ul.d0_besttime, ul.d0_besttimestamp, ul.d1_completed, ul.d1_played, ul.d1_bestuserid, u1.username AS d1_bestusername, ul.d1_besttime, ul.d1_besttimestamp, ul.d2_completed, ul.d2_played, ul.d2_bestuserid, u2.username AS d2_bestusername, ul.d2_besttime, ul.d2_besttimestamp, ul.d3_completed, ul.d3_played, ul.d3_bestuserid, u3.username AS d3_bestusername, ul.d3_besttime, ul.d3_besttimestamp FROM userlevels AS ul LEFT JOIN users AS ux ON ux.userid=ul.userid LEFT JOIN users AS u0 ON u0.userid=ul.d0_bestuserid LEFT JOIN users AS u1 ON u1.userid=ul.d1_bestuserid LEFT JOIN users AS u2 ON u2.userid=ul.d2_bestuserid LEFT JOIN users AS u3 ON u3.userid=ul.d3_bestuserid WHERE ul.upload_timestamp IS NOT NULL AND ul.upload_decversion >= :dvs ORDER BY ul.upload_timestamp DESC LIMIT :lim OFFSET :off');
		$stmt->bindValue(':dvs', $decappversion,   PDO::PARAM_INT);
		$stmt->bindValue(':lim', 32,               PDO::PARAM_INT);
		$stmt->bindValue(':off', 32 * $pagination, PDO::PARAM_INT);
		executeOrFail($stmt);
		$data = $stmt->fetchAll(PDO::FETCH_ASSOC);

		outputResultSuccess(['data' => $data]);
	}
	else if ($category === '@hot') // https://news.ycombinator.com/item?id=1781013
	{
		/*
			SELECT
				ul.id, ul.name, ul.userid, ux.username,
				ul.upload_timestamp, ul.datahash, ul.upload_version,
				ul.grid_width, ul.grid_height, ul.stars,
				ul.d0_completed, ul.d0_played, ul.d0_bestuserid, u0.username AS d0_bestusername, ul.d0_besttime, ul.d0_besttimestamp,
				ul.d1_completed, ul.d1_played, ul.d1_bestuserid, u1.username AS d1_bestusername, ul.d1_besttime, ul.d1_besttimestamp,
				ul.d2_completed, ul.d2_played, ul.d2_bestuserid, u2.username AS d2_bestusername, ul.d2_besttime, ul.d2_besttimestamp,
				ul.d3_completed, ul.d3_played, ul.d3_bestuserid, u3.username AS d3_bestusername, ul.d3_besttime, ul.d3_besttimestamp,

				(ul.stars / POW(TIMESTAMPDIFF(HOUR, ul.upload_timestamp, NOW()),1.8)) AS hot_ranking

			FROM userlevels AS ul

			LEFT JOIN users AS ux ON ux.userid=ul.userid

			LEFT JOIN users AS u0 ON u0.userid=ul.d0_bestuserid
			LEFT JOIN users AS u1 ON u1.userid=ul.d1_bestuserid
			LEFT JOIN users AS u2 ON u2.userid=ul.d2_bestuserid
			LEFT JOIN users AS u3 ON u3.userid=ul.d3_bestuserid

			WHERE
				ul.upload_timestamp IS NOT NULL AND
				ul.upload_decversion >= :dvs

			ORDER BY
				hot_ranking DESC,


			LIMIT  :lim
			OFFSET :off
		 */
		$stmt = $pdo->prepare('SELECT ul.id, ul.name, ul.userid, ux.username, ul.upload_timestamp, ul.datahash, ul.upload_version, ul.grid_width, ul.grid_height, ul.stars, ul.d0_completed, ul.d0_played, ul.d0_bestuserid, u0.username AS d0_bestusername, ul.d0_besttime, ul.d0_besttimestamp, ul.d1_completed, ul.d1_played, ul.d1_bestuserid, u1.username AS d1_bestusername, ul.d1_besttime, ul.d1_besttimestamp, ul.d2_completed, ul.d2_played, ul.d2_bestuserid, u2.username AS d2_bestusername, ul.d2_besttime, ul.d2_besttimestamp, ul.d3_completed, ul.d3_played, ul.d3_bestuserid, u3.username AS d3_bestusername, ul.d3_besttime, ul.d3_besttimestamp, (ul.stars/POW(TIMESTAMPDIFF(HOUR, ul.upload_timestamp, NOW()),1.8)) AS hot_ranking FROM userlevels AS ul LEFT JOIN users AS ux ON ux.userid=ul.userid LEFT JOIN users AS u0 ON u0.userid=ul.d0_bestuserid LEFT JOIN users AS u1 ON u1.userid=ul.d1_bestuserid LEFT JOIN users AS u2 ON u2.userid=ul.d2_bestuserid LEFT JOIN users AS u3 ON u3.userid=ul.d3_bestuserid WHERE ul.upload_timestamp IS NOT NULL AND ul.upload_decversion >= :dvs ORDER BY hot_ranking DESC LIMIT :lim OFFSET :off');
		$stmt->bindValue(':dvs', $decappversion,   PDO::PARAM_INT);
		$stmt->bindValue(':lim', 32,               PDO::PARAM_INT);
		$stmt->bindValue(':off', 32 * $pagination, PDO::PARAM_INT);
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