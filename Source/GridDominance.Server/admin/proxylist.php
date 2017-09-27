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

	$file = file_get_contents("/var/log/gdapi_log/proxystate.json");
	$state = json_decode($file, true);

    ?>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
                <tr>
                    <th>SessionID</th>
                    <th>SessionSecret</th>
                    <th>UserCount</th>
                    <th>SessionSize</th>
                </tr>
            </thead>
            <?php foreach ($state['sessions'] as $entry): ?>
                <tr>
                    <td><?php echo $entry['sid']; ?></td>
                    <td><?php echo $entry['sec']; ?></td>
                    <td><?php echo $entry['usr']; ?></td>
                    <td><?php echo $entry['cap']; ?></td>
                </tr>
            <?php endforeach; ?>
        </table>
    </div>

    <?php printSQLStats(); ?>

    <script src="sorttable.js"></script>
</body>
</html>