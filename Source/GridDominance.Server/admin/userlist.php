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
	if (! empty($_GET['a'])) $showall = $_GET['a'] == 'y';

    $showallparam = ($showall ? 'y' : 'n');

    $showregistered = 0;
    if (!empty($_GET['showregistered'])) $showregistered = $_GET['showregistered'];

    $page = 0;
    if (!empty($_GET['page'])) $page = $_GET['page'];

    $days = -1;
	if (!empty($_GET['d'])) $days= $_GET['d'];

	$users = getUsers($showall, $showregistered, $days, $page, 500);
	$entrycount = countUsers($showall, $showregistered, $days);

    ?>

    <div class="infocontainer">
        <div class="infodiv">
            All: <?php echo countUsers(true, false, -1); ?>
        </div>
        <div class="infodiv">
            With score: <?php echo countUsers(false, false, -1); ?>
        </div>
        <div class="infodiv">
            Registered: <?php echo countUsers(true, true, -1); ?>
        </div>
        <div class="infodiv">
            Today active: <?php echo countUsers(true, false, 1); ?>
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

    <div data-collapse>

        <h2 class="open collapseheader">Score Distribution</h2>
        <div>
            <canvas id="scoreChart" width="85%" height="25%"></canvas>
            <script>
                let ctx = document.getElementById("scoreChart").getContext('2d');

				<?php $groups = getScoreDistribution(); ?>

                new Chart(ctx,
                    {
                        type: 'line',
                        data:
                            {
                                labels: [ <?php foreach ($groups as $entry) echo $entry['score'].","; ?> ],
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
    </div>

    <div class="tablebox" data-collapse>

        <h2 class="open collapseheader">Userlist</h2>
        <div>
            <div class="filterlinkrow">
                <?php if (! $showall): ?>
                    <a href="<?php echo suffixGetParams('a', 'y'); ?>">[Show All]</a>
                <?php endif; ?>

                <?php if (! $showregistered): ?>
                    <a href="<?php echo suffixGetParams('showregistered', '1'); ?>">[Show Registered]</a>
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
                        <td><?php echo $entry['device_name']; ?></td>
                        <td><?php echo $entry['device_version']; ?></td>
                        <td><?php echo $entry['device_resolution']; ?></td>
                        <?php expansioncell3($entry['unlocked_worlds'], lc($entry['unlocked_worlds'])); ?>
                        <td><?php echo strpos($entry['unlocked_worlds'], '{d34db335-0001-4000-7711-000000300001}') ? 'TRUE' : 'FALSE'; ?></td>
                        <td><?php echo $entry['last_online']; ?></td>
                        <td><?php echo $entry['app_version']; ?></td>
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