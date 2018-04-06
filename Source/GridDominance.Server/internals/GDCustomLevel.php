<?php

class GDCustomLevel
{
	public static function getByID($oid)
	{
		global $pdo;

		$stmt = $pdo->prepare(loadSQL('query-custom-level-by-id'));
		$stmt->bindValue(':oid', $oid, PDO::PARAM_INT);
		executeOrFail($stmt);

		$d = $stmt->fetch(PDO::FETCH_ASSOC);
		if ($d === FALSE) $d = null;
		return $d;
	}
}