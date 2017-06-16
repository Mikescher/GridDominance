<?php require_once '../internals/backend.php'; ?>
<?php require_once '../internals/utils.php'; ?>
<?php init("admin"); ?>
<!doctype html>

<html lang="en">
<head>
    <meta charset="utf-8">
    <link rel="stylesheet" href="pure-min.css"/>
    <link rel="stylesheet" type="text/css" href="admin.css">
</head>

<body id="rootbox">

<script src="jquery-3.1.0.min.js"></script>

<h1><a href="index.php">Cannon Conquest | Admin Page</a></h1>

<?php

$previd = 0;
function expansioncell($txt) {
	global $previd;

	echo "<td>";
	echo "<a href='#' onclick='ShowExpandedColumn(" . $previd . ", " . str_replace("'", "\\u0027", str_replace('\n', '<br/>', json_encode($txt))) . ");return false;'>show</a>";
	echo "</td>";
}
function expansioncell3($txt, $hdr) {
	global $previd;

	echo "<td>";
	echo "<a href='#' onclick='ShowExpandedColumn(" . $previd . ", " . str_replace("'", "\\u0027", str_replace('\n', '<br/>', json_encode($txt))) . ");return false;'>show (" . $hdr . ")</a>";
	echo "</td>";
}
function lc($txt) {
	$c = 0;
	foreach (explode("\n", $txt) as $l) { if (!empty($l)) $c++; }
	return $c;
}
function pqi($sql) {
	global $pdo;
	$stmt = $pdo->query($sql);
	$stmt->execute();
	return $stmt->fetch(PDO::FETCH_NUM)[0];
}
function fmtw($w) {
    switch ($w) {
		case '{d34db335-0001-4000-7711-000000100002}': return 'NO_WORLD';
		case '{d34db335-0001-4000-7711-000000100001}': return 'TUTORIAL';
		case '{d34db335-0001-4000-7711-000000200001}': return 'WORLD_1';
		case '{d34db335-0001-4000-7711-000000200002}': return 'WORLD_2';
		case '{d34db335-0001-4000-7711-000000200003}': return 'WORLD_3';
		case '{d34db335-0001-4000-7711-000000200004}': return 'WORLD_4';
		case '{d34db335-0001-4000-7711-000000200005}': return 'WORLD_5';
		case '{d34db335-0001-4000-7711-000000200006}': return 'WORLD_6';
		case '{d34db335-0001-4000-7711-000000200007}': return 'WORLD_7';
		case '{d34db335-0001-4000-7711-000000200008}': return 'WORLD_8';
		case '{d34db335-0001-4000-7711-000000200009}': return 'WORLD_9';
		case '{d34db335-0001-4000-7711-000000300001}': return 'MULTIPLAYER';
    }
}
?>

<div class="columnbox3">

    <div class="column3_0">

        <h2>Users By Device</h2>

		<?php
		global $pdo;
		$u1 =  $pdo->query('SELECT device_name AS name, COUNT(*) AS count FROM users GROUP BY device_name')->fetchAll(PDO::FETCH_ASSOC);
		?>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered">
                <thead>
                <tr>
                    <th style='min-width: 300px'>Device</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u1 as $entry): ?>
                    <tr>
                        <td><?php echo $entry['name']; ?></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

    <div class="column3_1">

        <h2>Users By Operating System</h2>

		<?php
		global $pdo;
		$u2 =  $pdo->query('SELECT device_version AS name, COUNT(*) AS count FROM users GROUP BY device_version')->fetchAll(PDO::FETCH_ASSOC);
		?>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered">
                <thead>
                <tr>
                    <th style='width: 350px'>Operating System</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u2 as $entry): ?>
                    <tr>
                        <td><?php echo $entry['name']; ?></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>
    </div>

    <div class="column3_2">

        <h2>Users By Resolution</h2>

		<?php
		global $pdo;
		$u3 =  $pdo->query('SELECT device_resolution AS name, COUNT(*) AS count FROM users GROUP BY device_resolution')->fetchAll(PDO::FETCH_ASSOC);
		?>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered">
                <thead>
                <tr>
                    <th style='min-width: 200px'>Resolution</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u3 as $entry): ?>
                    <tr>
                        <td><?php echo $entry['name']; ?></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

</div>

<div class="columnbox3">

    <div class="column3_0">

        <h2>Users By App Version</h2>

		<?php
		global $pdo;
		$u4 =  $pdo->query('SELECT app_version AS name, COUNT(*) AS count FROM users GROUP BY app_version')->fetchAll(PDO::FETCH_ASSOC);
		?>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered">
                <thead>
                <tr>
                    <th style='min-width: 170px'>App Version</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u4 as $entry): ?>
                    <tr>
                        <td><?php echo $entry['name']; ?></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

    <div class="column3_1">

        <h2>Users By Unlocks</h2>

		<?php
        global $config;

		$u5 = [];
		$u5[] = ['name' => '{d34db335-0001-4000-7711-000000100001}', 'count' => pqi("SELECT COUNT(*) AS count FROM users WHERE unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000100001}%'")];
		$u5[] = ['name' => '{d34db335-0001-4000-7711-000000100002}', 'count' => pqi("SELECT COUNT(*) AS count FROM users WHERE unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000100002}%'")];
		$u5[] = ['name' => '{d34db335-0001-4000-7711-000000300001}', 'count' => pqi("SELECT COUNT(*) AS count FROM users WHERE unlocked_worlds LIKE '%{d34db335-0001-4000-7711-000000300001}%'")];

		foreach (array_unique(array_map(function($k){ return $k[0]; }, $config['levelmapping'])) as $w) {
			$u5[] = ['name' => $w, 'count' => pqi("SELECT COUNT(*) AS count FROM users WHERE unlocked_worlds LIKE '%" . $w . "%'")];
		}

		usort($u5, function ($a, $b) { return ($a['name'] <=> $b['name']); })

		?>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered">
                <thead>
                <tr>
                    <th style='min-width: 220px'>World</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u5 as $entry): ?>
					<?php if ($entry['count'] > 0): ?>
                        <tr>
                            <td title="<?php echo $entry['name']; ?>" ><?php echo fmtw($entry['name']); ?></td>
                            <td><?php echo $entry['count']; ?></td>
                        </tr>
					<?php endif; ?>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

    <div class="column3_3">

        <h2>Users By Anon</h2>

		<?php
		global $pdo;
		$u1 =  $pdo->query('SELECT is_auto_generated AS name, COUNT(*) AS count FROM users GROUP BY is_auto_generated')->fetchAll(PDO::FETCH_ASSOC);
		?>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered">
                <thead>
                <tr>
                    <th>Anonymous User</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u1 as $entry): ?>
                    <tr>
                        <td><?php echo $entry['name']; ?></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

</div>

<script type="text/javascript">
	<?php echo file_get_contents('admin.js'); ?>
</script>
</body>
</html>