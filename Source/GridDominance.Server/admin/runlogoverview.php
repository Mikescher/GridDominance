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

    $DISABLED = ['cron', 'admin', 'change-password', 'log-client', 'merge-login', 'savesessionstate', 'get-ranking', 'upgrade-user', 'set-multiscore' ];

    ?>

    <?php
    
	function fmtd($df)
    {
		$a = explode(' ', $df);
		$b = explode(':', $a[1]);

		return $a[0] . ' ' . 6*($b[0]/6) . ':' . $b[1];
    }

    function getDates($rloglist, $runlogs)
    {
        $dates = [];
        foreach ($rloglist as $raction)
        {
            foreach ($runlogs[$raction['action']] as $entry) $dates []= fmtd($entry['exectime']);
        }
        asort($dates);
		$dates = array_unique($dates);
		$dates = array_values($dates);
        return $dates;
    }

    function getDateData($rloglist, $runlogs, $dates, $field, $round=true)
    {
	    $datedata = [];

        foreach ($rloglist as $raction)
        {
            $arr = [];

            $last = 0;
            foreach ($dates as $date)
            {
                $v = $last;
                foreach ($runlogs[$raction['action']] as $entry)
                {
                    if (fmtd($entry['exectime']) == $date) 
                    {
                        $v = $entry[$field];
                        if ($round) $v = round($v/(1000.0*1000.0), 5);
                    }
                }
                $arr []= $v;
                $last = $v;
            }

            $datedata[$raction['action']]    = $arr;
        }

        return $datedata;
    }

    function getSumDateData($rloglist, $runlogs, $dates, $field)
    {
	    $datedata = [];

        foreach ($dates as $date)
        {
			$sum = 0;
            foreach ($rloglist as $raction)
            {
                foreach ($runlogs[$raction['action']] as $entry)
                {
                    if (fmtd($entry['exectime']) == $date) 
                    {
                        $v = $entry[$field];
                        $sum = $sum + $v;
                    }
                }
            }

            $datedata []= $sum;
        }
        
        return $datedata;
    }

    ?>

    <?php

	$rloglist = getRunLogActionList();
    $runlogs = [];
	foreach ($rloglist as $raction) $runlogs[$raction['action']] = getRunLog($raction['action']);

	$dates = getDates($rloglist, $runlogs);

	$datedata_avg      = getDateData($rloglist, $runlogs, $dates,    'duration_avg');
	$datedata_median   = getDateData($rloglist, $runlogs, $dates,    'duration_median');
	$datedata_count    = getDateData($rloglist, $runlogs, $dates,    'count', false);
	$datedata_reqcount = getSumDateData($rloglist, $runlogs, $dates, 'count');

    ?>

    <div class="graphbox" data-collapse>
        <h2 class="open collapseheader">History (Median)</h2>
        <div>
            <div id="scoreChart1" style="width:calc(100% - 8px); height:850px"></div>
            <script>
                AmCharts.makeChart("scoreChart1", {
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
							for ($i=0; $i < count($dates); $i++)
							{
								if ($i>0)echo ',';
								echo "{";
								echo " date: new Date('".$dates[$i].          "')";

								$j=0;
								foreach ($rloglist as $raction)
                                {
                                    $j++;
									echo ", count_".$j.": "       .$datedata_median[$raction['action']][$i];
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
							<?php $i=0; foreach ($rloglist as $raction): $i++; ?>
							<?php if ($i>1) echo ','; ?>
                            {
                                "id": "g<?php echo $i; ?>",
                                "fillAlphas": 0,
                                "title": "<?php echo $raction['action']; ?>",
                                "hidden": <?php echo (in_array($raction['action'], $DISABLED)) ? 'true' : 'false'; ?>,
                                "valueField": "count_<?php echo $i; ?>",
                                "bullet": "round",
                                "lineColor": "<?php echo $COLORS[$i%count($COLORS)]; ?>",
                                "bulletBorderAlpha": 0.2,
                                "bulletAlpha": 0.5,
                                "bulletSize": 4,
                                "bulletBorderThickness": 1,
                                "lineThickness": 2,
                                "type": "line",
                                "balloonText": "<?php echo $raction['action']; ?>: <b>[[value]]s</b>"
                            }
                            <?php endforeach; ?>
                        ],
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


                function legendHandler1( evt ) {
                    var state = evt.dataItem.hidden;
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
                AmCharts.makeChart("scoreChart3", {
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
                            "method": legendHandler3
                        }, {
                            "event": "showItem",
                            "method": legendHandler3
                        } ]
                    },
                    "dataProvider":
                        [
							<?php
							for ($i=0; $i < count($dates); $i++)
							{
								if ($i>0)echo ',';
								echo "{";
								echo " date: new Date('".$dates[$i].          "')";

								$j=0;
								foreach ($rloglist as $raction)
								{
									$j++;
									echo ", count_".$j.": "       .$datedata_avg[$raction['action']][$i];
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
							<?php $i=0; foreach ($rloglist as $raction): $i++; ?>
							<?php if ($i>1) echo ','; ?>
                            {
                                "id": "g<?php echo $i; ?>",
                                "fillAlphas": 0,
                                "title": "<?php echo $raction['action']; ?>",
                                "hidden": <?php echo (in_array($raction['action'], $DISABLED)) ? 'true' : 'false'; ?>,
                                "valueField": "count_<?php echo $i; ?>",
                                "bullet": "none",
                                "lineColor": "<?php echo $COLORS[$i%count($COLORS)]; ?>",
                                "bulletBorderAlpha": 1,
                                "bulletBorderThickness": 1,
                                "lineThickness": 2,
                                "type": "smoothedLine",
                                "balloonText": "<?php echo $raction['action']; ?>: <b>[[value]]s</b>"
                            }
							<?php endforeach; ?>
                        ],
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


                function legendHandler3( evt ) {
                    var state = evt.dataItem.hidden;
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
        <h2 class="collapseheader">History (Cron)</h2> <?php //TODO AMCharts ?>
        <div>
            <canvas id="scoreChart2" width="85%" height="25%"></canvas>
            <script>
                let ctx2 = document.getElementById("scoreChart2").getContext('2d');

                new Chart(ctx2,
                    {
                        type: 'line',
                        data:
                            {
                                labels: [ <?php foreach ($dates as $rld) echo "'".$rld."',"; ?> ],
                                datasets:
                                    [
										<?php foreach ($rloglist as $raction): ?>
										<?php if ($raction['action'] != 'cron') continue; ?>
                                        {
                                            label: '<?php echo $raction['action']; ?>',
                                            data: [ <?php foreach ($datedata_avg[$raction['action']] as $dd) echo $dd.","; ?> ],
                                        },
										<?php endforeach; ?>
                                    ]
                            },
                    });
            </script>
        </div>
    </div>

    <div class="graphbox" data-collapse>
        <h2 class="collapseheader">Requests/Time</h2> <?php //TODO AMCharts ?>
        <div>
            <canvas id="scoreChart4" width="85%" height="25%"></canvas>
            <script>
                let ctx4 = document.getElementById("scoreChart4").getContext('2d');

                new Chart(ctx4,
                    {
                        type: 'line',
                        options: { scales: { yAxes: [{ ticks: { suggestedMin: 0, } }] }, },
                        data:
                            {
                                labels: [ <?php foreach ($dates as $rld) echo "'".$rld."',"; ?> ],
                                datasets:
                                    [
                                        {
                                            label: 'Requests',
                                            data: [ <?php foreach ($datedata_reqcount as $dd) echo $dd.","; ?> ],
                                        },
                                    ]
                            },
                    });
            </script>


            <div>
                <div id="scoreChart5" style="width:calc(100% - 8px); height:650px"></div>
                <script>
                    AmCharts.makeChart("scoreChart5", {
                        "type": "serial",
                        "theme": "light",
                        "marginRight": 80,
                        "legend": {
                            "equalWidths": false,
                            "periodValueText": "Count: [[value.sum]]s",
                            "position": "top",
                            "valueAlign": "left",
                            "valueWidth": 100,
                            "listeners": [ {
                                "event": "hideItem",
                                "method": legendHandler5
                            }, {
                                "event": "showItem",
                                "method": legendHandler5
                            } ]
                        },
                        "dataProvider":
                            [
								<?php
								for ($i=0; $i < count($dates); $i++)
								{
									if ($i>0)echo ',';
									echo "{";
									echo " date: new Date('".$dates[$i].          "')";

									$j=0;
									foreach ($rloglist as $raction)
									{
										$j++;
										echo ", count_".$j.": "       .$datedata_count[$raction['action']][$i];
									}
									echo "}\n";
								}
								?>
                            ],
                        "valueAxes": [{
                            "position": "left",
                            "title": "Requests"
                        }],
                        "graphs":
                            [
								<?php $i=0; foreach ($rloglist as $raction): $i++; ?>
								<?php if ($i>1) echo ','; ?>
                                {
                                    "id": "g<?php echo $i; ?>",
                                    "fillAlphas": 0,
                                    "title": "<?php echo $raction['action']; ?>",
                                    "valueField": "count_<?php echo $i; ?>",
                                    "bullet": "none",
                                    "lineColor": "<?php echo $COLORS[$i%count($COLORS)]; ?>",
                                    "bulletBorderAlpha": 1,
                                    "bulletBorderThickness": 1,
                                    "lineThickness": 2,
                                    "type": "line",
                                    "balloonText": "<?php echo $raction['action']; ?>: <b>[[value]]</b>"
                                }
								<?php endforeach; ?>
                            ],
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


                    function legendHandler5( evt ) {
                        var state = evt.dataItem.hidden;
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
    </div>

    <div class="samsupertabcontainer">
        <div class="samtabheaderbox">

            <?php foreach($rloglist as $raction): ?>
            <div class="samtabheader" data-tabheader data-tabid="<?php echo $raction['action']; ?>" data-tabcontainerid="c1"><?php echo $raction['action']; ?></div>
            <?php endforeach; ?>
        </div>

        <?php foreach($rloglist as $raction): ?>

        <div class="samtabbox" data-tabcontent data-tabid="<?php echo $raction['action']; ?>" data-tabcontainerid="c1">
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
                <?php foreach ($runlogs[$raction['action']] as $entry): ?>
                    <tr>
                        <td><?php echo $entry['exectime']; ?></td>
                        <td><?php echo $entry['min_timestamp'] .  " - " . $entry['max_timestamp']; ?></td>
                        <td><?php echo $entry['count']; ?></td>
                        <td><?php echo round(($entry['duration_avg'])/(1000.0*1000.0), 5); ?> s</td>
                        <td><?php echo round(($entry['duration_median'])/(1000.0*1000.0), 5); ?> s</td>
                        <td><?php echo round(($entry['duration_min'])/(1000.0*1000.0), 5); ?> s</td>
                        <td><?php echo round(($entry['duration_max'])/(1000.0*1000.0), 5); ?> s</td>
                        <td><?php echo round(($entry['duration'])/(1000.0*1000.0), 5); ?> s</td>
                    </tr>
                <?php endforeach; ?>
            </table>
        </div>

        <?php endforeach; ?>
    </div>

    <?php printSQLStats(); ?>
</body>
</html>