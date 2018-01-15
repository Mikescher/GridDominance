<?php require_once '../internals/backend.php'; ?>
<?php require_once '../internals/utils.php'; ?>
<?php require_once 'common/libadmin.php'; ?>
<?php init("admin"); ?>
<?php $INTERACTIVE = true; ?>
<!doctype html>

<html lang="en">
<head>
	<meta charset="utf-8">
	<?php includeStyles(); ?>

    <link rel="stylesheet" href="/admin/common/export.css" type="text/css" media="all" />

</head>

<body id="rootbox">

    <?php includeScripts(); ?>
    <script src="/admin/common/amcharts.js"></script>
    <script src="/admin/common/serial.js"></script>
    <script src="/admin/common/export.min.js"></script>
    <script src="/admin/common/light.js"></script>

    <h1><a href="index.php">Cannon Conquest | Admin Page</a></h1>

    <?php

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


	$agroups = array_merge([], getScoreDistribution(1));
	$sum = 0;
	for ($i=0; $i < count($agroups); $i++)
	{
		$sum += $agroups[$i]['count'];
		$agroups[$i]['count'] = $sum;
	}

    $userdist = getNewUsersDistribution();
    $cuserdist = array_merge([], $userdist);
	$sum = 0;
	for ($i=0; $i < count($cuserdist); $i++)
	{
		$sum += $cuserdist[$i]['count'];
		$cuserdist[$i]['count'] = $sum;
	}
    $udates = [];
    for ($i=0; $i < count($userdist); $i++) $udates []= $userdist[$i]['date'];

	?>

    <div class="graphbox" data-collapse>

        <h2 class="collapseheader">Score Distribution</h2>
        <div>
            <div>
				<?php if (!$INTERACTIVE): ?>
                    <canvas id="scoreChart1" width="85%" height="25%"></canvas>
				<?php else: ?>
                    <div id="scoreChart1" style="width:95%; height:500px"></div>
				<?php endif; ?>
                <script>
					<?php if (!$INTERACTIVE): ?>
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
                            options: { animation: { duration: 0, }, elements:  { line: { tension: 0, } }, }
                        });
					<?php else: ?>
                    AmCharts.makeChart("scoreChart1", {
                        "type": "serial",
                        "theme": "light",
                        "marginRight": 80,
                        "dataProvider":
                            [
								<?php
								for ($i=0; $i < count($groups); $i++)
								{
									if ($i>0)echo ',';
									echo "{";
									echo " score: '".($groups[$i]['score']-$PARTITIONSIZE+1)." - ".$groups[$i]['score']."', ";
									echo " count: ".$groups[$i]['count']."";
									echo "}\n";
								}
								?>
                            ],
                        "valueAxes": [ {
                            "gridColor": "#FFFFFF",
                            "gridAlpha": 0.2,
                            "dashLength": 0
                        } ],
                        "gridAboveGraphs": true,
                        "startDuration": 1,
                        "graphs": [ {
                            "balloonText": "[[category]]: <b>[[value]]</b>",
                            "fillAlphas": 0.8,
                            "lineAlpha": 0.2,
                            "type": "column",
                            "valueField": "count"
                        } ],
                        "chartCursor": {
                            "categoryBalloonEnabled": false,
                            "cursorAlpha": 0,
                            "zoomable": true
                        },
                        "categoryField": "score",
                        "categoryAxis": {
                            "gridPosition": "start",
                            "gridAlpha": 0,
                            "tickPosition": "start",
                            "tickLength": 20
                        },
                        "export": {
                            "enabled": true
                        }
                    });
					<?php endif; ?>
                </script>
            </div>
            <div>
				<?php if (!$INTERACTIVE): ?>
                    <canvas id="scoreChart2" width="85%" height="25%"></canvas>
				<?php else: ?>
                    <div id="scoreChart2" style="width:95%; height:500px"></div>
				<?php endif; ?>
                <script>
					<?php if (!$INTERACTIVE): ?>
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
					<?php else: ?>
                    AmCharts.makeChart("scoreChart2", {
                        "type": "serial",
                        "theme": "light",
                        "marginRight": 80,
                        "dataProvider":
                            [
								<?php
								for ($i=0; $i < count($agroups); $i++)
								{
									if ($i>0)echo ',';
									echo "{";
									echo " score: '".$agroups[$i]['score']."', ";
									echo " count: ".$agroups[$i]['count']."";
									echo "}";
								}
								?>
                            ],

                        "valueAxes": [{
                            "position": "left",
                            "title": "Score"
                        }],
                        "graphs": [{
                            "id": "g22",
                            "fillAlphas": 0.4,
                            "valueField": "count",
                            "balloonText": "<div style='margin:5px; font-size:19px;'>Score:<b>[[value]]</b></div>"
                        }],
                        "chartScrollbar": {
                            "graph": "g22",
                            "scrollbarHeight": 80,
                            "backgroundAlpha": 0,
                            "selectedBackgroundAlpha": 0.1,
                            "selectedBackgroundColor": "#888888",
                            "graphFillAlpha": 0,
                            "graphLineAlpha": 0.5,
                            "selectedGraphFillAlpha": 0,
                            "selectedGraphLineAlpha": 1,
                            "autoGridCount": true,
                            "color": "#AAAAAA"
                        },
                        "chartCursor": {
                            "categoryBalloonDateFormat": "JJ:NN, DD MMMM",
                            "cursorPosition": "mouse",
                            "valueZoomable": true
                        },
                        "valueScrollbar": {
                            "autoGridCount": true,
                            "color": "#000000",
                            "scrollbarHeight": 50
                        },
                        "categoryField": "score",
                        "categoryAxis": {
                            "minPeriod": "mm",
                            "parseDates": false
                        },
                        "export": {
                            "enabled": true,
                            "dateFormat": "YYYY-MM-DD HH:NN:SS"
                        }
                    });
					<?php endif; ?>
                </script>
            </div>
        </div>
    </div>

    <div class="graphbox" data-collapse>
        <h2 class="collapseheader">New Users / Time</h2>
        <div>
            <div>
				<?php if (!$INTERACTIVE): ?>
                    <canvas id="scoreChart3" width="85%" height="25%"></canvas>
				<?php else: ?>
                    <div id="scoreChart3" style="width:95%; height:500px"></div>
				<?php endif; ?>
                <script>
					<?php if (!$INTERACTIVE): ?>
                    let ctx3 = document.getElementById("scoreChart3").getContext('2d');
                    new Chart(ctx3,
                        {
                            type: 'line',
                            data:
                                {
                                    labels: [ <?php foreach ($udates as $rld) echo "'".$rld."',"; ?> ],
                                    datasets:
                                        [
                                            {
                                                label: 'Accounts',
                                                data: [ <?php foreach ($userdist as $dd) echo $dd['count'].","; ?> ],
                                            },
                                        ]
                                },
                        });
					<?php else: ?>
                    AmCharts.makeChart("scoreChart3", {
                        "type": "serial",
                        "theme": "light",
                        "marginRight": 80,
                        "dataProvider":
                        [
							<?php
                            for ($i=0; $i < count($udates); $i++)
                            {
                                if ($i>0)echo ',';
								echo "{";
								echo " date: new Date('".$udates[$i]."'), ";
								echo " count: ".$userdist[$i]['count']."";
								echo "}";
							}
                            ?>
                        ],
                        "valueAxes": [{
                            "position": "left",
                            "title": "New users"
                        }],
                        "graphs": [{
                            "id": "g1",
                            "fillAlphas": 0.4,
                            "valueField": "count",
                            "balloonText": "<div style='margin:5px; font-size:19px;'>New users:<b>[[value]]</b></div>"
                        }],
                        "chartScrollbar": {
                            "graph": "g1",
                            "scrollbarHeight": 80,
                            "backgroundAlpha": 0,
                            "selectedBackgroundAlpha": 0.1,
                            "selectedBackgroundColor": "#888888",
                            "graphLineAlpha": 0.5,
                            "selectedGraphFillAlpha": 0,
                            "selectedGraphLineAlpha": 1,
                            "autoGridCount": true,
                            "color": "#AAAAAA"
                        },
                        "chartCursor": {
                            "categoryBalloonDateFormat": "JJ:NN, DD MMMM",
                            "cursorPosition": "mouse"
                        },
                        "categoryField": "date",
                        "categoryAxis": {
                            "minPeriod": "mm",
                            "parseDates": true
                        },
                        "export": {
                            "enabled": true,
                            "dateFormat": "YYYY-MM-DD HH:NN:SS"
                        }
                    });
					<?php endif; ?>
                </script>
            </div>
            <div>
				<?php if (!$INTERACTIVE): ?>
                    <canvas id="scoreChart4" width="85%" height="25%"></canvas>
				<?php else: ?>
                    <div id="scoreChart4" style="width:95%; height:500px"></div>
                <?php endif; ?>
                <script>
					<?php if (!$INTERACTIVE): ?>
                    let ctx4 = document.getElementById("scoreChart4").getContext('2d');
                    new Chart(ctx4,
                        {
                            type: 'line',
                            data:
                                {
                                    labels: [ <?php foreach ($udates as $rld) echo "'".$rld."',"; ?> ],
                                    datasets:
                                        [
                                            {
                                                label: 'Accounts',
                                                data: [ <?php foreach ($cuserdist as $dd) echo $dd['count'].","; ?> ],
                                            },
                                        ]
                                },
                        });
					<?php else: ?>
                    AmCharts.makeChart("scoreChart4", {
                        "type": "serial",
                        "theme": "light",
                        "marginRight": 80,
                        "dataProvider":
                            [
								<?php
								for ($i=0; $i < count($udates); $i++)
								{
									if ($i>0)echo ',';
									echo "{";
									echo " date: new Date('".$udates[$i]."'), ";
									echo " count: ".$cuserdist[$i]['count']."";
									echo "}";
								}
								?>
                            ],
                        "valueAxes": [{
                            "position": "left",
                            "title": "New users"
                        }],
                        "graphs": [{
                            "id": "g2",
                            "fillAlphas": 0.4,
                            "valueField": "count",
                            "balloonText": "<div style='margin:5px; font-size:19px;'>New users:<b>[[value]]</b></div>"
                        }],
                        "chartScrollbar": {
                            "graph": "g2",
                            "scrollbarHeight": 80,
                            "backgroundAlpha": 0,
                            "selectedBackgroundAlpha": 0.1,
                            "selectedBackgroundColor": "#888888",
                            "graphLineAlpha": 0.5,
                            "selectedGraphFillAlpha": 0,
                            "selectedGraphLineAlpha": 1,
                            "autoGridCount": true,
                            "color": "#AAAAAA"
                        },
                        "chartCursor": {
                            "categoryBalloonDateFormat": "JJ:NN, DD MMMM",
                            "cursorPosition": "mouse",
                            "valueZoomable": true
                        },
                        "valueScrollbar": {
                            "autoGridCount": true,
                            "color": "#000000",
                            "scrollbarHeight": 50
                        },
                        "categoryField": "date",
                        "categoryAxis": {
                            "minPeriod": "mm",
                            "parseDates": true
                        },
                        "export": {
                            "enabled": true,
                            "dateFormat": "YYYY-MM-DD HH:NN:SS"
                        }
                    });
					<?php endif; ?>
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

</body>
</html>