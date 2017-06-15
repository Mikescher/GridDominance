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
	?>

    <?php global $pdo; $user = GDUser::QueryByIDOrNull($pdo, $_GET['id']); ?>

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
    </div>

    <h2>Ranking</h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered">
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

					$condition = ' WHERE (';
					$ccfirst = true;
					foreach ($config['levelmapping'] as $mapping) {
						if ($mapping[0] == $w) {
							if (!$ccfirst) $condition .= ' OR ';
							$ccfirst = false;
							$condition .= 'level_highscores.levelid LIKE \'' . $mapping[1] . '\'';
						}
					}
					if ($ccfirst) $condition .= '0=1';
					$condition .= ') ';

					$stmt = $pdo->prepare(loadReplSQL('get-ranking_local_playerrank', '#$$CONDITION$$', $condition));
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

    <script type="text/javascript">
        function ShowExpandedColumn(id, text) {
            $(".tab_prev").css("visibility", "collapse");
            $("#td_prev_"+id).html(text);
            $("#tr_prev_"+id).css("visibility", "visible");
        }
    </script>

    <h2>Scores</h2>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered">
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
							<?php echo (int)substr($entry['levelid'], 25, 6) . " - " . (int)substr($entry['levelid'], 31, 6); ?>
                        </a>
                    </td>
                    <td><?php echo $entry['difficulty']; ?></td>
                    <td title="<?php echo $entry['best_time']; ?>ms" ><?php echo gmdate("H:i:s", $entry['best_time']/1000.0); ?></td>
                    <td><?php echo $entry['last_changed']; ?></td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>

</body>
</html>