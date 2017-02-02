<?php

/**
 * Created by PhpStorm.
 * User: Mike
 * Date: 2017-01-28
 * Time: 23:27
 */
class GDUser
{
	const DEFAULT_USERNAME = 'anonymous';

	/** @var $ID int */
	public $ID = 0;

	/** @var $Username string */
	public $Username = self::DEFAULT_USERNAME;

	/** @var $AutoUser bool */
	public $AutoUser = true;

	/** @var $Score int */
	public $Score = 0;

	/** @var $RevID int */
	public $RevID = 0;

	/** @var $PasswordHash string */
	private $PasswordHash = "";

	public static function CreateNew(int $_id, string $_username, int $_score) : GDUser
	{
		$r = new GDUser();
		$r->ID = $_id;
		$r->Username = $_username;
		$r->Score = $_score;
		return $r;
	}

	public static function QueryOrFail(PDO $pdo, string $pw, int $userid) : GDUser
	{
		$stmt = $pdo->prepare("SELECT userid, username, password_hash, is_auto_generated, score, revision_id FROM users WHERE userid=:id");
		$stmt->bindValue(':id', $userid, PDO::PARAM_INT);
		$stmt->execute();
		$row = $stmt->fetch(PDO::FETCH_ASSOC);

		if ($row === FALSE) outputError(ERRORS::USER_BY_ID_NOT_FOUND, "No user with id $userid found", LOGLEVEL::DEBUG);

		$user = self::CreateFromSQL($row);

		if (! $user->verify_password($pw)) outputError(ERRORS::WRONG_PASSWORD, "Wrong password supplied", LOGLEVEL::DEBUG);

		return $user;
	}

	public static function CreateFromSQL(array $row) : GDUser
	{
		$r = new GDUser();
		$r->ID            = $row['userid'];
		$r->Username      = $row['username'];
		$r->Score         = $row['score'];
		$r->AutoUser      = (bool)$row['is_auto_generated'];
		$r->PasswordHash  = $row['password_hash'];
		$r->RevID         = $row['reviosion_id'];
		return $r;
	}

	public function verify_password(string $pw) : bool
	{
		return password_verify($pw, $this->PasswordHash);
	}

	public function Upgrade(string $username, string $hash)
	{
		$this->AutoUser = false;
		$this->PasswordHash = $hash;
		$this->Username = $username;
		$this->RevID++;
	}
}