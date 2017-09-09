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
	function fmtLevelID($id) {
		if ($id == '{b16b00b5-0001-4000-9999-000000000002}') return "TUTORIAL";

		return (int)substr($id, 25, 6) . " - " . (int)substr($id, 31, 6);
	}
	function getSessionCount() {
	    if (! file_exists("/var/log/gdapi_log/proxystate.json")) return "?";

		$string = file_get_contents("/var/log/gdapi_log/proxystate.json");
		$json = json_decode($string, true);

		return count($json['sessions']);
	}

	function lc($txt) {
		$c = 0;
		foreach (explode("\n", $txt) as $l) { if (!empty($l)) $c++; }
		return $c;
	}

	?>

	<div class="infocontainer">
		<div class="infodiv">
			Total Users: <a href="userlist.php"><?php echo getUserCount(); ?></a>&nbsp;&nbsp;(<a href="statistics.php">stats</a>)
		</div>
		<div class="infodiv">
			Active Users (week):  <a href="userlist.php?d=7"><?php echo getActiveUserCount(7); ?></a>
		</div>
        <div class="infodiv">
            Active Users (today): <a href="userlist.php?d=1"><?php echo getActiveUserCount(1); ?></a>
        </div>
        <div class="infodiv">
            Online Sessions: <a href="proxylist.php"><?php echo getSessionCount(); ?></a>
        </div>
	</div>

	<div class="infocontainer">
		<div class="infodiv">
			Errors: <a href="errorlist.php?filter=1"><?php echo getRemainingErrorCount(); ?>/<?php echo getErrorCount(); ?></a>
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


    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">New errors [+/-]</h2>

        <table class="sqltab pure-table pure-table-bordered">
            <thead>
                <tr>
                    <th>id</th>
                    <th style='width: 250px'>Username</th>
                    <th>anon</th>
                    <th>resolution</th>
                    <th>version</th>
                    <th style="width: 170px;">exception id</th>
                    <th>msg</th>
                    <th>trace</th>
                    <th style='width: 160px'>timestamp</th>
                    <th>additional info</th>
                    <th>acknowledged</th>
                </tr>
            </thead>
            <?php foreach (getRemainingErrors() as $entry): ?>
                <tr>
                    <td><?php echo $entry['error_id']; ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['password_verified']?0:1; ?></td>
                    <td><?php echo $entry['screen_resolution']; ?></td>
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

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Logs [+/-]</h2>

        <table class="sqltab pure-table pure-table-bordered" style="width:100%;">
            <thead>
            <tr>
                <th style='width: 250px'>Logfile</th>
                <th style='width: 250px'>Changedate</th>
                <th>Entries</th>
                <th>Content</th>
                <th>Tasks</th>
            </tr>
            </thead>
			<?php foreach (listLogFiles() as $entry): ?>
                <tr>
                    <td title="<?php echo $entry['path']; ?>" ><?php echo $entry['name']; ?></td>
                    <td><?php echo $entry['changedate']; ?></td>
                    <td><?php echo lc($entry['content']); ?></td>
					<?php remoteexpansioncell($entry['name']); ?>
                    <td>
                        <a href="logquery.php?id=<?php echo $entry['name']; ?>">Download</a>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <a href="logpurge.php?id=<?php echo $entry['name']; ?>">Purge</a>
                    </td>
                </tr>
                <tr class='tab_prev' id='tr_prev_<?php echo $previd; ?>'><td colspan='12'><div class='div_prev' id='td_prev_<?php echo $previd; ?>' style='text-align: left;' ></div></td></tr>
				<?php $previd++; ?>
			<?php endforeach; ?>
        </table>
    </div>

    <div data-collapse>
        <h2 class="open collapseheader">Global highscore [+/-]</h2>

        <div>
            <?php
            global $config;
            foreach (array_unique(array_map(function($k){ return $k[0]; }, $config['levelmapping'])) as $w) {
                if ($w != $config['worldid_0']) echo "<a href=\"worldhighscore.php?id=$w\">$w</a><br/>";
            }
            ?>

            <br/>
            <br/>

            <div class="tablebox">
                <table class="sqltab pure-table pure-table-bordered sortable">
                    <thead>
                    <tr>
                        <th>Row</th>
                        <th style='width: 250px'>Username</th>
                        <th>Score</th>
                        <th>Time</th>
                    </tr>
                    </thead>
                    <?php $i=1; foreach (getGlobalHighscores() as $entry): ?>
                        <tr>
                            <td><?php echo $i++; ?></td>
                            <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                            <td><?php echo $entry['totalscore']; ?></td>
                            <td title="<?php echo $entry['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", $entry['totaltime']/1000.0); ?></td>
                        </tr>
                    <?php endforeach; ?>
                </table>
            </div>
        </div>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Level highscores [+/-]</h2>

        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th>ID</th>
                <th>Difficulty</th>
                <th>Highscore</th>
                <th style='width: 250px'>Highscore Holder</th>
                <th style='width: 200px'>Last Changed</th>
                <th>Completion Count</th>
            </tr>
            </thead>
			<?php foreach (getLevelHighscores() as $entry): ?>
                <tr>
                    <td title="<?php echo $entry['levelid']; ?>" >
                        <?php if ($entry['difficulty'] == 0): ?>
                            <a href="levelscores.php?id=<?php echo $entry['levelid']; ?>">
								<?php echo fmtLevelID($entry['levelid']); ?>
                            </a>
                        <?php else: ?>
                            <?php echo fmtLevelID($entry['levelid']); ?>
                        <?php endif; ?>
                    </td>
                    <td><?php echo $entry['difficulty']; ?></td>
                    <td title="<?php echo $entry['best_time']; ?>ms" ><?php echo gmdate("H:i:s", $entry['best_time']/1000.0); ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['best_userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['best_userid']; ?>)</td>
                    <td><?php echo $entry['best_last_changed']; ?></td>
                    <td><?php echo $entry['completion_count']; ?></td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Multiplayer highscore [+/-]</h2>

        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th>Row</th>
                <th style='width: 250px'>Username</th>
                <th>MultiplayerScore</th>
            </tr>
            </thead>
			<?php $i=1; foreach (getMultiplayerHighscores() as $entry): ?>
                <tr>
                    <td><?php echo $i++; ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['mpscore']; ?></td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <script type="text/javascript">
        <?php echo file_get_contents('admin.js'); ?>
    </script>

    <script src="jquery.collapse.js"></script>
    <script src="toastr.min.js"></script>
    <script src="sorttable.js"></script>
</body>
</html>