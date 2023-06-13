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
        global $pdo;
        
        $user = GDUser::QueryByIDOrNull($pdo, $_GET['id']);
        $userdata = getUserData($_GET['id']);
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
    </div>
    <div class="infocontainer">
        <div class="infodiv">
            Score: <?php echo $user->Score; ?>
        </div>
        <div class="infodiv">
            Score(MP): <?php echo $user->MultiplayerScore; ?>
        </div>
        <div class="infodiv">
            Score(SCCM): <?php echo $user->ScoreSCCM; ?>
        </div>
        <div class="infodiv">
            Stars: <?php echo $user->ScoreStars; ?>
        </div>
        <div class="infodiv" title="<?php echo $userdata['unlocked_worlds']; ?>">
            Unlocks: <?php echo lc($userdata['unlocked_worlds']); ?>
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
        <div class="infodiv">
            App Version: <?php echo $userdata['app_version']; ?>
        </div>
        <div class="infodiv">
            App Type: <?php echo $userdata['app_type']; ?>
        </div>
        <div class="infodiv">
            Pings: <?php echo $userdata['ping_counter']; ?>
        </div>
    </div>
    <div class="infocontainer">
        <div class="infodiv">
            Created At: <?php echo $userdata['creation_time']; ?>
        </div>
        <div class="infodiv">
            Last Online: <?php echo $userdata['last_online']; ?>
        </div>
    </div>

    <?php
	global $config;
	global $pdo;

    $manualtimes = getManualRecalculatedUserTimes($user->ID);

	$manual = [];
	foreach ($manualtimes as $row) {
		$manual[$row['worldid']] = $row['best_time'];
	}
    $manual['$'] = getManualRecalculatedScoreStars($user->ID);
    $manual['#'] = getManualRecalculatedScoreSCCM($user->ID);
	?>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Ranking</h2>

        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th style='width: 350px'>World</th>
                <th>Rank</th>
                <th style='width: 250px'>Username</th>
                <th>Total Score</th>
                <th>Total Score (Recalulated)</th>
                <th>Total Time</th>
                <th>Total Time (Recalulated)</th>
            </tr>
            </thead>

			<?php
			global $config;
			global $pdo;

			$entry = getGlobalPlayerRank($user->ID);

			if (count($entry) > 0):
                $entry = $entry[0];
			?>

                <tr class="<?php if ($manual['*'] != $entry['totaltime']) echo "td_err"; ?>">
                    <td>global</td>
                    <td><?php echo $entry['rank']; ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['totalscore']; ?></td>
                    <td></td>
                    <td title="<?php echo $entry['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", (int)($entry['totaltime']/1000.0)); ?></td>
                    <td title="<?php echo $manual['*']; ?>ms" ><?php echo gmdate("H:i:s", (int)($manual['*']/1000.0)); ?></td>
                </tr>

			<?php endif; ?>

			<?php
			global $config;
			global $pdo;

            foreach (array_unique(array_map(function($k){ return $k[0]; }, $config['levelmapping'])) as $w):

                if ($w == $config['worldid_0']) continue;

                $entry = getLocalPlayerRank($user->ID, $w);

                if (count($entry) == 0) continue;

                $entry = $entry[0];

             ?>
                <tr class="<?php if ($manual[$w] != $entry['totaltime']) echo "td_err"; ?>">
                    <td><a href="worldhighscore.php?id=<?php echo $w; ?>"><?php echo $w; ?></a></td>
                    <td><?php echo $entry['rank']; ?></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['totalscore']; ?></td>
                    <td></td>
                    <td title="<?php echo $entry['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", (int)($entry['totaltime']/1000.0)); ?></td>
                    <td title="<?php echo $manual[$w]; ?>ms" ><?php echo gmdate("H:i:s", (int)($manual[$w]/1000.0)); ?></td>
                </tr>
			<?php endforeach; ?>

                <tr class="<?php if ($manual['#'] != $user->ScoreSCCM) echo "td_err"; ?>">
                    <td>SCCM</td>
                    <td><?php echo getSCCMRank($user->ID); ?></td>
                    <td><a href="userinfo.php?id=<?php echo $user->ID; ?>"><?php echo $user->Username; ?></a> (<?php echo $user->ID; ?>)</td>
                    <td><?php echo $user->ScoreSCCM; ?></td>
                    <td><?php echo $manual['#']; ?></td>
                    <td></td>
                    <td></td>
                </tr>
                
                <tr class="<?php if ($manual['$'] != $user->ScoreStars) echo "td_err"; ?>">
                    <td>Stars</td>
                    <td><?php echo getStarsRank($user->ID); ?></td>
                    <td><a href="userinfo.php?id=<?php echo $user->ID; ?>"><?php echo $user->Username; ?></a> (<?php echo $user->ID; ?>)</td>
                    <td><?php echo $user->ScoreStars; ?></td>
                    <td><?php echo $manual['$']; ?></td>
                    <td></td>
                    <td></td>
                </tr>


        </table>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Errors</h2>

        <table class="sqltab pure-table pure-table-bordered">
            <thead>
            <tr>
                <th>error id</th>
                <th style='width: 170px'>username</th>
                <th>app version</th>
                <th style="width: 225px;">exception id</th>
                <th>message</th>
                <th>stacktrace</th>
                <th style='width: 170px'>timestamp</th>
                <th>additional info</th>
                <th>acknowledged</th>
            </tr>
            </thead>
			<?php foreach (getUserErrors($user->ID) as $entry): ?>
                <tr>
                    <td><a href="errorinfo.php?id=<?php echo $entry['error_id']; ?>"><?php echo $entry['error_id']; ?></a></td>
                    <td><?php echo $entry['username']; ?> (<?php echo $entry['userid']; ?>)</td>
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

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Scores</h2>

        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th>World</th>
                <th>Score</th>
                <th>Time</th>
            </tr>
            </thead>
            <tr> <td>Total</td> <td><?php echo $user->Score; ?>           </td> <td title="<?php echo $user->TotalTime; ?>ms" ><?php echo gmdate("H:i:s", (int)($user->TotalTime/1000.0)); ?></td> </tr>
            <tr> <td>W1</td>    <td><?php echo $user->ScoreW1; ?>         </td> <td title="<?php echo $user->TimeW1; ?>ms"    ><?php echo gmdate("H:i:s", (int)($user->TimeW1/1000.0));    ?></td> </tr>
            <tr> <td>W2</td>    <td><?php echo $user->ScoreW2; ?>         </td> <td title="<?php echo $user->TimeW2; ?>ms"    ><?php echo gmdate("H:i:s", (int)($user->TimeW2/1000.0));    ?></td> </tr>
            <tr> <td>W3</td>    <td><?php echo $user->ScoreW3; ?>         </td> <td title="<?php echo $user->TimeW3; ?>ms"    ><?php echo gmdate("H:i:s", (int)($user->TimeW3/1000.0));    ?></td> </tr>
            <tr> <td>W4</td>    <td><?php echo $user->ScoreW4; ?>         </td> <td title="<?php echo $user->TimeW4; ?>ms"    ><?php echo gmdate("H:i:s", (int)($user->TimeW4/1000.0));    ?></td> </tr>
            <tr> <td>MP</td>    <td><?php echo $user->MultiplayerScore; ?></td> <td title="?"></td> </tr>
        </table>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Scores (per level)</h2>

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
                    <td title="<?php echo $entry['best_time']; ?>ms" ><?php echo gmdate("H:i:s", (int)($entry['best_time']/1000.0)); ?></td>
                    <td><?php echo $entry['last_changed']; ?></td>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Scores (per SCCM level)</h2>

        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th>Level</th>
                <th>Difficulty</th>
                <th>Time</th>
                <th style='width: 170px'>Last changed</th>
            </tr>
            </thead>
			<?php foreach (getUserSCCMEntries($user->ID) as $entry): ?>

                <?php if ($entry['d0_lastplayed'] != null): ?>
                    <tr>
                        <td title="<?php echo $entry['levelid']; ?>" >
                            <a href="sccmlevelinfo.php?id=<?php echo $entry['levelid']; ?>">
                                <?php echo fmtLevelID($entry['levelid']); ?>
                            </a>
                        </td>
                        <td>0</td>
                        <td title="<?php echo $entry['d0_time']; ?>ms" ><?php echo gmdate("H:i:s", (int)($entry['d0_time']/1000.0)); ?></td>
                        <td><?php echo $entry['d0_lastplayed']; ?></td>
                    </tr>
                <?php endif; ?> 

                <?php if ($entry['d1_lastplayed'] != null): ?>
                    <tr>
                        <td title="<?php echo $entry['levelid']; ?>" >
                            <a href="sccmlevelinfo.php?id=<?php echo $entry['levelid']; ?>">
                                <?php echo fmtLevelID($entry['levelid']); ?>
                            </a>
                        </td>
                        <td>1</td>
                        <td title="<?php echo $entry['d1_time']; ?>ms" ><?php echo gmdate("H:i:s", (int)($entry['d1_time']/1000.0)); ?></td>
                        <td><?php echo $entry['d1_lastplayed']; ?></td>
                    </tr>
                <?php endif; ?> 

                <?php if ($entry['d2_lastplayed'] != null): ?>
                    <tr>
                        <td title="<?php echo $entry['levelid']; ?>" >
                            <a href="sccmlevelinfo.php?id=<?php echo $entry['levelid']; ?>">
                                <?php echo fmtLevelID($entry['levelid']); ?>
                            </a>
                        </td>
                        <td>2</td>
                        <td title="<?php echo $entry['d2_time']; ?>ms" ><?php echo gmdate("H:i:s", (int)($entry['d2_time']/1000.0)); ?></td>
                        <td><?php echo $entry['d2_lastplayed']; ?></td>
                    </tr>
                <?php endif; ?> 

                <?php if ($entry['d3_lastplayed'] != null): ?>
                    <tr>
                        <td title="<?php echo $entry['levelid']; ?>" >
                            <a href="sccmlevelinfo.php?id=<?php echo $entry['levelid']; ?>">
                                <?php echo fmtLevelID($entry['levelid']); ?>
                            </a>
                        </td>
                        <td>3</td>
                        <td title="<?php echo $entry['d3_time']; ?>ms" ><?php echo gmdate("H:i:s", (int)($entry['d3_time']/1000.0)); ?></td>
                        <td><?php echo $entry['d3_lastplayed']; ?></td>
                    </tr>
                <?php endif; ?> 

			<?php endforeach; ?>
        </table>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Uploads</h2>

        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th>ID</th>
                <th style="width: 200px" >Upload timestamp</th>
                <th style="width: 500px" >Level</th>
                <th>Stars</th>
                <th>Author time</th>
            </tr>
            </thead>
			<?php foreach (GetSCCMLevelByUser($user->ID) as $entry): ?>
                <tr>
                    <td><a href="sccmlevelinfo.php?id=<?php echo $entry['id'] ?>"><?php echo $entry['id'] ?></a></td>
					<?php if ($entry['upload_timestamp'] == null): ?>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                    <?php else: ?>
                        <td><?php echo $entry['upload_timestamp']; ?></td>
                        <td><?php echo htmlspecialchars($entry['name']); ?></td>
                        <td><?php echo $entry['stars']; ?></td>
                        <td><?php echo gmdate("H:i:s", (int)($entry['author_time']/1000.0))?></td>
                    <?php endif ?>
                </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <div class="formbox" data-collapse>
        <h2 class="open collapseheader">Change password</h2>

        <form action="force_change_password.php">
            UserID:<br>
            <input type="text" name="id" value="<?php echo $user->ID; ?>"/><br><br>
            New Password:<br>
            <input type="text" name="newpw"/><br><br>
            <input type="submit" value="Change" />
        </form>
    </div>

    <?php printSQLStats(); ?>
</body>
</html>