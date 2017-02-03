<?php

function executeOrFail(PDOStatement $stmt) {
	try {
		$stmt->execute();
	} catch (PDOException $e) {
		logDebug("Failing SQL stmt: '$stmt->queryString'");
		outputErrorException(ERRORS::SQL_FAILED, "SQL Statement failed", $e, LOGLEVEL::ERROR);
	}
}

function connectOrFail(string $host, string $dbname, string $user, string $password) {
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
	}
}