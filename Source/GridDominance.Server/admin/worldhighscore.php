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

    $id = '@';
	if (!empty($_GET['id'])) $id = $_GET['id'];

	$page = 0;
	if (!empty($_GET['page'])) $page = $_GET['page'];


	if ($_GET['id']=='*') {
		$entries = getGlobalHighscores(1000, $page);
		$entrycount = getUserCount();
		$showtime = true;
    } else if ($_GET['id']=='@') {
		$entries = getMultiplayerHighscores(1000, $page);
		$entrycount = getUserCountWithMPScore();
		$showtime = false;
    } else {
		$entries = getWorldHighscores($_GET['id'], 1000, $page);
		$entrycount = getUserCount();
		$showtime = true;
    }


    ?>

    <h2><?php echo htmlspecialchars($_GET['id']); ?></h2>

    <?php if ($_GET['id'] == '*'): ?>


        <div class="graphbox" data-collapse>
            <h2 class="collapseheader">Top score / Time</h2>
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
									$statshist = sql_query_assoc("GetStatsHistory", "SELECT * FROM stats_history ORDER BY exectime ASC");
									for ($i=0; $i < count($statshist); $i++)
									{
										if ($i>0)echo ',';
										echo "{";
										echo " date: new Date('".$statshist[$i]['exectime'].   "'), ";
										echo " count_1: "       .$statshist[$i]['user_topscore']. ", ";
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
                                        "fillAlphas": 0.4,
                                        "valueField": "count_1",
                                        "bullet": "square",
                                        "bulletBorderAlpha": 1,
                                        "bulletBorderThickness": 1,
                                        "balloonText": "<div style='margin:5px; font-size:19px;'>Users with top score:<b>[[value]]</b></div>"
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

    <?php endif; ?>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
                <tr>
                    <th>Row</th>
                    <th style='width: 250px'>Username</th>
                    <th>Score</th>
                    <?php if ($showtime): ?>
                    <th>Time</th>
					<?php endif; ?>
                </tr>
            </thead>
            <?php $i=1 + ($page * 1000); foreach ($entries as $entry): ?>
                <tr>
                    <td><?php echo $i++; ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['totalscore']; ?></td>
                    <?php if ($showtime): ?>
                        <td title="<?php echo $entry['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", $entry['totaltime']/1000.0); ?></td>
                    <?php endif; ?>
                </tr>
            <?php endforeach; ?>
        </table>
        <div class="pagination_row">
            <?php for ($i=0; $i < ceil($entrycount/1000); $i++ ): ?>
                <?php if ($i != $page): ?>
                    <a class="pagination_link" href="<?php echo suffixGetParams('page', $i); ?>"><?php echo ($i+1); ?></a>
                <?php else: ?>
                    <a class="pagination_curr"><?php echo ($i+1); ?></a>
                <?php endif; ?>
            <?php endfor; ?>
        </div>
    </div>

    <?php printSQLStats(); ?>
</body>
</html>