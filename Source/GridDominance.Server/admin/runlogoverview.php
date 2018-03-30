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

	<div class="infocontainer">
		<div class="infodiv">
            Count(history):&nbsp;<?php echo getRunLogCountHistory(); ?>
		</div>
		<div class="infodiv">
            Count(volatile):&nbsp;<?php echo getRunLogCountVolatile(); ?>
		</div>
	</div>

    <?php

    $COLORS =
        [
            '#4661EE','#EC5657','#1BCDD1','#8FAABB','#B08BEB','#3EA0DD','#F5A52A','#23BFAA','#FAA586','#EB8CC6',
            '#9BC4E5','#04640D','#FEFB0A','#FB5514','#E115C0','#00587F','#0BC582','#FEB8C8','#9E8317',
            '#01190F','#847D81','#58018B','#B70639','#703B01','#F7F1DF','#118B8A','#4AFEFA','#FCB164','#796EE6',
            '#000D2C','#53495F','#F95475','#61FC03','#5D9608','#DE98FD','#98A088','#4F584E','#248AD0','#5C5300',
            '#9F6551','#BCFEC6','#932C70','#2B1B04','#B5AFC4','#D4C67A','#AE7AA1','#C2A393','#0232FD','#6A3A35',
            '#BA6801','#168E5C','#16C0D0','#C62100','#014347','#233809','#42083B','#82785D','#023087','#B7DAD2',
            '#196956','#8C41BB','#ECEDFE','#2B2D32','#94C661','#F8907D','#895E6B','#788E95'
        ];

    $DISABLED = ['cron', 'admin', 'change-password', 'log-client', 'merge-login', 'savesessionstate', 'get-ranking', 'upgrade-user', 'set-multiscore', 'merge-login-2' ];

    ?>

    <?php

	function fmtd($df)
    {
		$a = explode(' ', $df);
		$b = explode(':', $a[1]);

		$h = 6*($b[0]/6);
		//if ($h != 12 && $h != 0) return NULL;

		return $a[0] . ' ' . str_pad($h, 2, '0', STR_PAD_LEFT) . ':' . '00';
    }

    function getDates(&$entries)
    {
        $dates = [];
		foreach ($entries as &$entry)
		{
		    $d = fmtd($entry['exectime']);

		    //if ($d===NULL) { continue; }

		    $dates []= $d;
			$entry['execdate'] = $d;
		}
        asort($dates);
		$dates = array_unique($dates);
		$dates = array_values($dates);
        return $dates;
    }

	function flatten(array $array) {
		$return = array();
		array_walk_recursive($array, function($a) use (&$return) { $return[] = $a; });
		return $return;
	}

    ?>

    <?php

	$actionlist = flatten(getRunLogActionList());
	$hostoryentries = getAllRunLogs();

	$dates = getDates($hostoryentries);

	$data = [];

	foreach ($dates as $d) $data[$d] = [];

	foreach ($hostoryentries as $e) if (array_key_exists($e['execdate'], $data)) $data[$e['execdate']][$e['action']] = $e;

    ?>

    <div class="graphbox" data-collapse>
        <h2 class="open collapseheader">History (Median)</h2>
        <div>
            <div id="scoreChart1" style="width:calc(100% - 8px); height:850px"></div>
            <script>
                let chart1 = AmCharts.makeChart("scoreChart1", {
                    "type": "serial",
                    "theme": "light",
                    "marginRight": 80,
                    "legend": {
                        "equalWidths": false,
                        "periodValueText": "Median: [[value.sum]]s",
                        "position": "top",
                        "valueAlign": "left",
                        "valueWidth": 100,
                        "listeners": [ {
                            "event": "hideItem",
                            "method": legendHandler1
                        }, {
                            "event": "showItem",
                            "method": legendHandler1
                        } ]
                    },
                    "dataProvider":
                        [
							<?php

                            $lastval = [];

                            $first=true;
							foreach ($data as $edate => $d)
                            {
								if (!$first) echo ','; $first=false;

								echo "{";
								echo " date: new Date('$edate')";

								$j=0;
								foreach ($actionlist as $actionname)
								{
									$j++;
									if (array_key_exists($actionname, $d))
									    echo ", count_".$j.": ". ($lastval[$actionname] = $d[$actionname]['duration_median']/(1000.0*1000.0));
									else if (array_key_exists($actionname, $lastval))
										echo ", count_".$j.": ".$lastval[$actionname];
									else
										echo ", count_".$j.": "."0";
								}
								echo "}\n";
                            }

							?>
                        ],
                    "valueAxes": [{
                        "position": "left",
                        "title": "seconds"
                    }],
                    "graphs":
                        [
							<?php $i=0; foreach ($actionlist as $raction): $i++; ?>
							<?php if ($i>1) echo ','; ?>
                            {
                                "id": "g<?php echo $i; ?>",
                                "fillAlphas": 0,
                                "title": "<?php echo $raction; ?>",
                                "hidden": <?php echo (in_array($raction, $DISABLED)) ? 'true' : 'false'; ?>,
                                "valueField": "count_<?php echo $i; ?>",
                                "bullet": "round",
                                "lineColor": "<?php echo $COLORS[$i%count($COLORS)]; ?>",
                                "bulletBorderAlpha": 0.2,
                                "bulletAlpha": 0.5,
                                "bulletSize": 4,
                                "bulletBorderThickness": 1,
                                "lineThickness": 2,
                                "type": "line",
                                "balloonText": "<?php echo $raction; ?>: <b>[[value]]s</b>",
                                "showBalloon": false
                            }
                            <?php endforeach; ?>
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
                chart1.addListener("init", initChart1);
                function initChart1(){
                    var d1 = new Date();
                    d1.setMonth(d1.getMonth() - 1);
                    var d2 = new Date();
                    d2.setMonth(d1.getMonth() + 1);
                    chart1.zoomToDates(d1, d2);
                }

                function legendHandler1( evt ) {
                    if ( evt.dataItem.id == "all" ) {
                        for ( var i1 in evt.chart.graphs ) {
                            if ( evt.chart.graphs[ i1 ].id != "all" ) {
                                evt.chart[ evt.dataItem.hidden ? "hideGraph" : "showGraph" ]( evt.chart.graphs[ i1 ] );
                            }
                        }
                    }
                }

            </script>
        </div>
    </div>

    <div class="graphbox" data-collapse>
        <h2 class="collapseheader">History (Average)</h2>
        <div>
            <div id="scoreChart3" style="width:calc(100% - 8px); height:650px"></div>
            <script>
                let chart3 = AmCharts.makeChart("scoreChart3", {
                    "type": "serial",
                    "theme": "light",
                    "marginRight": 80,
                    "legend": {
                        "equalWidths": false,
                        "periodValueText": "Average: [[value.sum]]s",
                        "position": "top",
                        "valueAlign": "left",
                        "valueWidth": 100,
                        "listeners": [ {
                            "event": "hideItem",
                            "method": legendHandler1
                        }, {
                            "event": "showItem",
                            "method": legendHandler1
                        } ]
                    },
                    "dataProvider":
                        [
							<?php

							$lastval = [];

							$first=true;
							foreach ($data as $edate => $d)
							{
								if (!$first) echo ','; $first=false;

								echo "{";
								echo " date: new Date('$edate')";

								$j=0;
								foreach ($actionlist as $actionname)
								{
									$j++;
									if (array_key_exists($actionname, $d))
										echo ", count_".$j.": ". ($lastval[$actionname] = $d[$actionname]['duration_avg']/(1000.0*1000.0));
									else if (array_key_exists($actionname, $lastval))
										echo ", count_".$j.": ".$lastval[$actionname];
									else
										echo ", count_".$j.": "."0";
								}
								echo "}\n";
							}

							?>
                        ],
                    "valueAxes": [{
                        "position": "left",
                        "title": "seconds"
                    }],
                    "graphs":
                        [
							<?php $i=0; foreach ($actionlist as $raction): $i++; ?>
							<?php if ($i>1) echo ','; ?>
                            {
                                "id": "g<?php echo $i; ?>",
                                "fillAlphas": 0,
                                "title": "<?php echo $raction; ?>",
                                "hidden": <?php echo (in_array($raction, $DISABLED)) ? 'true' : 'false'; ?>,
                                "valueField": "count_<?php echo $i; ?>",
                                "bullet": "round",
                                "lineColor": "<?php echo $COLORS[$i%count($COLORS)]; ?>",
                                "bulletBorderAlpha": 0.2,
                                "bulletAlpha": 0.5,
                                "bulletSize": 4,
                                "bulletBorderThickness": 1,
                                "lineThickness": 2,
                                "type": "line",
                                "balloonText": "<?php echo $raction; ?>: <b>[[value]]s</b>",
                                "showBalloon": false
                            }
							<?php endforeach; ?>
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
                chart3.addListener("init", initChart3);
                function initChart3(){
                    var d1 = new Date();
                    d1.setMonth(d1.getMonth() - 1);
                    var d2 = new Date();
                    d2.setMonth(d1.getMonth() + 1);
                    chart3.zoomToDates(d1, d2);
                }

            </script>
        </div>
    </div>

    <div class="graphbox" data-collapse>
        <h2 class="collapseheader">History (Cron)</h2>
        <div>
            <div id="scoreChart2" style="width:calc(100% - 8px); height:850px"></div>
            <script>
                let chart2 = AmCharts.makeChart("scoreChart2", {
                    "type": "serial",
                    "theme": "light",
                    "marginRight": 80,
                    "legend": {
                        "equalWidths": false,
                        "periodValueText": "[[value.sum]]s",
                        "position": "top",
                        "valueAlign": "left",
                        "valueWidth": 100,
                        "listeners": [ {
                            "event": "hideItem",
                            "method": legendHandler1
                        }, {
                            "event": "showItem",
                            "method": legendHandler1
                        } ]
                    },
                    "dataProvider":
                        [
							<?php

							$lastval = [];

							$first=true;
							foreach ($data as $edate => $d)
							{
								if (!$first) echo ','; $first=false;

								echo "{";
								echo " date: new Date('$edate')";

								$j=0;
								if (array_key_exists('cron', $d))
									echo ", count: ". ($lastval['cron'] = $d['cron']['duration']/(1000.0*1000.0*60.0));
								else if (array_key_exists('cron', $lastval))
									echo ", count: ".$lastval['cron'];
								else
									echo ", count: "."0";
								echo "}\n";
							}

							?>
                        ],
                    "valueAxes": [{
                        "position": "left",
                        "title": "minutes"
                    }],
                    "graphs":
                        [
                            {
                                "id": "g0",
                                "fillAlphas": 0.4,
                                "title": "Cron.",
                                "hidden": false,
                                "valueField": "count",
                                "bullet": "round",
                                "lineColor": "#888",
                                "bulletBorderAlpha": 0.2,
                                "bulletAlpha": 0.5,
                                "bulletSize": 4,
                                "bulletBorderThickness": 1,
                                "lineThickness": 2,
                                "type": "smoothedLine",
                                "balloonText": "Time (cron): <b>[[value]]min</b>",
                                "showBalloon": true
                            }
                        ],
                    "chartScrollbar": {
                        "graph": "g0",
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
                chart2.addListener("init", initChart2);
                function initChart2(){
                    var d1 = new Date();
                    d1.setMonth(d1.getMonth() - 1);
                    var d2 = new Date();
                    d2.setMonth(d1.getMonth() + 1);
                    chart2.zoomToDates(d1, d2);
                }

            </script>
        </div>
    </div>

    <div class="graphbox" data-collapse>
        <h2 class="collapseheader">Requests/Time</h2>
        <div>
            <div id="scoreChart4" style="width:calc(100% - 8px); height:650px"></div>
            <script>
                let chart4 = AmCharts.makeChart("scoreChart4", {
                    "type": "serial",
                    "theme": "light",
                    "marginRight": 80,
                    "legend": {
                        "equalWidths": false,
                        "periodValueText": "Count: [[value.sum]]",
                        "position": "top",
                        "valueAlign": "left",
                        "valueWidth": 100,
                        "listeners": [ {
                            "event": "hideItem",
                            "method": legendHandler1
                        }, {
                            "event": "showItem",
                            "method": legendHandler1
                        } ]
                    },
                    "dataProvider":
                        [
							<?php

							$first=true;
							foreach ($data as $edate => $d)
							{
								if (!$first) echo ','; $first=false;

								echo "{";
								echo " date: new Date('$edate')";

								$j=0;
								foreach ($actionlist as $actionname)
								{
									$j++;
									if (array_key_exists($actionname, $d))
										echo ", count_".$j.": ". ($lastval[$actionname] = $d[$actionname]['count']);
									else
										echo ", count_".$j.": "."0";
								}
								echo "}\n";
							}

							?>
                        ],
                    "valueAxes": [{
                        "position": "left",
                        "title": "request count"
                    }],
                    "graphs":
                        [
							<?php $i=0; foreach ($actionlist as $raction): $i++; ?>
							<?php if ($i>1) echo ','; ?>
                            {
                                "id": "g<?php echo $i; ?>",
                                "fillAlphas": 0,
                                "title": "<?php echo $raction; ?>",
                                "hidden": false,
                                "valueField": "count_<?php echo $i; ?>",
                                "bullet": "round",
                                "lineColor": "<?php echo $COLORS[$i%count($COLORS)]; ?>",
                                "bulletBorderAlpha": 0.2,
                                "bulletAlpha": 0.5,
                                "bulletSize": 4,
                                "bulletBorderThickness": 1,
                                "lineThickness": 2,
                                "type": "line",
                                "balloonText": "<?php echo $raction; ?>: <b>[[value]]</b>",
                                "showBalloon": false
                            }
							<?php endforeach; ?>
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
                    "valueScrollbar": {
                        "autoGridCount": true,
                        "color": "#000000",
                        "scrollbarHeight": 50
                    },
                    "chartCursor": {
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
                chart4.addListener("init", initChart4);
                function initChart4(){
                    var d1 = new Date();
                    d1.setMonth(d1.getMonth() - 1);
                    var d2 = new Date();
                    d2.setMonth(d1.getMonth() + 1);
                    chart4.zoomToDates(d1, d2);
                }

            </script>
        </div>
    </div>

    <div class="samsupertabcontainer">
        <div class="samtabheaderbox">

            <?php foreach($actionlist as $raction): ?>
            <div class="samtabheader" data-tabheader data-tabid="<?php echo $raction; ?>" data-tabcontainerid="c1"><?php echo $raction; ?></div>
            <?php endforeach; ?>
        </div>

        <?php foreach($actionlist as $raction): ?>

        <div class="samtabbox" data-tabcontent data-tabid="<?php echo $raction; ?>" data-tabcontainerid="c1">
            <table class="sqltab pure-table pure-table-bordered sortable">
                <thead>
                <tr>
                    <th width="150px">Time (Cron)</th>
                    <th width="400px">Timesspan</th>
                    <th width="100px">Count</th>
                    <th width="150px">Duration (Avg)</th>
                    <th width="150px">Duration (Med)</th>
                    <th width="150px">Duration (Min)</th>
                    <th width="150px">Duration (Max)</th>
                    <th width="150px">Duration (Total)</th>
                </tr>
                </thead>
                <?php foreach (array_slice($dates, -32, 32, true) as $date): ?>
                    <tr>
                        <td><?php echo $date; ?></td>
						<?php if (array_key_exists($date, $data) && array_key_exists($raction, $data[$date])): ?>
                            <td><?php echo $data[$date][$raction]['min_timestamp'] .  " - " . $data[$date][$raction]['max_timestamp']; ?></td>
                            <td><?php echo $data[$date][$raction]['count']; ?></td>
                            <td><?php echo round(($data[$date][$raction]['duration_avg'])/(1000.0*1000.0), 5); ?> s</td>
                            <td><?php echo round(($data[$date][$raction]['duration_median'])/(1000.0*1000.0), 5); ?> s</td>
                            <td><?php echo round(($data[$date][$raction]['duration_min'])/(1000.0*1000.0), 5); ?> s</td>
                            <td><?php echo round(($data[$date][$raction]['duration_max'])/(1000.0*1000.0), 5); ?> s</td>
                            <td><?php echo round(($data[$date][$raction]['duration'])/(1000.0*1000.0), 5); ?> s</td>
						<?php else: ?>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
						<?php endif; ?>
                    </tr>
                <?php endforeach; ?>
            </table>
        </div>

        <?php endforeach; ?>
    </div>

    <?php printSQLStats(); ?>
</body>
</html>