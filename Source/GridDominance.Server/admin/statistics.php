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
function fmtw($w) {
    switch ($w) {
		case '{d34db335-0001-4000-7711-000000100002}': return 'NO_WORLD';
		case '{d34db335-0001-4000-7711-000000100001}': return 'TUTORIAL';
		case '{d34db335-0001-4000-7711-000000200001}': return 'WORLD_1';
		case '{d34db335-0001-4000-7711-000000200002}': return 'WORLD_2';
		case '{d34db335-0001-4000-7711-000000200003}': return 'WORLD_3';
		case '{d34db335-0001-4000-7711-000000200004}': return 'WORLD_4';
		case '{d34db335-0001-4000-7711-000000200005}': return 'WORLD_5';
		case '{d34db335-0001-4000-7711-000000200006}': return 'WORLD_6';
		case '{d34db335-0001-4000-7711-000000200007}': return 'WORLD_7';
		case '{d34db335-0001-4000-7711-000000200008}': return 'WORLD_8';
		case '{d34db335-0001-4000-7711-000000200009}': return 'WORLD_9';
		case '{d34db335-0001-4000-7711-000000300001}': return 'MULTIPLAYER';
        default:                                       return "????";
    }
}
?>

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
                            <td title="<?php echo $entry['name']; ?>" ><?php echo fmtw($entry['name']); ?></td>
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
                        <td><?php echo fmtw($entry['world']); ?></td>
                        <td><?php echo $entry['count']; ?></td>
                    </tr>
				<?php endforeach; ?>
            </table>
        </div>

    </div>

</div>

<?php printSQLStats(); ?>

<script type="text/javascript">
	<?php echo file_get_contents('admin.js'); ?>
</script>
<script src="sorttable.js"></script>
<script src="jquery.collapse.js"></script>
</body>
</html>