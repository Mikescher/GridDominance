<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

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

	/**
	 * @param int $_id
	 * @param string $_username
	 * @param int $_score
	 * @return GDUser
	 */
	public static function CreateNew($_id, $_username, $_score)
	{
		$r = new GDUser();
		$r->ID = $_id;
		$r->Username = $_username;
		$r->Score = $_score;
		return $r;
	}

	/**
	 * @param PDO $pdo
	 * @param string $pw
	 * @param string $username
	 * @return GDUser
	 */
	public static function QueryOrFailByName($pdo, $pw, $username)
	{
		$stmt = $pdo->prepare("SELECT userid, username, password_hash, is_auto_generated, score, revision_id FROM users WHERE username=:name");
		$stmt->bindValue(':name', $username, PDO::PARAM_STR);
		$stmt->execute();
		$row = $stmt->fetch(PDO::FETCH_ASSOC);

		if ($row === FALSE) outputError(ERRORS::USER_BY_NAME_NOT_FOUND, "No user with name $username found", LOGLEVEL::DEBUG);

		$user = self::CreateFromSQL($row);

		if (! $user->verify_password($pw)) outputError(ERRORS::WRONG_PASSWORD, "Wrong password supplied", LOGLEVEL::DEBUG);

		return $user;
	}

	/**
	 * @param PDO $pdo
	 * @param string $pw
	 * @param int $userid
	 * @return GDUser
	 */
	public static function QueryOrFail($pdo, $pw, $userid)
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


	/**
	 * @param PDO $pdo
	 * @param int $userid
	 * @return GDUser
	 */
	public static function QueryByIDOrNull($pdo, $userid)
	{
		$stmt = $pdo->prepare("SELECT userid, username, password_hash, is_auto_generated, score, revision_id FROM users WHERE userid=:id");
		$stmt->bindValue(':id', $userid, PDO::PARAM_INT);
		$stmt->execute();
		$row = $stmt->fetch(PDO::FETCH_ASSOC);

		if ($row === FALSE) return NULL;

		$user = self::CreateFromSQL($row);

		return $user;
	}

	/**
	 * @param PDO $pdo
	 * @param string $pw
	 * @param int $userid
	 * @return GDUser|null
	 */
	public static function QueryOrNull($pdo, $pw, $userid)
	{
		$stmt = $pdo->prepare("SELECT userid, username, password_hash, is_auto_generated, score, revision_id FROM users WHERE userid=:id");
		$stmt->bindValue(':id', $userid, PDO::PARAM_INT);
		$stmt->execute();
		$row = $stmt->fetch(PDO::FETCH_ASSOC);

		if ($row === FALSE) return NULL;

		$user = self::CreateFromSQL($row);

		if (! $user->verify_password($pw)) return NULL;

		return $user;
	}

	/**
	 * @param array $row
	 * @return GDUser
	 */
	public static function CreateFromSQL($row)
	{
		$r = new GDUser();
		$r->ID            = $row['userid'];
		$r->Username      = $row['username'];
		$r->Score         = $row['score'];
		$r->AutoUser      = (bool)$row['is_auto_generated'];
		$r->PasswordHash  = $row['password_hash'];
		$r->RevID         = $row['revision_id'];
		return $r;
	}

	/**
	 * @param string $pw
	 * @return bool
	 */
	public function verify_password($pw)
	{
		return password_verify($pw, $this->PasswordHash);
	}

	/**
	 * @param $username
	 * @param $hash
	 */
	public function Upgrade(string $username, string $hash)
	{
		$this->AutoUser = false;
		$this->PasswordHash = $hash;
		$this->Username = $username;
		$this->RevID++;
	}
}