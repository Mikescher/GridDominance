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
			Errors: <a href="errorlist.php"><?php echo getErrorCount(); ?>/<?php echo getRemainingErrorCount(); ?></a>
		</div>
		<div class="infodiv">
			Entries:  <a href="entrylist.php"><?php echo getEntryCount(); ?></a>
		</div>
        <div class="infodiv">
            Highscore: <?php echo getTotalHighscore(); ?>
        </div>
        <div class="infodiv">
            <a href="<?php global $config; echo "../cron.php?cronsecret=" . $config['cron-secret']; ?>">Cron</a>
        </div>
	</div>

	<h2>New errors</h2>

    <?php

    $previd = 0;
	function expansioncell($txt) {
	    global $previd;

	    echo "<td>";
	    echo "<a href='#' onclick='ShowExpandedColumn(" . $previd . ", " . str_replace("'", "\\u0027", str_replace('\n', '<br/>', json_encode($txt))) . ");return false;'>show</a>";
		echo "</td>";
    }
    ?>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered">
            <thead>
                <tr>
                    <th>error id</th>
                    <th>userid</th>
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
            <?php foreach (getRemainingErrors() as $error): ?>
                <tr>
                    <td><?php echo $error['error_id']; ?></td>
                    <td><?php echo $error['userid']; ?></td>
                    <td><?php echo $error['password_verified']; ?></td>
                    <td style='max-width: 256px'><?php echo $error['screen_resolution']; ?></td>
                    <td><?php echo $error['app_version']; ?></td>
                    <td><?php echo $error['exception_id']; ?></td>
					<?php expansioncell($error['exception_message']); ?>
					<?php expansioncell($error['exception_stacktrace']); ?>
                    <td style='max-width: 256px'><?php echo $error['timestamp']; ?></td>
					<?php expansioncell($error['additional_info']); ?>
                    <td><a href="ack.php?id=<?php echo $error['error_id']; ?>" target="_blank">acknowledge</a></td>
                </tr>
                <tr class='tab_prev' id='tr_prev_<?php echo $previd; ?>'><td colspan='12' id='td_prev_<?php echo $previd; ?>' style='text-align: left;' ></td></tr>
                <?php $previd++; ?>
            <?php endforeach; ?>
        </table>
    </div>

    <script type="text/javascript">
        function ShowExpandedColumn(id, text) {
            $(".tab_prev").css("visibility", "collapse");
            $("#td_prev_"+id).html(text);
            $("#tr_prev_"+id).css("visibility", "visible");
        }
    </script>

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
                <th>Username</th>
                <th>Score</th>
                <th>Time</th>
            </tr>
            </thead>
			<?php foreach (getGlobalHighscores() as $ghigh): ?>
                <tr>
                    <td><?php echo $ghigh['username'] . " (" .$ghigh['userid']. ")"; ?></td>
                    <td><?php echo $ghigh['totalscore']; ?></td>
                    <td title="<?php echo $ghigh['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", $ghigh['totaltime']/1000.0); ?></td>
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
			<?php foreach (getLevelHighscores() as $lhigh): ?>
                <tr>
                    <td title="<?php echo $lhigh['levelid']; ?>" >
                        <a href="levelscores.php?id=<?php echo $lhigh['levelid']; ?>">
							<?php echo (int)substr($lhigh['levelid'], 25, 6) . " - " . (int)substr($lhigh['levelid'], 31, 6); ?>
                        </a>
                    </td>
                    <td><?php echo $lhigh['difficulty']; ?></td>
                    <td title="<?php echo $lhigh['best_time']; ?>ms" ><?php echo gmdate("H:i:s", $lhigh['best_time']/1000.0); ?></td>
                    <td><?php echo $lhigh['username'] . " (" .$lhigh['best_userid']. ")"; ?></td>
                    <td><?php echo $lhigh['best_last_changed']; ?></td>
                    <td><?php echo $lhigh['completion_count']; ?></td>
                </tr>
                <tr class='tab_prev' id='tr_prev_<?php echo $previd; ?>'><td colspan='12' id='td_prev_<?php echo $previd; ?>' style='text-align: left;' ></td></tr>
			<?php endforeach; ?>
        </table>
    </div>
</body>
</html>