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

	$showall = false;
	if (! empty($_GET['a'])) $showall = $_GET['a'] == 'y';

	if (empty($_GET['d']))
		$users = getUsers($showall);
	else
		$users = getActiveUsers($_GET['d'], $showall);

    ?>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
                <tr>
                    <th style='width: 170px'>Username</th>
                    <th>Password</th>
                    <th>Anon</th>
                    <th>Score</th>
                    <th>Score(MP)</th>
                    <th>Rev ID</th>
                    <th style='width: 200px'>Devicename</th>
                    <th style='width: 350px'>Deviceversion</th>
                    <th>Resolution</th>
                    <th>Unlocks</th>
                    <th>Multiplayer</th>
                    <th style='width: 170px'>Last Online</th>
                    <th>App Version</th>

                </tr>
            </thead>
            <?php foreach ($users as $entry): ?>
                <tr>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <?php expansioncell($entry['password_hash']); ?>
                    <td><?php echo $entry['is_auto_generated']; ?></td>
                    <td><?php echo $entry['score']; ?></td>
                    <td><?php echo $entry['mpscore']; ?></td>
                    <td><?php echo $entry['revision_id']; ?></td>
                    <td><?php echo $entry['device_name']; ?></td>
                    <td><?php echo $entry['device_version']; ?></td>
                    <td><?php echo $entry['device_resolution']; ?></td>
                    <?php expansioncell3($entry['unlocked_worlds'], lc($entry['unlocked_worlds'])); ?>
                    <td><?php echo strpos($entry['unlocked_worlds'], '{d34db335-0001-4000-7711-000000300001}') ? 'TRUE' : 'FALSE'; ?></td>
                    <td><?php echo $entry['last_online']; ?></td>
                    <td><?php echo $entry['app_version']; ?></td>
                </tr>
                <tr class='tab_prev' id='tr_prev_<?php echo $previd; ?>'><td colspan='12' id='td_prev_<?php echo $previd; ?>' style='text-align: left;' ></td></tr>
                <?php $previd++; ?>
            <?php endforeach; ?>
        </table>
    </div>

    <?php if (! $showall): ?>
        <div style="text-align: right; font-size: xx-large">
			<?php
            if (empty($_SERVER['QUERY_STRING']))
				echo " <a href=\"userlist.php?d=y\">Show All</a>";
            else
				echo " <a href=\"userlist.php?" . $_SERVER['QUERY_STRING'] . "&a=y\">Show All</a>";
			?>

        </div>
    <?php endif; ?>

    <script type="text/javascript">
		<?php echo file_get_contents('admin.js'); ?>
    </script>
    <script src="sorttable.js"></script>
</body>
</html>