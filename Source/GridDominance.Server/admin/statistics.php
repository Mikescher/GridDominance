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

$u1 = statisticsUserByDevice();
$u2 = statisticsUserByOS();
$u3 = statisticsUserByResolution();
$u4 = statisticsUserByAppVersion();
$u5 = statisticsUserByUnlocks();
$u6 = statisticsUserByAnon();
$u7 = statisticsUserByAppType();
$u8 = statisticsUserByScoreRange();
$u9 = statisticsUserByWorld();

?>

<div class="columnbox3">

    <div class="column3_0" data-collapse>
        <h2 class="collapseheader">Users By Device</h2>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered sortable">
                <thead>
                <tr>
                    <th style='min-width: 300px'>Device</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u1 as $entry): ?>
                    <tr>
                        <td><a class="nolink" href="userlist.php?device=<?php echo urlencode($entry['name']); ?>"><?php echo $entry['name']; ?></a></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

    <div class="column3_1" data-collapse>
        <h2 class="collapseheader">Users By Operating System</h2>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered sortable">
                <thead>
                <tr>
                    <th style='width: 350px'>Operating System</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u2 as $entry): ?>
                    <tr>
                        <td><a class="nolink" href="userlist.php?device_version=<?php echo urlencode($entry['name']); ?>"><?php echo $entry['name']; ?></a></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>
    </div>

    <div class="column3_2" data-collapse>
        <h2 class="collapseheader">Users By Resolution</h2>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered sortable">
                <thead>
                <tr>
                    <th style='min-width: 200px'>Resolution</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u3 as $entry): ?>
                    <tr>
                        <td><a class="nolink" href="userlist.php?resolution=<?php echo urlencode($entry['name']); ?>"><?php echo $entry['name']; ?></a></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

</div>

<div class="columnbox3">

    <div class="column3_0" data-collapse>
        <h2 class="collapseheader">Users By App Version</h2>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered sortable">
                <thead>
                <tr>
                    <th style='min-width: 170px'>App Version</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u4 as $entry): ?>
                    <tr>
                        <td><a class="nolink" href="userlist.php?app_version=<?php echo urlencode($entry['name']); ?>"><?php echo $entry['name']; ?></a></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

    <div class="column3_1" data-collapse>
        <h2 class="collapseheader">Users By Unlocks</h2>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered sortable">
                <thead>
                <tr>
                    <th style='min-width: 220px'>World</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u5 as $entry): ?>
					<?php if ($entry['count'] > 0): ?>
                        <tr>
                            <td title="<?php echo $entry['name']; ?>" ><?php echo fmtWorldID($entry['name']); ?></td>
                            <td><?php echo $entry['count']; ?></td>
                        </tr>
					<?php endif; ?>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

    <div class="column3_2" data-collapse>
        <h2 class="collapseheader">Users By Anon</h2>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered sortable">
                <thead>
                <tr>
                    <th>Anonymous User</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u6 as $entry): ?>
                    <tr>
                        <td><a class="nolink" href="userlist.php?anon=<?php echo ($entry['name']?'y':'n'); ?>"><?php echo $entry['name']; ?></a></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

</div>

<div class="columnbox3">

    <div class="column3_0" data-collapse>
        <h2 class="collapseheader">Users By App Type</h2>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered sortable">
                <thead>
                <tr>
                    <th style='min-width: 170px'>Type</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u7 as $entry): ?>
                    <tr>
                        <td><a class="nolink" href="userlist.php?app_type=<?php echo urlencode($entry['name']); ?>"><?php echo $entry['name']; ?></a></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

    <div class="column3_1" data-collapse>
        <h2 class="collapseheader">Users By Score Range</h2>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered sortable">
                <thead>
                <tr>
                    <th style='min-width: 170px'>Score Range</th>
                    <th>Count</th>
                </tr>
                </thead>
                    <tr>
                        <td>0</td>
                        <td><?php echo countZeroScoreUsers(); ?></td>
                    </tr>
				<?php foreach ($u8 as $entry): ?>
                    <tr>
                        <td><?php echo $entry['score1']; ?> - <?php echo $entry['score2']; ?></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
                    <tr>
                        <td>MAX</td>
                        <td><?php echo countFirstPlaceUsers(); ?></td>
                    </tr>
            </table>
        </div>

    </div>

    <div class="column3_2" data-collapse>
        <h2 class="collapseheader">Users By World</h2>

        <div class="tablebox">
            <table class="sqltab pure-table pure-table-bordered sortable">
                <thead>
                <tr>
                    <th>Anonymous User</th>
                    <th>Count</th>
                </tr>
                </thead>
				<?php foreach ($u9 as $entry): ?>
                    <tr>
                        <td><?php echo fmtWorldID($entry['world']); ?></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

</div>

<?php printSQLStats(); ?>
</body>
</html>