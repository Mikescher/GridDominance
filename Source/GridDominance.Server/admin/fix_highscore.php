<?php require_once '../internals/backend.php'; ?>
<?php require_once '../internals/utils.php'; ?>
<?php require_once 'common/libadmin.php'; ?>
<?php init("admin"); ?>

<?php

try {

	global $pdo;
	global $config;

	function fetch($uid)
	{
		global $pdo;

		$stmt = $pdo->prepare(loadSQL('manual_calculate_time'));
		$stmt->bindValue(':uid1', $uid, PDO::PARAM_INT);
		$stmt->bindValue(':uid2', $uid, PDO::PARAM_INT);
		$stmt->bindValue(':uid3', $uid, PDO::PARAM_INT);
		$stmt->bindValue(':uid4', $uid, PDO::PARAM_INT);
		$stmt->execute();

		$manual = [];
		foreach ($stmt->fetchAll(PDO::FETCH_ASSOC) as $row) {
			$manual[$row['worldid']] = $row['best_time'];
		}
		return $manual;
	}

	ob_end_flush();


	$stmt = $pdo->prepare("(SELECT * FROM users ORDER BY score DESC LIMIT 500) UNION (SELECT * FROM users ORDER BY score_w1 DESC LIMIT 500) UNION (SELECT * FROM users ORDER BY score_w2 DESC LIMIT 500) UNION (SELECT * FROM users ORDER BY score_w3 DESC LIMIT 500) UNION (SELECT * FROM users ORDER BY score_w4 DESC LIMIT 500)");
	$stmt->execute();


	$rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

	echo "<table>";

	foreach ($rows as $row) {

		$f = fetch($row['userid']);

		$dd1 = ($f['*'] != $row['time_total']);
		$dd2 = ($f['{d34db335-0001-4000-7711-000000200001}'] != $row['time_w1']);
		$dd3 = ($f['{d34db335-0001-4000-7711-000000200002}'] != $row['time_w2']);
		$dd4 = ($f['{d34db335-0001-4000-7711-000000200003}'] != $row['time_w3']);
		$dd5 = ($f['{d34db335-0001-4000-7711-000000200004}'] != $row['time_w4']);

		if ($dd1 || $dd2 || $dd3 || $dd4 || $dd5) {

			echo "<tr>";
			echo "<td>" . $row['username'] . " (" . $row['userid'] . ")" . "</td>";
			echo "<td>" . $row['app_version'] . "</td>";
			echo "<td>" . $row['time_total'] . "</td>";
			echo "<td>" . $f['*'] . "</td>";
			echo "<td>" . ($dd1 ? '!' : '') . "</td>";
			echo "<tr>";

			echo "<tr>";
			echo "<td></td>";
			echo "<td></td>";
			echo "<td>" . $row['time_w1'] . "</td>";
			echo "<td>" . $f['{d34db335-0001-4000-7711-000000200001}'] . "</td>";
			echo "<td>" . ($dd2 ? '!' : '') . "</td>";
			echo "<tr>";

			echo "<tr>";
			echo "<td></td>";
			echo "<td></td>";
			echo "<td>" . $row['time_w2'] . "</td>";
			echo "<td>" . $f['{d34db335-0001-4000-7711-000000200002}'] . "</td>";
			echo "<td>" . ($dd3 ? '!' : '') . "</td>";
			echo "<tr>";

			echo "<tr>";
			echo "<td></td>";
			echo "<td></td>";
			echo "<td>" . $row['time_w3'] . "</td>";
			echo "<td>" . $f['{d34db335-0001-4000-7711-000000200003}'] . "</td>";
			echo "<td>" . ($dd4 ? '!' : '') . "</td>";
			echo "<tr>";

			echo "<tr>";
			echo "<td></td>";
			echo "<td></td>";
			echo "<td>" . $row['time_w4'] . "</td>";
			echo "<td>" . $f['{d34db335-0001-4000-7711-000000200004}'] . "</td>";
			echo "<td>" . ($dd5 ? '!' : '') . "</td>";
			echo "<tr>";


			$stmt = $pdo->prepare('UPDATE users SET users.time_total=:t0, users.time_w1=:t1, users.time_w2=:t2, users.time_w3=:t3, users.time_w4=:t4 WHERE users.userid=:uid');
			$stmt->bindValue(':uid', $row['userid'], PDO::PARAM_INT);
			$stmt->bindValue(':t0', $f['*'], PDO::PARAM_INT);
			$stmt->bindValue(':t1', $f['{d34db335-0001-4000-7711-000000200001}'], PDO::PARAM_INT);
			$stmt->bindValue(':t2', $f['{d34db335-0001-4000-7711-000000200002}'], PDO::PARAM_INT);
			$stmt->bindValue(':t3', $f['{d34db335-0001-4000-7711-000000200003}'], PDO::PARAM_INT);
			$stmt->bindValue(':t4', $f['{d34db335-0001-4000-7711-000000200004}'], PDO::PARAM_INT);
			$stmt->execute();
			echo "<tr><td>" . $stmt->rowCount() . " rows affected" . "</td></tr>";
		}
	}

	echo "</table>";

} catch (Exception $e) {

	echo $e->getMessage();

}