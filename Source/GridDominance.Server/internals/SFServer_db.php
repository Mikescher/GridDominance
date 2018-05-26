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
 * @param boolean $notransaction
 * @return PDO
 */
function connectOrFail($host, $dbname, $user, $password, $notransaction)
{
	for ($i = 6;; $i--)
	{
		try
		{
			$dsn = "mysql:host=$host;dbname=$dbname;charset=utf8";
			$opt =
			[
				PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
				PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
				PDO::ATTR_EMULATE_PREPARES   => false,
				//PDO::ATTR_PERSISTENT         => true,
			];
			if (!$notransaction) $opt[PDO::ATTR_TIMEOUT] = 10;

			return new PDO($dsn, $user, $password, $opt);
		}
		catch (Exception $e)
		{
			if ($i>0 && strpos($e->getMessage(), 'SQLSTATE[HY000] [2002] Connection refused') !== false) { usleep(100 * 1000); continue; } // wait 0.1s and retry

			outputErrorException(ERRORS::INTERNAL_EXCEPTION, "Can't connect to db", $e, LOGLEVEL::ERROR);
			return null;
		}
	}

}

function loadSQL($scriptname) {
	$sql = file_get_contents(__DIR__ . '/../data/' . $scriptname . '.sql'); //TODO evtl memcached ??

	$sql = str_replace("\r", "", $sql);
	$sql = str_replace("\n", " ", $sql);
	return $sql;
}

function loadReplSQL($scriptname, $replArray) {
	$sql = loadSQL($scriptname);
	foreach ($replArray as $rep) $sql = str_replace(':__'.$rep[0].'__', $rep[1], $sql);
	return $sql;
}