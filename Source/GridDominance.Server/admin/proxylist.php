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

	$arr = getActiveAndTotalSessionsCount();
	$count_active = $arr[0];
	$count_total  = $arr[1];

	$file = file_get_contents("/var/log/gdapi_log/proxystate.json");
	$state = json_decode($file, true);

	$timestamp = date('Y-m-d H:i:s', filemtime("/var/log/gdapi_log/proxystate.json"));

	$history = getProxyHistory();

    ?>

    <div class="infocontainer">
        <div class="infodiv">
            Count (active): <?php echo $count_active ; ?>
        </div>
        <div class="infodiv">
            Count (total): <?php echo $count_total ; ?>
        </div>
        <div class="infodiv">
            Laste State change: <?php echo $timestamp ; ?>
        </div>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Current</h2>

        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
                <tr>
                    <th>SessionID</th>
                    <th>SessionSecret</th>
                    <th>UserCount</th>
                    <th>SessionSize</th>
                    <th>Active</th>
                </tr>
            </thead>
            <?php foreach ($state['sessions'] as $entry): ?>
                <tr>
                    <td><?php echo $entry['sid']; ?></td>
                    <td><?php echo $entry['sec']; ?></td>
                    <td><?php echo $entry['usr']; ?></td>
                    <td><?php echo $entry['cap']; ?></td>
                    <td><?php echo $entry['act']?'true':'false'; ?></td>
                </tr>
            <?php endforeach; ?>
        </table>
    </div>

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
                                labels: [ <?php foreach ($history as $h) echo "'".$h['time']."',"; ?> ],
                                datasets:
                                    [
                                        {
                                            label: 'active sessions',
                                            data: [ <?php foreach ($history as $h) echo $h['sessioncount_active'].","; ?> ],
                                        },
                                        {
                                            label: 'total sessions',
                                            data: [ <?php foreach ($history as $h) echo $h['sessioncount_total'].","; ?> ],
                                        },
                                    ]
                            },
                    });
            </script>
        </div>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="collapseheader">Log</h2>

        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th style="width: 200px;">Time</th>
                <th>Sessions (active)</th>
                <th>Sessions (total)</th>
            </tr>
            </thead>
			<?php foreach ($history as $entry): ?>
                <tr>
                    <td><?php echo $entry['time']; ?></td>
                    <td><?php echo $entry['sessioncount_active']; ?></td>
                    <td><?php echo $entry['sessioncount_total']; ?></td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <?php printSQLStats(); ?>
</body>
</html>