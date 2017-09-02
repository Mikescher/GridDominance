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
	function lc($txt) {
		$c = 0;
		foreach (explode("\n", $txt) as $l) { if (!empty($l)) $c++; }
		return $c;
	}
	function fmtLevelID($id) {
		if ($id == '{b16b00b5-0001-4000-9999-000000000002}') return "TUTORIAL";

		return (int)substr($id, 25, 6) . " - " . (int)substr($id, 31, 6);
	}
	?>

    <?php
        global $pdo;
        $user = GDUser::QueryByIDOrNull($pdo, $_GET['id']);

        $stmt = $pdo->prepare("SELECT * FROM users WHERE userid=:id");
        $stmt->bindValue(':id', $_GET['id'], PDO::PARAM_INT);
        $stmt->execute();
        $userdata = $stmt->fetch(PDO::FETCH_ASSOC);

    ?>

    <div class="infocontainer">
        <div class="infodiv">
            ID: <?php echo $user->ID; ?>
        </div>
        <div class="infodiv">
            Username: <?php echo $user->Username; ?>
        </div>
        <div class="infodiv">
            IsAutoUser: <?php echo $user->AutoUser ? 'true' : 'false'; ?>
        </div>
        <div class="infodiv">
            Revision: <?php echo $user->RevID; ?>
        </div>
        <div class="infodiv">
            Score: <?php echo $user->Score; ?>
        </div>
        <div class="infodiv">
            Score(MP): <?php echo $user->MultiplayerScore; ?>
        </div>
    </div>

    <div class="infocontainer">
        <div class="infodiv">
            Device: <?php echo $userdata['device_name']; ?>
        </div>
        <div class="infodiv">
            Operating System: <?php echo $userdata['device_version']; ?>
        </div>
        <div class="infodiv">
            Resolution: <?php echo $userdata['device_resolution']; ?>
        </div>
    </div>
    <div class="infocontainer">
        <div class="infodiv" title="<?php echo $userdata['unlocked_worlds']; ?>">
            Unlocks: <?php echo lc($userdata['unlocked_worlds']); ?>
        </div>
        <div class="infodiv">
            App Version: <?php echo $userdata['app_version']; ?>
        </div>
        <div class="infodiv">
            Last Online: <?php echo $userdata['last_online']; ?>
        </div>
        <div class="infodiv">
            Pings: <?php echo $userdata['ping_counter']; ?>
        </div>
    </div>

    <h2>Ranking</h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th style='width: 350px'>World</th>
                <th>Rank</th>
                <th style='width: 250px'>Username</th>
                <th>Total Score</th>
                <th>Total Time</th>
            </tr>
            </thead>

			<?php
			global $config;
			global $pdo;

			$stmt = $pdo->prepare(loadSQL("get-ranking_global_playerrank"));
			$stmt->bindValue(':uid', $user->ID, PDO::PARAM_INT);
			executeOrFail($stmt);
			$entry = $stmt->fetchAll(PDO::FETCH_ASSOC);

			if (count($entry) > 0):
                $entry = $entry[0];
			?>

                <tr>
                    <td>global</td>
                    <td><?php echo $entry['rank']; ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['totalscore']; ?></td>
                    <td title="<?php echo $entry['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", $entry['totaltime']/1000.0); ?></td>
                </tr>

			<?php endif; ?>

			<?php
			global $config;
			global $pdo;
                foreach (array_unique(array_map(function($k){ return $k[0]; }, $config['levelmapping'])) as $w):

                    if ($w == $config['worldid_0']) continue;

					$stmt = $pdo->prepare(loadReplSQL('get-ranking_local_playerrank', '#$$FIELD$$', worldGuidToSQLField($w)));
					$stmt->bindValue(':uid', $user->ID, PDO::PARAM_INT);
					executeOrFail($stmt);
					$entry = $stmt->fetchAll(PDO::FETCH_ASSOC);

                    if (count($entry) == 0) continue;

					$entry = $entry[0];

             ?>
                <tr>
                    <td><a href="worldhighscore.php?id=<?php echo $w; ?>"><?php echo $w; ?></a></td>
                    <td><?php echo $entry['rank']; ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['totalscore']; ?></td>
                    <td title="<?php echo $entry['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", $entry['totaltime']/1000.0); ?></td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <h2>Errors</h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered">
            <thead>
            <tr>
                <th>error id</th>
                <th style='width: 170px'>username</th>
                <th>pw verified</th>
                <th>resolution</th>
                <th>app version</th>
                <th>exception id</th>
                <th>message</th>
                <th>stacktrace</th>
                <th style='width: 170px'>timestamp</th>
                <th>additional info</th>
                <th>acknowledged</th>
            </tr>
            </thead>
			<?php foreach (getUserErrors($user->ID) as $entry): ?>
                <tr>
                    <td><?php echo $entry['error_id']; ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['password_verified']; ?></td>
					<?php expansioncell($entry['screen_resolution']); ?>
                    <td><?php echo $entry['app_version']; ?></td>
                    <td><?php echo $entry['exception_id']; ?></td>
					<?php expansioncell($entry['exception_message']); ?>
					<?php expansioncell($entry['exception_stacktrace']); ?>
                    <td><?php echo $entry['timestamp']; ?></td>
					<?php expansioncell($entry['additional_info']); ?>
                    <td><?php echo $entry['acknowledged']; ?></td>
                </tr>
                <tr class='tab_prev' id='tr_prev_<?php echo $previd; ?>'><td colspan='12' id='td_prev_<?php echo $previd; ?>' style='text-align: left;' ></td></tr>
				<?php $previd++; ?>
			<?php endforeach; ?>
        </table>
    </div>

    <h2>Scores</h2>


    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th>World</th>
                <th>Score</th>
                <th>Time</th>
            </tr>
            </thead>
            <tr> <td>Total</td> <td><?php echo $user->Score; ?>           </td> <td title="<?php echo $user->TotalTime; ?>ms" ><?php echo gmdate("H:i:s", $user->TotalTime/1000.0); ?></td> </tr>
            <tr> <td>W1</td>    <td><?php echo $user->ScoreW1; ?>         </td> <td title="<?php echo $user->TimeW1; ?>ms" ><?php echo gmdate("H:i:s", $user->TimeW1/1000.0); ?></td> </tr>
            <tr> <td>W2</td>    <td><?php echo $user->ScoreW2; ?>         </td> <td title="<?php echo $user->TimeW2; ?>ms" ><?php echo gmdate("H:i:s", $user->TimeW2/1000.0); ?></td> </tr>
            <tr> <td>W3</td>    <td><?php echo $user->ScoreW3; ?>         </td> <td title="<?php echo $user->TimeW3; ?>ms" ><?php echo gmdate("H:i:s", $user->TimeW3/1000.0); ?></td> </tr>
            <tr> <td>W4</td>    <td><?php echo $user->ScoreW4; ?>         </td> <td title="<?php echo $user->TimeW4; ?>ms" ><?php echo gmdate("H:i:s", $user->TimeW4/1000.0); ?></td> </tr>
            <tr> <td>MP</td>    <td><?php echo $user->MultiplayerScore; ?></td> <td title="?"></td> </tr>
        </table>
    </div>

    <h2>&nbsp;</h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th style='width: 170px'>Username</th>
                <th>Level</th>
                <th>Difficulty</th>
                <th>Time</th>
                <th style='width: 170px'>Last changed</th>
            </tr>
            </thead>
			<?php foreach (getUserEntries($user->ID) as $entry): ?>
                <tr>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td title="<?php echo $entry['levelid']; ?>" >
                        <a href="levelscores.php?id=<?php echo $entry['levelid']; ?>">
							<?php echo fmtLevelID($entry['levelid']); ?>
                        </a>
                    </td>
                    <td><?php echo $entry['difficulty']; ?></td>
                    <td title="<?php echo $entry['best_time']; ?>ms" ><?php echo gmdate("H:i:s", $entry['best_time']/1000.0); ?></td>
                    <td><?php echo $entry['last_changed']; ?></td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>


    <script type="text/javascript">
		<?php echo file_get_contents('admin.js'); ?>
    </script>
    <script src="sorttable.js"></script>
</body>
</html>