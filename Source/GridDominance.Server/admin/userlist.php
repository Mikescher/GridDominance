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
    <script src="Chart.min.js"></script>

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
	function expansioncell4($hdr, $dat) {
		global $previd;

		echo "<td>";
		echo "<a href='#' onclick='ShowExpandedColumn(" . $previd . ", " . str_replace("'", "\\u0027", str_replace('\n', '<br/>', json_encode($dat))) . ");return false;'>$hdr</a>";
		echo "</td>";
	}

	function lc($txt) {
	    $c = 0;
	    foreach (explode("\n", $txt) as $l) { if (!empty($l)) $c++; }
	    return $c;
	}

	function getScoreInfo($entry)
    {
	    return
            "Score     = " . $entry['score']      . "\n" .
			"Score[W1] = " . $entry['score_w1']   . "\n" .
			"Score[W2] = " . $entry['score_w2']   . "\n" .
			"Score[W3] = " . $entry['score_w3']   . "\n" .
			"Score[W4] = " . $entry['score_w4']   . "\n" .
			"Time      = " . gmdate("H:i:s", $entry['time_total']) . " = " . $entry['time_total'] . "\n" .
			"Time[W1]  = " . gmdate("H:i:s", $entry['time_w1'])    . " = " . $entry['time_w1']    . "\n" .
			"Time[W2]  = " . gmdate("H:i:s", $entry['time_w2'])    . " = " . $entry['time_w2']    . "\n" .
			"Time[W3]  = " . gmdate("H:i:s", $entry['time_w3'])    . " = " . $entry['time_w3']    . "\n" .
			"Time[W4]  = " . gmdate("H:i:s", $entry['time_w4'])    . " = " . $entry['time_w4']    . "\n" .
			"Score[MP] = " . $entry['mpscore']    . "\n" ;
    }

	$showall = false;
	if (! empty($_GET['a']) && $_GET['a'] == 'y') $showall = true;
	if (! empty($_GET['a']) && $_GET['a'] == 'n') $showall = false;

    $anon = null;
	if (!empty($_GET['anon']) && $_GET['anon']=='y') $anon = false;
	if (!empty($_GET['anon']) && $_GET['anon']=='n') $anon = true;

    $page = 0;
    if (!empty($_GET['page'])) $page = $_GET['page'];

    $days = -1;
	if (!empty($_GET['d'])) $days = $_GET['d'];

    $device = '';
	if (!empty($_GET['device'])) $device = $_GET['device'];

    $os = '';
	if (!empty($_GET['device_version'])) $os = $_GET['device_version'];

    $resolution = '';
	if (!empty($_GET['resolution'])) $resolution = $_GET['resolution'];

    $appversion = '';
	if (!empty($_GET['app_version'])) $appversion = $_GET['app_version'];

    $apptype = '';
	if (!empty($_GET['app_type'])) $apptype = $_GET['app_type'];

	$users = getUsers($showall, $anon, $days, $device, $os, $resolution, $appversion, $apptype, $page, 500);
	$entrycount = countUsers($showall, $anon, $days, $device, $os, $resolution, $appversion, $apptype);

    ?>

    <div class="infocontainer">
        <div class="infodiv">
            All: <?php echo countUsers(true, null, -1, "", "", "", "", ""); ?>
        </div>
        <div class="infodiv">
            With score: <?php echo countUsers(false, null, -1, "", "", "", "", ""); ?>
        </div>
        <div class="infodiv">
            Registered: <?php echo countUsers(true, true, -1, "", "", "", "", ""); ?>
        </div>
        <div class="infodiv">
            Today active: <?php echo countUsers(true, null, 1, "", "", "", "", ""); ?>
        </div>
    </div>

    <div class="infocontainer">
        <div class="infodiv">
            W1: <?php echo countUsersByUnlock("{d34db335-0001-4000-7711-000000200001}"); ?>
        </div>
        <div class="infodiv">
            W2: <?php echo countUsersByUnlock("{d34db335-0001-4000-7711-000000200002}"); ?>
        </div>
        <div class="infodiv">
            W3: <?php echo countUsersByUnlock("{d34db335-0001-4000-7711-000000200003}"); ?>
        </div>
        <div class="infodiv">
            W4: <?php echo countUsersByUnlock("{d34db335-0001-4000-7711-000000200004}"); ?>
        </div>
        <div class="infodiv">
            MP: <?php echo countUsersByUnlock("{d34db335-0001-4000-7711-000000300001}"); ?>
        </div>
    </div>

    <div class="infocontainer">
        <div class="infodiv">
            First place: <?php echo countFirstPlaceUsers(); ?>
        </div>
        <div class="infodiv">
            Zero score: <?php echo countZeroScoreUsers(); ?>
        </div>
    </div>

	<?php

	$PARTITIONSIZE = 50;

	$groups = getScoreDistribution($PARTITIONSIZE);
	$cgroups = array_merge([], $groups);

	$sum = 0;
	for ($i=0; $i < count($cgroups); $i++)
	{
		$sum += $cgroups[$i]['count'];
		$cgroups[$i]['count'] = $sum;
	}

	?>

    <div data-collapse>

        <h2 class="collapseheader">Score Distribution</h2>
        <div>
        <div>
            <canvas id="scoreChart1" width="85%" height="25%"></canvas>
            <script>
                let ctx1 = document.getElementById("scoreChart1").getContext('2d');

                new Chart(ctx1,
                    {
                        type: 'line',
                        data:
                            {
                                labels: [ <?php foreach ($groups as $entry) echo "'".($entry['score']-$PARTITIONSIZE+1)." - ".$entry['score']."',"; ?> ],
                                datasets:
                                    [
                                        {
                                            label: 'count',
                                            data: [ <?php foreach ($groups as $entry) echo $entry['count'].","; ?> ],
                                        }
                                    ]
                            },
                        options:
                            {
                                animation:
                                    {
                                        duration: 0,
                                    },
                                elements:
                                    {
                                        line:
                                            {
                                                tension: 0 ,
                                            }
                                    },
                            }
                    });
            </script>
        </div>
        <div>
            <canvas id="scoreChart2" width="85%" height="25%"></canvas>
            <script>
                let ctx2 = document.getElementById("scoreChart2").getContext('2d');

                new Chart(ctx2,
                    {
                        type: 'line',
                        data:
                            {
                                labels: [ <?php foreach ($cgroups as $entry) echo $entry['score'].","; ?> ],
                                datasets:
                                    [
                                        {
                                            label: 'count',
                                            data: [ <?php foreach ($cgroups as $entry) echo $entry['count'].","; ?> ],
                                            pointRadius: 0,
                                        }
                                    ]
                            },
                        options:
                            {
                                animation:
                                    {
                                        duration: 0,
                                    },
                                elements:
                                    {
                                        line:
                                            {
                                                tension: 0 ,
                                            }
                                    },
                            }
                    });
            </script>
        </div>
        </div>
    </div>

    <div class="tablebox" data-collapse>

        <h2 class="open collapseheader">Userlist</h2>
        <div>
            <div class="filterlinkrow">
                <?php if (! $showall): ?>
                    <a href="<?php echo suffixGetParams('a', 'y'); ?>">[Show Zero Scored]</a>
                <?php endif; ?>

				<?php if ($anon !== TRUE): ?>
                    <a href="<?php echo suffixGetParams('anon', ''); ?>">[Show Registered]</a>
				<?php endif; ?>

				<?php if ($anon !== FALSE): ?>
                    <a href="<?php echo suffixGetParams('anon', ''); ?>">[Show Unregistered]</a>
				<?php endif; ?>

				<?php if ($anon !== NULL): ?>
                    <a href="<?php echo suffixGetParams('anon', ''); ?>">[Show all Account States]</a>
				<?php endif; ?>

                <?php if ($device != ''): ?>
                    <a href="<?php echo suffixGetParams('device', ''); ?>">[All Devices]</a>
                <?php endif; ?>

                <?php if ($os != ''): ?>
                    <a href="<?php echo suffixGetParams('device_version', ''); ?>">[All Device Versions]</a>
                <?php endif; ?>

                <?php if ($resolution != ''): ?>
                    <a href="<?php echo suffixGetParams('resolution', ''); ?>">[All Resolutions]</a>
                <?php endif; ?>

                <?php if ($appversion != ''): ?>
                    <a href="<?php echo suffixGetParams('app_version', ''); ?>">[All App Versions]</a>
                <?php endif; ?>

                <?php if ($apptype != ''): ?>
                    <a href="<?php echo suffixGetParams('app_type', ''); ?>">[All App Types]</a>
                <?php endif; ?>
            </div>
            <table class="sqltab pure-table pure-table-bordered sortable">
                <thead>
                    <tr>
                        <th style='width: 170px'>Username</th>
                        <th>Password</th>
                        <th>Anon</th>
                        <th>Score</th>
                        <th>Score(MP)</th>
                        <th>Rev</th>
                        <th style='width: 250px'>Devicename</th>
                        <th style='width: 300px'>Deviceversion</th>
                        <th>Resolution</th>
                        <th>Unlocks</th>
                        <th>Multiplayer</th>
                        <th style='width: 170px'>Last Online</th>
                        <th>Version</th>
                    </tr>
                </thead>
                <?php foreach ($users as $entry): ?>
                    <tr>
                        <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                        <?php expansioncell($entry['password_hash']); ?>
                        <td><?php echo $entry['is_auto_generated']; ?></td>
                        <?php expansioncell4($entry['score'], getScoreInfo($entry)) ?>
                        <td><?php echo $entry['mpscore']; ?></td>
                        <td><?php echo $entry['revision_id']; ?></td>
                        <td><a class="nolink" href="<?php echo suffixGetParams('device', $entry['device_name']); ?>"><?php echo $entry['device_name']; ?></a></td>
                        <td><a class="nolink" href="<?php echo suffixGetParams('device_version', $entry['device_version']); ?>"><?php echo $entry['device_version']; ?></a></td>
                        <td><a class="nolink" href="<?php echo suffixGetParams('resolution', $entry['device_resolution']); ?>"><?php echo $entry['device_resolution']; ?></a></td>
                        <?php expansioncell3($entry['unlocked_worlds'], lc($entry['unlocked_worlds'])); ?>
                        <td><?php echo strpos($entry['unlocked_worlds'], '{d34db335-0001-4000-7711-000000300001}') ? 'TRUE' : 'FALSE'; ?></td>
                        <td><?php echo $entry['last_online']; ?></td>
                        <td><a class="nolink" href="<?php echo suffixGetParams('app_version', $entry['app_version']); ?>"><?php echo $entry['app_version']; ?></a></td>
                    </tr>
                    <tr class='tab_prev' id='tr_prev_<?php echo $previd; ?>'><td colspan='12' id='td_prev_<?php echo $previd; ?>' style='text-align: left;' ></td></tr>
                    <?php $previd++; ?>
                <?php endforeach; ?>
            </table>
            <div class="pagination_row">
                <?php for ($i=0; $i < ceil($entrycount/500); $i++ ): ?>
                    <?php if ($i != $page): ?>
                        <a class="pagination_link" href="<?php echo suffixGetParams('page', $i); ?>"><?php echo ($i+1); ?></a>
                    <?php else: ?>
                        <a class="pagination_curr"><?php echo ($i+1); ?></a>
                    <?php endif; ?>
                <?php endfor; ?>
            </div>
        </div>
    </div>

    <?php printSQLStats(); ?>

    <script type="text/javascript">
		<?php echo file_get_contents('admin.js'); ?>
    </script>

    <script src="jquery.collapse.js"></script>
    <script src="toastr.min.js"></script>
    <script src="sorttable.js"></script>

</body>
</html>