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

	function getScoreInfo($entry)
    {
	    return
            "Score     = " . $entry['score']      . "\n" .
			"Score[W1] = " . $entry['score_w1']   . "\n" .
			"Score[W2] = " . $entry['score_w2']   . "\n" .
			"Score[W3] = " . $entry['score_w3']   . "\n" .
			"Score[W4] = " . $entry['score_w4']   . "\n" .
			"Time      = " . gmdate("H:i:s", (int)($entry['time_total'])) . " = " . $entry['time_total'] . "\n" .
			"Time[W1]  = " . gmdate("H:i:s", (int)($entry['time_w1']))    . " = " . $entry['time_w1']    . "\n" .
			"Time[W2]  = " . gmdate("H:i:s", (int)($entry['time_w2']))    . " = " . $entry['time_w2']    . "\n" .
			"Time[W3]  = " . gmdate("H:i:s", (int)($entry['time_w3']))    . " = " . $entry['time_w3']    . "\n" .
			"Time[W4]  = " . gmdate("H:i:s", (int)($entry['time_w4']))    . " = " . $entry['time_w4']    . "\n" .
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
	$statshist = sql_query_assoc("GetStatsHistory", "SELECT * FROM stats_history ORDER BY exectime ASC");

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
        <div class="infodiv">
            SCCM: <?php echo countUsersByUnlock("{d34db335-0001-4000-7711-000000300002}"); ?>
        </div>
    </div>

    <div class="infocontainer">
        <div class="infodiv">
            First place: <?php echo countFirstPlaceUsers(); ?>
        </div>
        <div class="infodiv">
            Zero score: <?php echo countZeroScoreUsers(); ?>
        </div>
        <div class="infodiv">
            Zombies: <?php echo countZombies(); ?>&nbsp;(<a href="deleteZombies.php">delete</a>)
        </div>
    </div>

    <div class="infocontainer">
        <div class="infodiv">
            New (today): <?php echo getNewUsersToday(); ?>
        </div>
        <div class="infodiv">
            Purchases: <?php echo getPuchaseTotal(); ?> (<?php echo getPuchaseDelta(); ?>)
        </div>
        <div class="infodiv">
            Unlocks: <?php echo getUnlockTotal(); ?> (<?php echo getUnlockDelta(); ?>)
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
                <div id="scoreChart1" style="width:95%; height:500px"></div>
                <script>
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
                </script>
            </div>
            <div>
                <div id="scoreChart2" style="width:95%; height:500px"></div>
                <script>
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
                </script>
            </div>
        </div>
    </div>

    <div class="graphbox" data-collapse>
        <h2 class="collapseheader">New Users / Time</h2>
        <div>
            <div>
                <div id="scoreChart3" style="width:95%; height:500px"></div>
                <script>
                    let chart3 = AmCharts.makeChart("scoreChart3", {
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
								echo "}\n";
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
                    chart3.addListener("init", initChart3);
                    function initChart3(){
                        var d1 = new Date();
                        d1.setMonth(d1.getMonth() - 3);
                        var d2 = new Date();
                        chart3.zoomToDates(d1, d2);
                    }
                </script>
            </div>
            <div>
                <div id="scoreChart4" style="width:95%; height:500px"></div>
                <script>
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
									echo "}\n";
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
                </script>
            </div>
        </div>
    </div>

    <div class="graphbox" data-collapse>
        <h2 class="collapseheader">Paying Users / Time</h2>
        <div>
            <div>
                <div id="scoreChart6" style="width:95%; height:500px"></div>
                <script>
                    AmCharts.makeChart("scoreChart6", {
                        "type": "serial",
                        "theme": "light",
                        "marginRight": 80,
                        "dataProvider":
                            [
								<?php
								for ($i=0; $i < count($statshist); $i++)
								{
									if ($i>0)echo ',';
									echo "{";
									echo " date: new Date('".$statshist[$i]['exectime'].          "'), ";
									echo " count_1: "       .$statshist[$i]['user_amazon'].       ", ";
									echo " count_2: "       .$statshist[$i]['user_android_full']. ", ";
									echo " count_3: "       .$statshist[$i]['user_ios'].          ", ";
									echo " count_4: "       .$statshist[$i]['user_winphone'].     "";
									echo "}\n";
								}
								?>
                            ],
                        "valueAxes": [{
                            "position": "left",
                            "title": "Users"
                        }],
                        "graphs":
                            [
                                {
                                    "id": "g1",
                                    "fillAlphas": 0,
                                    "valueField": "count_1",
                                    "bullet": "round",
                                    "bulletBorderAlpha": 1,
                                    "bulletBorderThickness": 1,
                                    "balloonText": "<div style='margin:5px; font-size:19px;'>Amazon:<b>[[value]]</b></div>"
                                },
                                {
                                    "id": "g2",
                                    "fillAlphas": 0,
                                    "valueField": "count_2",
                                    "bullet": "round",
                                    "bulletBorderAlpha": 1,
                                    "bulletBorderThickness": 1,
                                    "balloonText": "<div style='margin:5px; font-size:19px;'>Android Full:<b>[[value]]</b></div>"
                                },
                                {
                                    "id": "g3",
                                    "fillAlphas": 0,
                                    "valueField": "count_3",
                                    "bullet": "round",
                                    "bulletBorderAlpha": 1,
                                    "bulletBorderThickness": 1,
                                    "balloonText": "<div style='margin:5px; font-size:19px;'>iOS:<b>[[value]]</b></div>"
                                },
                                {
                                    "id": "g4",
                                    "fillAlphas": 0,
                                    "valueField": "count_4",
                                    "bullet": "round",
                                    "bulletBorderAlpha": 1,
                                    "bulletBorderThickness": 1,
                                    "balloonText": "<div style='margin:5px; font-size:19px;'>Win Phone:<b>[[value]]</b></div>"
                                },
                            ],
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
                </script>
            </div>
        </div>
    </div>

    <div class="graphbox" data-collapse>
        <h2 class="collapseheader">Unlocks / Time</h2>
        <div>
            <div>
                <div id="scoreChart7" style="width:95%; height:500px"></div>
                <script>
                    AmCharts.makeChart("scoreChart7", {
                        "type": "serial",
                        "theme": "light",
                        "marginRight": 80,
                        "dataProvider":
                            [
								<?php
								for ($i=0; $i < count($statshist); $i++)
								{
									if ($i>0)echo ',';
									echo "{";
									echo " date: new Date('".$statshist[$i]['exectime'].   "'), ";
									echo " count_1: "       .$statshist[$i]['unlocks_w1']. ", ";
									echo " count_2: "       .$statshist[$i]['unlocks_w2']. ", ";
									echo " count_3: "       .$statshist[$i]['unlocks_w3']. ", ";
									echo " count_4: "       .$statshist[$i]['unlocks_w4']. ", ";
									echo " count_4: "       .$statshist[$i]['unlocks_mp']. ", ";
									echo " count_6: "       .$statshist[$i]['unlocks_sccm']. "";
									echo "}\n";
								}
								?>
                            ],
                        "valueAxes": [{
                            "position": "left",
                            "title": "Users"
                        }],
                        "graphs":
                            [
                                {
                                    "id": "g1",
                                    "fillAlphas": 0,
                                    "valueField": "count_1",
                                    "bullet": "square",
                                    "bulletBorderAlpha": 1,
                                    "bulletBorderThickness": 1,
                                    "balloonText": "<div style='margin:5px; font-size:19px;'>Unlocks W1:<b>[[value]]</b></div>"
                                },
                                {
                                    "id": "g2",
                                    "fillAlphas": 0,
                                    "valueField": "count_2",
                                    "bullet": "square",
                                    "bulletBorderAlpha": 1,
                                    "bulletBorderThickness": 1,
                                    "balloonText": "<div style='margin:5px; font-size:19px;'>Unlocks W2:<b>[[value]]</b></div>"
                                },
                                {
                                    "id": "g3",
                                    "fillAlphas": 0,
                                    "valueField": "count_3",
                                    "bullet": "square",
                                    "bulletBorderAlpha": 1,
                                    "bulletBorderThickness": 1,
                                    "balloonText": "<div style='margin:5px; font-size:19px;'>Unlocks W3:<b>[[value]]</b></div>"
                                },
                                {
                                    "id": "g4",
                                    "fillAlphas": 0,
                                    "valueField": "count_4",
                                    "bullet": "square",
                                    "bulletBorderAlpha": 1,
                                    "bulletBorderThickness": 1,
                                    "balloonText": "<div style='margin:5px; font-size:19px;'>Unlocks W4:<b>[[value]]</b></div>"
                                },
                                {
                                    "id": "g5",
                                    "fillAlphas": 0,
                                    "valueField": "count_5",
                                    "bullet": "square",
                                    "bulletBorderAlpha": 1,
                                    "bulletBorderThickness": 1,
                                    "balloonText": "<div style='margin:5px; font-size:19px;'>Unlocks MP:<b>[[value]]</b></div>"
                                },
                                {
                                    "id": "g6",
                                    "fillAlphas": 0,
                                    "valueField": "count_5",
                                    "bullet": "square",
                                    "bulletBorderAlpha": 1,
                                    "bulletBorderThickness": 1,
                                    "balloonText": "<div style='margin:5px; font-size:19px;'>Unlocks SCCM:<b>[[value]]</b></div>"
                                },
                            ],
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
                </script>
            </div>
        </div>
    </div>

    <div class="graphbox" data-collapse>
        <h2 class="collapseheader">Active Users / Time</h2>
        <div>
            <div>
                <div id="scoreChart5" style="width:95%; height:500px"></div>
                <script>
                    AmCharts.makeChart("scoreChart5", {
                        "type": "serial",
                        "theme": "light",
                        "marginRight": 80,
                        "dataProvider":
                            [
								<?php
								for ($i=0; $i < count($statshist); $i++)
								{
									if ($i>0)echo ',';
									echo "{";
									echo " date: new Date('".$statshist[$i]['exectime']."'), ";
									echo " count: ".$statshist[$i]['active_users_per_day']."";
									echo "}";
								}
								?>
                            ],
                        "valueAxes": [{
                            "position": "left",
                            "title": "Users"
                        }],
                        "graphs": [{
                            "id": "g1",
                            "fillAlphas": 0.4,
                            "valueField": "count",
                            "bullet": "round",
                            "bulletBorderAlpha": 1,
                            "bulletBorderThickness": 1,
                            "balloonText": "<div style='margin:5px; font-size:19px;'>Value:<b>[[value]]</b></div>"
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
                </script>
            </div>
        </div>
    </div>

    <div class="graphbox" data-collapse>
        <h2 class="collapseheader">Versions / Time</h2>
        <div>
            <div>
                <div id="scoreChart8" style="width:95%; height:500px"></div>
                <script>
                    AmCharts.makeChart("scoreChart8", {
                        "type": "serial",
                        "theme": "light",
                        "marginRight": 80,
                        "dataProvider":
                            [
								<?php
								for ($i=0; $i < count($statshist); $i++)
								{
								    $usum = $statshist[$i]['user_current_version'] + $statshist[$i]['user_old_version'];

									$pnew = $usum==0 ? 0 : ($statshist[$i]['user_current_version'] * 100.0 /  $usum);
									$pold = $usum==0 ? 0 : ($statshist[$i]['user_old_version']     * 100.0 /  $usum);

									if ($i>0)echo ',';
									echo "{";
									echo " date: new Date('".$statshist[$i]['exectime']."'), ";
									echo " count_cur: ".$pnew.",";
									echo " count_old: ".$pold."";
									echo "}\n";
								}
								?>
                            ],
                        "valueAxes": [{
                            "position": "left",
                            "title": "Users (%)"
                        }],
                        "graphs": [{
                            "id": "g1",
                            "fillAlphas": 0.4,
                            "lineThickness": 2,
                            "valueField": "count_cur",
                            "bullet": "round",
                            "bulletBorderAlpha": 1,
                            "bulletBorderThickness": 1,
                            "balloonText": "<div style='margin:5px; font-size:19px;'>Current version:<b>[[value]]</b>%</div>"
                        }],
                        "valueScrollbar": {
                            "autoGridCount": true,
                            "color": "#000000",
                            "scrollbarHeight": 50
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
                        <th>Online</th>
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
                        <td><?php echo strpos($entry['unlocked_worlds'], '{d34db335-0001-4000-7711-000000300002}') ? 'TRUE' : 'FALSE'; ?></td>
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