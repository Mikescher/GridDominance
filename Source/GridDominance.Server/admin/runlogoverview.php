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

    ?>

    <?php
    
	function fmtd($df)
    {
		$d = DateTime::createFromFormat('Y-m-d H:i:s', $df);
		$d->setTime(round($d->format('H')/6)*6, 0, 0);
		return $d->format('Y-m-d H:i');
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
        return $dates;
    }

    function getDateData($rloglist, $runlogs, $dates, $field)
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
                        $v = round($v/(1000.0*1000.0), 5);
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

	$datedata_avg      = getDateData($rloglist, $runlogs, $dates, 'duration_avg');
	$datedata_median   = getDateData($rloglist, $runlogs, $dates, 'duration_median');
	$datedata_reqcount = getSumDateData($rloglist, $runlogs, $dates, 'count');

    ?>

    <div class="graphbox" data-collapse>
        <h2 class="open collapseheader">History (Median)</h2>
        <div>
            <canvas id="scoreChart1" width="85%" height="25%"></canvas>
            <script>
                let ctx1 = document.getElementById("scoreChart1").getContext('2d');

                new Chart(ctx1,
                    {
                        type: 'line',
                        data:
                            {
                                labels: [ <?php foreach ($dates as $rld) echo "'".$rld."',"; ?> ],
                                datasets:
                                    [
										<?php $i=0; ?>
										<?php foreach ($rloglist as $raction): ?>
										<?php $i++; ?>
										<?php if ($raction['action'] == 'cron') continue; ?>
                                        {
                                            label: '<?php echo $raction['action']; ?>',
                                            data: [ <?php foreach ($datedata_median[$raction['action']] as $dd) echo $dd.","; ?> ],
                                            borderColor: '<?php echo $COLORS[$i%count($COLORS)]; ?>',
                                            backgroundColor: 'transparent',
                                        },
										<?php endforeach; ?>
                                    ]
                            },
                    });
            </script>
        </div>
    </div>

    <div class="graphbox" data-collapse>
        <h2 class="collapseheader">History (Average)</h2>
        <div>
            <canvas id="scoreChart3" width="85%" height="25%"></canvas>
            <script>
                let ctx3 = document.getElementById("scoreChart3").getContext('2d');

                new Chart(ctx3,
                    {
                        type: 'line',
                        data:
                            {
                                labels: [ <?php foreach ($dates as $rld) echo "'".$rld."',"; ?> ],
                                datasets:
                                    [
										<?php $i=0; ?>
										<?php foreach ($rloglist as $raction): ?>
										<?php $i++; ?>
										<?php if ($raction['action'] == 'cron') continue; ?>
                                        {
                                            label: '<?php echo $raction['action']; ?>',
                                            data: [ <?php foreach ($datedata_avg[$raction['action']] as $dd) echo $dd.","; ?> ],
                                            borderColor: '<?php echo $COLORS[$i%count($COLORS)]; ?>',
                                            backgroundColor: 'transparent',
                                        },
										<?php endforeach; ?>
                                    ]
                            },
                    });
            </script>
        </div>
    </div>

    <div class="graphbox" data-collapse>
        <h2 class="collapseheader">History (Cron)</h2>
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
        <h2 class="collapseheader">Requests/Time</h2>
        <div>
            <canvas id="scoreChart4" width="85%" height="25%"></canvas>
            <script>
                let ctx4 = document.getElementById("scoreChart4").getContext('2d');

                new Chart(ctx4,
                    {
                        type: 'line',
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
        </div>
    </div>

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

    <?php printSQLStats(); ?>

    <script type="text/javascript">
		<?php echo file_get_contents('admin.js'); ?>
    </script>

    <script src="sorttable.js"></script>
    <script src="jquery.collapse.js"></script>
</body>
</html>