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
	function fmtLevelID($id)
	{
		if ($id == '{b16b00b5-0001-4000-9999-000000000002}') return "TUTORIAL";

		return (int)substr($id, 25, 6) . " - " . (int)substr($id, 31, 6);
	}
    ?>

    <h2><?php echo htmlspecialchars($_GET['id']); ?></h2>

    <h2>DIFFICULTY 0</h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
                <tr>
                    <th style='width: 170px'>Username</th>
                    <th>Level</th>
                    <th>Difficulty</th>
                    <th>Time</th>
                    <th style='width: 170px'>Last changed</th>
                </tr>
            </thead>
            <?php foreach (getLevelDiffEntries($_GET['id'], 0) as $entry): ?>
                <tr>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td title="<?php echo $entry['levelid']; ?>" >
                        <?php echo fmtLevelID($entry['levelid']); ?>
                    </td>
                    <td><?php echo $entry['difficulty']; ?></td>
                    <td title="<?php echo $entry['best_time']; ?>ms" ><?php echo gmdate("H:i:s", $entry['best_time']/1000.0); ?></td>
                    <td><?php echo $entry['last_changed']; ?></td>
                </tr>
            <?php endforeach; ?>
        </table>
    </div>

    <h2>DIFFICULTY 1</h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th style='width: 170px'>Username</th>
                <th>Level</th>
                <th>Difficulty</th>
                <th>Time</th>
                <th style='width: 170px'>Last changed</th>
            </tr>
            </thead>
			<?php foreach (getLevelDiffEntries($_GET['id'], 1) as $entry): ?>
                <tr>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td title="<?php echo $entry['levelid']; ?>" >
						<?php echo fmtLevelID($entry['levelid']); ?>
                    </td>
                    <td><?php echo $entry['difficulty']; ?></td>
                    <td title="<?php echo $entry['best_time']; ?>ms" ><?php echo gmdate("H:i:s", $entry['best_time']/1000.0); ?></td>
                    <td><?php echo $entry['last_changed']; ?></td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <h2>DIFFICULTY 2</h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th style='width: 170px'>Username</th>
                <th>Level</th>
                <th>Difficulty</th>
                <th>Time</th>
                <th style='width: 170px'>Last changed</th>
            </tr>
            </thead>
			<?php foreach (getLevelDiffEntries($_GET['id'], 2) as $entry): ?>
                <tr>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td title="<?php echo $entry['levelid']; ?>" >
						<?php echo fmtLevelID($entry['levelid']); ?>
                    </td>
                    <td><?php echo $entry['difficulty']; ?></td>
                    <td title="<?php echo $entry['best_time']; ?>ms" ><?php echo gmdate("H:i:s", $entry['best_time']/1000.0); ?></td>
                    <td><?php echo $entry['last_changed']; ?></td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <h2>DIFFICULTY 3</h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th style='width: 170px'>Username</th>
                <th>Level</th>
                <th>Difficulty</th>
                <th>Time</th>
                <th style='width: 170px'>Last changed</th>
            </tr>
            </thead>
			<?php foreach (getLevelDiffEntries($_GET['id'], 3) as $entry): ?>
                <tr>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td title="<?php echo $entry['levelid']; ?>" >
						<?php echo fmtLevelID($entry['levelid']); ?>
                    </td>
                    <td><?php echo $entry['difficulty']; ?></td>
                    <td title="<?php echo $entry['best_time']; ?>ms" ><?php echo gmdate("H:i:s", $entry['best_time']/1000.0); ?></td>
                    <td><?php echo $entry['last_changed']; ?></td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <script src="sorttable.js"></script>
</body>
</html>