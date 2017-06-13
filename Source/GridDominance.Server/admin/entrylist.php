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

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered">
            <thead>
                <tr>
                    <th style='width: 170px'>Username</th>
                    <th>Level</th>
                    <th>Difficulty</th>
                    <th>Time</th>
                    <th style='width: 170px'>Last changed</th>
                </tr>
            </thead>
            <?php foreach (getAllEntries() as $entry): ?>
                <tr>
                    <td><?php echo $entry['username'] . " (" . $entry['userid'] . ")" ; ?></td>
                    <td title="<?php echo $entry['levelid']; ?>" >
                        <?php echo (int)substr($entry['levelid'], 25, 6) . " - " . (int)substr($entry['levelid'], 31, 6); ?>
                    </td>
                    <td><?php echo $entry['difficulty']; ?></td>
                    <td title="<?php echo $entry['best_time']; ?>ms" ><?php echo gmdate("H:i:s", $entry['best_time']/1000.0); ?></td>
                    <td><?php echo $entry['last_changed']; ?></td>
                </tr>
            <?php endforeach; ?>
        </table>
    </div>
</body>
</html>