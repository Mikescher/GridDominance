<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

/**
 * Created by PhpStorm.
 * User: Mike
 * Date: 2017-01-28
 * Time: 23:27
 */
class GDUser implements JsonSerializable
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

	/** @var $ScoreW1 int */
	public $ScoreW1 = 0;

	/** @var $ScoreW2 int */
	public $ScoreW2 = 0;

	/** @var $ScoreW3 int */
	public $ScoreW3 = 0;

	/** @var $ScoreW4 int */
	public $ScoreW4 = 0;

	/** @var $TotalTime int */
	public $TotalTime = 0;

	/** @var $TimeW1 int */
	public $TimeW1 = 0;

	/** @var $TimeW2 int */
	public $TimeW2 = 0;

	/** @var $TimeW3 int */
	public $TimeW3 = 0;

	/** @var $TimeW4 int */
	public $TimeW4 = 0;

	/** @var $MultiplayerScore int */
	public $MultiplayerScore = 0;

	/** @var $RevID int */
	public $RevID = 0;

	/** @var $PasswordHash string */
	private $PasswordHash = "";

	/**
	 * @param int $_id
	 * @param string $_username
	 * @return GDUser
	 */
	public static function CreateNew($_id, $_username)
	{
		$r = new GDUser();
		$r->ID = $_id;
		$r->Username = $_username;
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
		$stmt = $pdo->prepare("SELECT userid, username, password_hash, is_auto_generated, score, mpscore, revision_id, score_w1, score_w2, score_w3, score_w4, time_total, time_w1, time_w2, time_w3, time_w4 FROM users WHERE username=:name");
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
		$stmt = $pdo->prepare("SELECT userid, username, password_hash, is_auto_generated, score, mpscore, revision_id, score_w1, score_w2, score_w3, score_w4, time_total, time_w1, time_w2, time_w3, time_w4 FROM users WHERE userid=:id");
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
		$stmt = $pdo->prepare("SELECT userid, username, password_hash, is_auto_generated, score, mpscore, revision_id, score_w1, score_w2, score_w3, score_w4, time_total, time_w1, time_w2, time_w3, time_w4 FROM users WHERE userid=:id");
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
		$stmt = $pdo->prepare("SELECT userid, username, password_hash, is_auto_generated, score, mpscore, revision_id, score_w1, score_w2, score_w3, score_w4, time_total, time_w1, time_w2, time_w3, time_w4 FROM users WHERE userid=:id");
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
		$r->ID               = $row['userid'];
		$r->Username         = $row['username'];
		$r->Score            = $row['score'];
		$r->MultiplayerScore = $row['mpscore'];
		$r->AutoUser         = (bool)$row['is_auto_generated'];
		$r->PasswordHash     = $row['password_hash'];
		$r->RevID            = $row['revision_id'];
		$r->ScoreW1          = $row['score_w1'];
		$r->ScoreW2          = $row['score_w2'];
		$r->ScoreW3          = $row['score_w3'];
		$r->ScoreW4          = $row['score_w4'];
		$r->TotalTime        = $row['time_total'];
		$r->TimeW1           = $row['time_w1'];
		$r->TimeW2           = $row['time_w2'];
		$r->TimeW3           = $row['time_w3'];
		$r->TimeW4           = $row['time_w4'];
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
	 * @param string $username
	 * @param string $hash
	 * @param string $app_version
	 */
	public function Upgrade($username, $hash, $app_version)
	{
		global $pdo;

		$stmt = $pdo->prepare("UPDATE users SET username=:usr, password_hash=:pw, is_auto_generated=0, last_online=CURRENT_TIMESTAMP(), app_version=:av, revision_id=(revision_id+1) WHERE userid=:id");
		$stmt->bindValue(':usr', $username, PDO::PARAM_STR);
		$stmt->bindValue(':pw', $hash, PDO::PARAM_STR);
		$stmt->bindValue(':id', $this->ID, PDO::PARAM_INT);
		$stmt->bindValue(':av', $app_version, PDO::PARAM_STR);
		executeOrFail($stmt);

		$this->AutoUser = false;
		$this->PasswordHash = $hash;
		$this->Username = $username;
		$this->RevID++;
	}

	/**
	 * @param string $app_version
	 * @param string $device_name
	 * @param string $device_version
	 * @param string $unlocked_worlds
	 * @param string $device_resolution
	 */
	public function UpdateMeta($app_version, $device_name, $device_version, $unlocked_worlds, $device_resolution) {
		global $pdo;
		$stmt = $pdo->prepare("UPDATE users SET last_online=CURRENT_TIMESTAMP(), app_version=:av, device_name=:dn, device_version=:dv, unlocked_worlds=:uw, device_resolution=:dr, ping_counter=ping_counter+1 WHERE userid=:uid");
		$stmt->bindValue(':uid', $this->ID,         PDO::PARAM_INT);
		$stmt->bindValue(':av', $app_version,       PDO::PARAM_STR);
		$stmt->bindValue(':dn', $device_name,       PDO::PARAM_STR);
		$stmt->bindValue(':dv', $device_version,    PDO::PARAM_STR);
		$stmt->bindValue(':uw', $unlocked_worlds,   PDO::PARAM_STR);
		$stmt->bindValue(':dr', $device_resolution, PDO::PARAM_STR);
		executeOrFail($stmt);
	}

	/**
	 * @param string $app_version
	 */
	public function UpdateLastOnline($app_version) {
		global $pdo;
		$stmt = $pdo->prepare("UPDATE users SET last_online=CURRENT_TIMESTAMP(), app_version=:av WHERE userid=:uid");
		$stmt->bindValue(':uid', $this->ID,   PDO::PARAM_INT);
		$stmt->bindValue(':av', $app_version, PDO::PARAM_STR);
		executeOrFail($stmt);
	}

	/**
	 * @param string $hash
	 * @param string $app_version
	 */
	public function ChangePassword($hash, $app_version) {
		global $pdo;
		$stmt = $pdo->prepare("UPDATE users SET password_hash=:pw, last_online=CURRENT_TIMESTAMP(), app_version=:av WHERE userid=:uid");
		$stmt->bindValue(':uid', $this->ID,   PDO::PARAM_INT);
		$stmt->bindValue(':av', $app_version, PDO::PARAM_STR);
		$stmt->bindValue(':pw', $hash,        PDO::PARAM_STR);
		executeOrFail($stmt);

		$this->PasswordHash = $hash;
	}

	/**
	 * @param int $score
	 * @param int $score_w1
	 * @param int $score_w2
	 * @param int $score_w3
	 * @param int $score_w4
	 * @param int $time
	 * @param int $time_w1
	 * @param int $time_w2
	 * @param int $time_w3
	 * @param int $time_w4
	 * @param int $score_mp
	 * @param string $app_version
	 */
	public function SetScoreAndTime($score, $score_w1, $score_w2, $score_w3, $score_w4, $time, $time_w1, $time_w2, $time_w3, $time_w4, $score_mp, $app_version) {
		global $pdo;
		$stmt = $pdo->prepare("UPDATE users SET score=:scr, score_w1=:scr1, score_w2=:scr2, score_w3=:scr3, score_w4=:scr4, time_total=:tim, time_w1=:tim1, time_w2=:tim2, time_w3=:tim3, time_w4=:tim4, mpscore=:smp, last_online=CURRENT_TIMESTAMP(), app_version=:av, revision_id=(revision_id+1) WHERE userid=:id");

		$stmt->bindValue(':id',   $this->ID, PDO::PARAM_INT);
		$stmt->bindValue(':av',   $app_version, PDO::PARAM_STR);
		$stmt->bindValue(':scr',  $score, PDO::PARAM_INT);
		$stmt->bindValue(':scr1', $score_w1, PDO::PARAM_INT);
		$stmt->bindValue(':scr2', $score_w2, PDO::PARAM_INT);
		$stmt->bindValue(':scr3', $score_w3, PDO::PARAM_INT);
		$stmt->bindValue(':scr4', $score_w4, PDO::PARAM_INT);
		$stmt->bindValue(':tim',  $time, PDO::PARAM_INT);
		$stmt->bindValue(':tim1', $time_w1, PDO::PARAM_INT);
		$stmt->bindValue(':tim2', $time_w2, PDO::PARAM_INT);
		$stmt->bindValue(':tim3', $time_w3, PDO::PARAM_INT);
		$stmt->bindValue(':tim4', $time_w4, PDO::PARAM_INT);
		$stmt->bindValue(':smp',  $score_mp, PDO::PARAM_INT);
		executeOrFail($stmt);

		$this->Score = $score;
		$this->ScoreW1 = $score_w1;
		$this->ScoreW2 = $score_w2;
		$this->ScoreW3 = $score_w3;
		$this->ScoreW4 = $score_w4;

		$this->TotalTime = $time;
		$this->TimeW1 = $time_w1;
		$this->TimeW2 = $time_w2;
		$this->TimeW3 = $time_w3;
		$this->TimeW4 = $time_w4;

		$this->RevID++;
	}

	/**
	 * (!) Does _NOT_ update RevID or Score in DB
	 *
	 * @param $levelid
	 * @param $difficulty
	 * @param $leveltime
	 * @return array
	 */
	public function InsertLevelScore($levelid, $difficulty, $leveltime) {
		global $pdo;

		$stmt = $pdo->prepare("SELECT levelid, difficulty, best_time FROM level_highscores WHERE userid=:uid AND levelid=:lid AND difficulty=:diff");
		$stmt->bindValue(':uid', $this->ID, PDO::PARAM_INT);
		$stmt->bindValue(':lid', $levelid, PDO::PARAM_STR);
		$stmt->bindValue(':diff', $difficulty, PDO::PARAM_INT);
		executeOrFail($stmt);
		$olddata = $stmt->fetchAll(PDO::FETCH_ASSOC);

		return $this->InsertLevelScoreInternal($levelid, $difficulty, $leveltime, empty($olddata) ? FALSE : $olddata[0]);
	}

	/**
	 * (!) Does _NOT_ update RevID or Score in DB
	 *
	 * @param array $scoredata
	 * @return int
	 */
	public function InsertMultiLevelScore($scoredata) {
		global $pdo;
		global $config;


		$stmt = $pdo->prepare("SELECT levelid, difficulty, best_time FROM level_highscores WHERE userid=:uid");
		$stmt->bindValue(':uid', $this->ID, PDO::PARAM_INT);
		executeOrFail($stmt);
		$olddata = $stmt->fetchAll(PDO::FETCH_ASSOC);


		$changecount = 0;
		foreach($scoredata as $new_data_row) {
			$levelid       = $new_data_row->levelid;
			$difficulty    = $new_data_row->difficulty;
			$leveltime     = $new_data_row->leveltime;

			if ($leveltime <= 0) { logMessage("The time $leveltime is not possible", "WARN"); continue; }
			if (!in_array($difficulty, $config['difficulties'], TRUE)) { logMessage("The difficulty $difficulty is not possible", "WARN"); continue; }
			if (!in_array($levelid, $config['levelids'], TRUE)) { logMessage("The levelID $levelid is not possible", "WARN"); continue; }

			$old_data_row = FALSE;
			foreach($olddata as $d) { if ($d['levelid'] == $levelid && $d['difficulty'] == $difficulty) $old_data_row = $d; }

			$r = $this->InsertLevelScoreInternal($levelid, $difficulty, $leveltime, $old_data_row);
			if ($r[0] != 1) $changecount++;
		}

		return $changecount;
	}

	/**
	 * @param string $levelid
	 * @param int $difficulty
	 * @param int $leveltime
	 * @param array|bool $olddata
	 * @return array
	 */
	private function InsertLevelScoreInternal($levelid, $difficulty, $leveltime, $olddata) {
		global $pdo;

		if ($olddata !== FALSE) {

			if ($olddata['best_time'] <= $leveltime) {
				// better or same value in db
				return [1, $olddata['best_time']];
			}

			// existing row in db
			$stmt = $pdo->prepare("UPDATE level_highscores SET best_time=:time, last_changed=CURRENT_TIMESTAMP() WHERE userid=:uid AND levelid=:lid AND difficulty=:diff");
			$stmt->bindValue(':uid', $this->ID, PDO::PARAM_INT);
			$stmt->bindValue(':lid', $levelid, PDO::PARAM_STR);
			$stmt->bindValue(':diff', $difficulty, PDO::PARAM_INT);
			$stmt->bindValue(':time', $leveltime, PDO::PARAM_INT);
			executeOrFail($stmt);

			return [2, $leveltime];
		} else {

			// no row in db
			$stmt = $pdo->prepare("INSERT INTO level_highscores (userid, levelid, difficulty, best_time, last_changed) VALUES (:uid, :lid, :diff, :time, CURRENT_TIMESTAMP())");
			$stmt->bindValue(':uid', $this->ID, PDO::PARAM_INT);
			$stmt->bindValue(':lid', $levelid, PDO::PARAM_STR);
			$stmt->bindValue(':diff', $difficulty, PDO::PARAM_INT);
			$stmt->bindValue(':time', $leveltime, PDO::PARAM_INT);
			executeOrFail($stmt);

			return [3, $leveltime];
		}
	}

	/**
	 * @return array
	 */
	public function GetAllLevelScoreEntries() {
		global $pdo;

		$stmt = $pdo->prepare("SELECT levelid, difficulty, best_time FROM level_highscores WHERE userid=:uid");
		$stmt->bindValue(':uid', $this->ID, PDO::PARAM_INT);
		executeOrFail($stmt);

		return $stmt->fetchAll(PDO::FETCH_ASSOC);
	}

	public function Delete() {
		global $pdo;

		$stmt = $pdo->prepare("DELETE FROM users WHERE userid=:uid");
		$stmt->bindValue(':uid', $this->ID, PDO::PARAM_INT);
		executeOrFail($stmt);

		$stmt = $pdo->prepare("DELETE FROM level_highscores WHERE userid=:uid");
		$stmt->bindValue(':uid', $this->ID, PDO::PARAM_INT);
		executeOrFail($stmt);
	}

	function jsonSerialize() {
		return
			[
				'ID'               => $this->ID,
				'Username'         => $this->Username,
				'AutoUser'         => $this->AutoUser,
				'MultiplayerScore' => $this->MultiplayerScore,
				'RevID'            => $this->RevID,
			];
	}
}