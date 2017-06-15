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
	function remoteexpansioncell($txt) {
		global $previd;

		echo "<td>";
		echo "<a href='#' onclick='ShowRemoteExpandedColumn(" . $previd . ", \"" . $txt . "\");return false;'>show</a>";
		echo "</td>";
	}
	?>

	<div class="infocontainer">
		<div class="infodiv">
			Total Users: <a href="userlist.php"><?php echo getUserCount(); ?></a>
		</div>
		<div class="infodiv">
			Active Users (week):  <?php echo getActiveUserCount(7); ?>
		</div>
		<div class="infodiv">
			Active Users (today): <?php echo getActiveUserCount(1); ?>
		</div>
	</div>

	<div class="infocontainer">
		<div class="infodiv">
			Errors: <a href="errorlist.php"><?php echo getRemainingErrorCount(); ?>/<?php echo getErrorCount(); ?></a>
		</div>
		<div class="infodiv">
			Entries:  <a href="entrylist.php"><?php echo getEntryCount(); ?></a>
		</div>
        <div class="infodiv">
            Highscore: <?php echo getTotalHighscore(); ?>
        </div>
        <div class="infodiv">
            Cron: <?php echo getLastCronTime(); ?> ( <a href="<?php global $config; echo "../cron.php?cronsecret=" . $config['cron-secret']; ?>">Now</a> )
        </div>
	</div>

	<h2>New errors</h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered">
            <thead>
                <tr>
                    <th>error id</th>
                    <th style='width: 250px'>Username</th>
                    <th>password verified</th>
                    <th>screen resolution</th>
                    <th>app version</th>
                    <th>exception id</th>
                    <th>exception message</th>
                    <th>exception stacktrace</th>
                    <th>timestamp</th>
                    <th>additional info</th>
                    <th>acknowledged</th>
                </tr>
            </thead>
            <?php foreach (getRemainingErrors() as $entry): ?>
                <tr>
                    <td><?php echo $entry['error_id']; ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['password_verified']; ?></td>
                    <td style='max-width: 256px'><?php echo $entry['screen_resolution']; ?></td>
                    <td><?php echo $entry['app_version']; ?></td>
                    <td><?php echo $entry['exception_id']; ?></td>
					<?php expansioncell($entry['exception_message']); ?>
					<?php expansioncell($entry['exception_stacktrace']); ?>
                    <td style='max-width: 256px'><?php echo $entry['timestamp']; ?></td>
					<?php expansioncell($entry['additional_info']); ?>
                    <td><a href="ack.php?id=<?php echo $entry['error_id']; ?>" target="_blank">acknowledge</a></td>
                </tr>
                <tr class='tab_prev' id='tr_prev_<?php echo $previd; ?>'><td colspan='12' id='td_prev_<?php echo $previd; ?>' style='text-align: left;' ></td></tr>
                <?php $previd++; ?>
            <?php endforeach; ?>
        </table>
    </div>

    <h2>Logs</h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered" style="width:100%;">
            <thead>
            <tr>
                <th style='width: 250px'>Logfile</th>
                <th style='width: 250px'>Changedate</th>
                <th>Content</th>
            </tr>
            </thead>
			<?php foreach (listLogFiles() as $entry): ?>
                <tr>
                    <td title="<?php echo $entry['path']; ?>" ><?php echo $entry['name']; ?></td>
                    <td><?php echo $entry['changedate']; ?></td>
					<?php remoteexpansioncell($entry['name']); ?>
                </tr>
                <tr class='tab_prev' id='tr_prev_<?php echo $previd; ?>'><td colspan='12' id='td_prev_<?php echo $previd; ?>' style='text-align: left;' ></td></tr>
				<?php $previd++; ?>
			<?php endforeach; ?>
        </table>
    </div>

	<h2>Global highscore</h2>

    <?php
    global $config;
    foreach (array_unique(array_map(function($k){ return $k[0]; }, $config['levelmapping'])) as $w) {
		echo "<a href=\"worldhighscore.php?id=$w\">$w</a><br/>";
    }
    ?>

    <br/>
    <br/>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered">
            <thead>
            <tr>
                <th style='width: 250px'>Username</th>
                <th>Score</th>
                <th>Time</th>
            </tr>
            </thead>
			<?php foreach (getGlobalHighscores() as $entry): ?>
                <tr>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['totalscore']; ?></td>
                    <td title="<?php echo $entry['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", $entry['totaltime']/1000.0); ?></td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>

	<h2>Level highscores</h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered">
            <thead>
            <tr>
                <th>ID</th>
                <th>Difficulty</th>
                <th>Highscore</th>
                <th>Highscore Holder</th>
                <th style='width: 200px'>Last Changed</th>
                <th>Completion Count</th>
            </tr>
            </thead>
			<?php foreach (getLevelHighscores() as $entry): ?>
                <tr>
                    <td title="<?php echo $entry['levelid']; ?>" >
                        <?php if ($entry['difficulty'] == 0): ?>
                            <a href="levelscores.php?id=<?php echo $entry['levelid']; ?>">
								<?php echo (int)substr($entry['levelid'], 25, 6) . " - " . (int)substr($entry['levelid'], 31, 6); ?>
                            </a>
                        <?php else: ?>
                            <?php echo (int)substr($entry['levelid'], 25, 6) . " - " . (int)substr($entry['levelid'], 31, 6); ?>
                        <?php endif; ?>
                    </td>
                    <td><?php echo $entry['difficulty']; ?></td>
                    <td title="<?php echo $entry['best_time']; ?>ms" ><?php echo gmdate("H:i:s", $entry['best_time']/1000.0); ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['best_userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['best_userid']; ?>)</td>
                    <td><?php echo $entry['best_last_changed']; ?></td>
                    <td><?php echo $entry['completion_count']; ?></td>
                </tr>
                <tr class='tab_prev' id='tr_prev_<?php echo $previd; ?>'><td colspan='12' id='td_prev_<?php echo $previd; ?>' style='text-align: left;' ></td></tr>
			<?php endforeach; ?>
        </table>
    </div>

    <script type="text/javascript">
        function ShowExpandedColumn(id, text) {
            if ($("#tr_prev_"+id).css('visibility') != 'visible') {
                $(".tab_prev").css("visibility", "collapse");
                $("#td_prev_"+id).html(text);
                $("#tr_prev_"+id).css("visibility", "visible");
            } else {
                $(".tab_prev").css("visibility", "collapse");
            }
        }
        function ShowRemoteExpandedColumn(id, ident) {
            if ($("#tr_prev_"+id).css('visibility') != 'visible') {
                $(".tab_prev").css("visibility", "collapse");

                $.get('logquery.php?id=' + ident, function( data ) {
                    $("#td_prev_"+id).html(data);
                    $("#tr_prev_"+id).css("visibility", "visible");
                });

            } else {
                $(".tab_prev").css("visibility", "collapse");
            }
        }
    </script>
</body>
</html>