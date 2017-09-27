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

    $id = '@';
	if (!empty($_GET['id'])) $id = $_GET['id'];

	$page = 0;
	if (!empty($_GET['page'])) $page = $_GET['page'];


	if ($_GET['id']=='*') {
		$entries = getGlobalHighscores(1000, $page);
		$entrycount = getUserCount();
		$showtime = true;
    } else if ($_GET['id']=='@') {
		$entries = getMultiplayerHighscores(1000, $page);
		$entrycount = getUserCountWithMPScore();
		$showtime = false;
    } else {
		$entries = getWorldHighscores($_GET['id'], 1000, $page);
		$entrycount = getUserCount();
		$showtime = true;
    }


    ?>

    <h2><?php echo htmlspecialchars($_GET['id']); ?></h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
                <tr>
                    <th>Row</th>
                    <th style='width: 250px'>Username</th>
                    <th>Score</th>
                    <?php if ($showtime): ?>
                    <th>Time</th>
					<?php endif; ?>
                </tr>
            </thead>
            <?php $i=1 + ($page * 1000); foreach ($entries as $entry): ?>
                <tr>
                    <td><?php echo $i++; ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['totalscore']; ?></td>
                    <?php if ($showtime): ?>
                        <td title="<?php echo $entry['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", $entry['totaltime']/1000.0); ?></td>
                    <?php endif; ?>
                </tr>
            <?php endforeach; ?>
        </table>
        <div class="pagination_row">
            <?php for ($i=0; $i < ceil($entrycount/1000); $i++ ): ?>
                <?php if ($i != $page): ?>
                    <a class="pagination_link" href="<?php echo suffixGetParams('page', $i); ?>"><?php echo ($i+1); ?></a>
                <?php else: ?>
                    <a class="pagination_curr"><?php echo ($i+1); ?></a>
                <?php endif; ?>
            <?php endfor; ?>
        </div>
    </div>

    <?php printSQLStats(); ?>

    <script src="sorttable.js"></script>

</body>
</html>