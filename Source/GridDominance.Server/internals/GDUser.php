<?php

/**
 * Created by PhpStorm.
 * User: Mike
 * Date: 2017-01-28
 * Time: 23:27
 */
class GDUser
{
	public $ID;
	public $Username;

	public static function Create(int $_id, string $_username) : GDUser {
		$r = new GDUser();
		$r->ID = $_id;
		$r->Username = $_username;
	}
}