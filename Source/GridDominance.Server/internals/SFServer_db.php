<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

/**
 * @param PDOStatement $stmt
 */
function executeOrFail($stmt) {
	try {
		$stmt->execute();
	} catch (PDOException $e) {
		logDebug("Failing SQL stmt: '$stmt->queryString'");
		outputErrorException(ERRORS::INTERNAL_EXCEPTION, "SQL Statement failed", $e, LOGLEVEL::ERROR);
	}
}

/**
 * @param string $host
 * @param string $dbname
 * @param string $user
 * @param string $password
 * @return PDO
 */
function connectOrFail($host, $dbname, $user, $password) {
	try {
		$dsn = "mysql:host=$host;dbname=$dbname;charset=utf8";
		$opt = [
			PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
			PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
			PDO::ATTR_EMULATE_PREPARES   => false,
		];

		return new PDO($dsn, $user, $password, $opt);
	} catch (Exception $e) {
		outputErrorException(ERRORS::INTERNAL_EXCEPTION, "Can't connect to db", $e, LOGLEVEL::ERROR);
		return null;
	}
}

function loadSQL($scriptname) {
	if (file_exists('data/' . $scriptname . '.sql'))
		$sql = file_get_contents('data/' . $scriptname . '.sql');
	else if (file_exists('../data/' . $scriptname . '.sql'))
		$sql = file_get_contents('../data/' . $scriptname . '.sql');
	else
		throw new Exception("FILE NOT FOUND: $scriptname");

	$sql = str_replace("\r", "", $sql);
	$sql = str_replace("\n", " ", $sql);
	return $sql;
}

function loadReplSQL($scriptname, $placeholder, $replace) {
	$sql = loadSQL($scriptname);
	$sql = str_replace($placeholder, $replace, $sql);
	return $sql;
}