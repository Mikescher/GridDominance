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

	public static function CreateFromSQL(array $row) : GDUser
	{
		$r = new GDUser();
		$r->ID            = $row['userid'];
		$r->Username      = $row['username'];
		$r->Score         = $row['score'];
		$r->AutoUser      = (bool)$row['is_auto_generated'];
		$r->PasswordHash  = $row['password_hash'];
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
	}
}