<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

class SQLStatistics
{
	public static $ExecutionTimes = [];
	
	public static function Add($name, $sql, $time)
	{
		if (array_key_exists($name, self::$ExecutionTimes)) {
			$v = self::$ExecutionTimes[$name];

			$v[0] += 1;
			$v[1] += $time;
			array_push($v[2], $sql);
			$v[2] = array_unique($v[2]);

			self::$ExecutionTimes[$name] = $v;
		} else {
			$v = [ 1, $time, [$sql] ];
			self::$ExecutionTimes[$name] = $v;
		}
	}
}

function sql_query_num($name, $query, $default = NULL)
{
	global $pdo;

	$start = microtime(true);

	if ($default !== NULL)
	{
		$rall = $pdo->query($query)->fetch(PDO::FETCH_NUM);
		$r = ($rall === FALSE) ? 0 : $rall[0];
	}
	else
	{
		$r = $pdo->query($query)->fetch(PDO::FETCH_NUM)[0];
	}

	SQLStatistics::Add($name, $query, microtime(true) - $start);
	
	return $r;
}

function sql_query_num_prep($name, $query, $params, $default = NULL)
{
	global $pdo;

	$start = microtime(true);

	$stmt = $pdo->prepare($query);
	
	foreach ($params as $p) 
	{
		if (strpos($query, $p[0]) !== FALSE) $stmt->bindValue($p[0], $p[1], $p[2]);
	}

	if ($default !== NULL)
	{
		$stmt->execute();
		$rall = $stmt->fetch(PDO::FETCH_NUM);
		$r = ($rall === FALSE) ? 0 : $rall[0];
	}
	else
	{
		$stmt->execute();
		$r = $stmt->fetch(PDO::FETCH_NUM)[0];
	}

	SQLStatistics::Add($name, $query, microtime(true) - $start);
	
	return $r;
}

function sql_query_assoc($name, $query)
{
	global $pdo;

	$start = microtime(true);

	$r = $pdo->query($query)->fetchAll(PDO::FETCH_ASSOC);

	SQLStatistics::Add($name, $query, microtime(true) - $start);
	
	return $r;
}

function sql_query_assoc_prep($name, $query, $params)
{
	global $pdo;

	$start = microtime(true);

	$stmt = $pdo->prepare($query);
	
	foreach ($params as $p) 
	{
		if (strpos($query, $p[0]) !== FALSE) $stmt->bindValue($p[0], $p[1], $p[2]);
	}

	$stmt->execute();
	$r = $stmt->fetchAll(PDO::FETCH_ASSOC);

	SQLStatistics::Add($name, $query, microtime(true) - $start);
	
	return $r;
}

function formatForSqlLike($value)
{
	$value = str_replace('%', '\\%', $value);
	$value = str_replace('_', '\\_', $value);
	$value = str_replace('\\', '\\\\', $value);
	return '%' . $value . '%';
}