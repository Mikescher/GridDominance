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

	<div class="infocontainer">
		<div class="infodiv">
            Count(history):&nbsp;<?php echo getRunLogCountHistory(); ?>
		</div>
		<div class="infodiv">
            Count(volatile):&nbsp;<?php echo getRunLogCountVolatile(); ?>
		</div>
	</div>

    <?php

	$rloglist = getRunLogActionList();

    ?>

    <?php foreach($rloglist as $raction): ?>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader"><?php echo $raction['action']; ?></h2>

        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th width="150px">Time (Cron)</th>
                <th width="400px">Timesspan</th>
                <th width="100px">Count</th>
                <th width="150px">Duration (Avg)</th>
            </tr>
            </thead>
			<?php foreach (getRunLog($raction['action']) as $entry): ?>
                <tr>
                    <td><?php echo $entry['exectime']; ?></td>
                    <td><?php echo $entry['min_timestamp'] .  " - " . $entry['max_timestamp']; ?></td>
                    <td><?php echo $entry['count']; ?></td>
                    <td><?php echo round(($entry['duration']/$entry['count'])/(1000.0*1000.0), 5); ?> s</td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <?php endforeach; ?>

    <?php printSQLStats(); ?>

    <script type="text/javascript">
		<?php echo file_get_contents('admin.js'); ?>
    </script>

    <script src="sorttable.js"></script>
    <script src="jquery.collapse.js"></script>
</body>
</html>