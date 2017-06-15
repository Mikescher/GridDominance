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

    <h2><?php echo htmlspecialchars($_GET['id']); ?></h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered">
            <thead>
                <tr>
                    <th style='width: 250px'>Username</th>
                    <th>Score</th>
                    <th>Time</th>
                </tr>
            </thead>
            <?php foreach (getWorldHighscores($_GET['id']) as $entry): ?>
                <tr>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['totalscore']; ?></td>
                    <td title="<?php echo $entry['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", $entry['totaltime']/1000.0); ?></td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>
</body>
</html>