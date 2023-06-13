<?php require_once '../internals/backend.php'; ?>
<?php require_once '../internals/utils.php'; ?>
<?php require_once 'common/libadmin.php'; ?>
<?php init("admin"); ?>
<!doctype html>

<html lang="en">
<head>
	<meta charset="utf-8">
	<?php includeStyles(); ?>
</head>

<body id="rootbox">

<?php includeScripts(); ?>

    <h1><a href="index.php">Cannon Conquest | Admin Page</a></h1>

    <?php

        $page = 0;
        if (!empty($_GET['page'])) $page = $_GET['page'];
        $entrycount = getEntryCount();
        $guesscount = guessEntryCount();
    ?>

    <div class="infocontainer">
        <div class="infodiv">
            Count (real): <?php echo $entrycount ; ?>
        </div>
        <div class="infodiv">
            Count (guess): <?php echo $guesscount ; ?>
        </div>
    </div>

    <?php

    $dist = getEntryChangedDistribution();
    $udates = [];
    for ($i=0; $i < count($dist); $i++) $udates []= $dist[$i]['date'];

    ?>

    <div class="graphbox" data-collapse>
        <h2 class="open collapseheader">History</h2>
        <div>
            <canvas id="scoreChart1" width="85%" height="25%"></canvas>
            <script>
                let ctx1 = document.getElementById("scoreChart1").getContext('2d');

                new Chart(ctx1,
                    {
                        type: 'line',
                        data:
                            {
                                labels: [ <?php foreach ($udates as $rld) echo "'".$rld."',"; ?> ],
                                datasets:
                                    [
                                        {
                                            label: 'last_changed',
                                            data: [ <?php foreach ($dist as $dd) echo $dd['count'].","; ?> ],
                                        },
                                    ]
                            },
                    });
            </script>
        </div>
    </div>

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
            <?php foreach (getAllEntries($page, 500) as $entry): ?>
                <tr>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td title="<?php echo $entry['levelid']; ?>" >
                        <a href="levelscores.php?id=<?php echo $entry['levelid']; ?>">
							<?php echo fmtLevelID($entry['levelid']); ?>
                        </a>
                    </td>
                    <td><?php echo $entry['difficulty']; ?></td>
                    <td title="<?php echo $entry['best_time']; ?>ms" ><?php echo gmdate("H:i:s", (int)($entry['best_time']/1000.0)); ?></td>
                    <td><?php echo $entry['last_changed']; ?></td>
                </tr>
            <?php endforeach; ?>
        </table>
        <div class="pagination_row">
            <?php for ($i=0; $i < ceil($entrycount/500); $i++ ): ?>
                <?php if ($i != $page): ?>
                    <a class="pagination_link" href="entrylist.php?page=<?php echo $i; ?>"><?php echo ($i+1); ?></a>
                <?php else: ?>
                    <a class="pagination_curr"><?php echo ($i+1); ?></a>
                <?php endif; ?>
            <?php endfor; ?>
        </div>
    </div>

	<?php printSQLStats(); ?>
</body>
</html>