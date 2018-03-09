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
			Total Users:&nbsp;<a href="userlist.php"><?php echo getUserCount(); ?></a>&nbsp;&nbsp;(<a href="statistics.php">stats</a>)
		</div>
		<div class="infodiv">
            Active Users:&nbsp;<a href="userlist.php?d=1"><?php echo getActiveUserCount(1); ?></a>&nbsp;|&nbsp;<a href="userlist.php?d=7"><?php echo getActiveUserCount(7); ?></a>
		</div>
		<?php if (isProxyActive()): ?>
        <div class="infodiv">
            Online Sessions:&nbsp;<a href="proxylist.php"><?php echo getSessionCount(); ?></a>
        </div>
		<?php else: ?>
        <div class="infodiv" style="background: #FF4444">
            PROXY INACTIVE
        </div>
		<?php endif; ?>
	</div>

	<div class="infocontainer">
		<div class="infodiv">
			Errors:
			&nbsp;
			<a href="errorlist.php?filter=1&version=<?php global $config; echo $config['latest_version']; ?>">
				<?php global $config; echo countErrors($config['latest_version'], true, ""); ?>
			</a>
			&nbsp;/&nbsp;
			<a href="errorlist.php?filter=1">
				<?php global $config; echo countErrors("", true, ""); ?>
			</a>
			&nbsp;/&nbsp;
			<a href="errorlist.php">
				<?php echo countErrors("", false, ""); ?>
			</a>
		</div>
		<div class="infodiv">
			Entries:&nbsp;<a href="entrylist.php"><?php echo guessEntryCount(); ?></a>
		</div>
        <div class="infodiv">
            Highscore:&nbsp;<?php echo getTotalHighscore(); ?>&nbsp;(<?php echo countFirstPlaceUsers(); ?>)
        </div>
	</div>

	<div class="infocontainer">
        <div class="infodiv">
            <?php if ($config['debug']): ?> 
                Cron:&nbsp;<?php echo getLastCronTime(); ?> ( <a href="<?php global $config; echo ("../cron.php?cronsecret=" . $config['cron-secret']); ?>">Now</a> )
            <?php else: ?> 
                Cron:&nbsp;<?php echo getLastCronTime(); ?>
            <?php endif; ?> 
        </div>
        <div class="infodiv">
            Requests (1d):&nbsp;<?php echo getLastRunLogCount(); ?>&nbsp;(=&nbsp;<a href="runlogoverview.php"><?php echo round(getLastTimingAverage()/(1000.0*1000.0), 4); ?>s</a>)
        </div>
	</div>

    <div class="infocontainer">
        <div class="infodiv">
            New (today): <?php echo getNewUsersToday(); ?>
        </div>
        <div class="infodiv">
            Purchases: <?php echo getPuchaseDelta(); ?>
        </div>
        <div class="infodiv">
            Unlocks: <?php echo getUnlockDelta(); ?>
        </div>
    </div>
    <div class="infocontainer">
		<div class="infodiv">
			Userlevels:&nbsp;<a href="sccmlist.php"><?php echo GetSCCMLevelCount(); ?>/<?php echo GetSCCMLevelCountAll(); ?></a>
		</div>
		<div class="infodiv">
			Userlevels (size):&nbsp;<?php echo formatSizeUnits(GetSCCMLevelSize()); ?></a>
		</div>
    </div>


    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">New errors</h2>

        <table class="sqltab pure-table pure-table-bordered">
            <thead>
                <tr>
                    <th>id</th>
                    <th style='width: 250px'>Username</th>
                    <th>version</th>
                    <th style="width: 225px;">exception id</th>
                    <th>msg</th>
                    <th>trace</th>
                    <th style='width: 160px'>timestamp</th>
                    <th>additional info</th>
                </tr>
            </thead>
            <?php foreach (getNewErrorsOverview() as $entry): ?>
                <tr>
                    <td><a href="errorinfo.php?id=<?php echo $entry['error_id']; ?>"><?php echo $entry['error_id']; ?></a></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['app_version']; ?></td>
                    <td><?php echo $entry['exception_id']; ?></td>
					<?php expansioncell($entry['exception_message']); ?>
					<?php expansioncell($entry['exception_stacktrace']); ?>
                    <td style='max-width: 256px'><?php echo $entry['timestamp']; ?></td>
					<?php expansioncell($entry['additional_info']); ?>
                </tr>
                <tr class='tab_prev' id='tr_prev_<?php echo $previd; ?>'><td colspan='12' id='td_prev_<?php echo $previd; ?>' style='text-align: left;' ></td></tr>
                <?php $previd++; ?>
            <?php endforeach; ?>
        </table>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Logs</h2>

        <table class="sqltab pure-table pure-table-bordered" >
            <thead>
            <tr>
                <th style='width: 300px'>Logfile</th>
                <th style='width: 200px'>Changedate</th>
                <th style='width: 100px'>Entries</th>
                <th style='width: 100px'>Size</th>
                <th style='width: 100px'>Content</th>
                <th style='width: 200px'>Tasks</th>
            </tr>
            </thead>
			<?php foreach (listLogFiles() as $entry): ?>
                <tr>
                    <td title="<?php echo $entry['path']; ?>" ><?php echo $entry['name']; ?></td>
                    <td><?php echo $entry['changedate']; ?></td>
                    <td><?php echo lc($entry['content']); ?></td>
                    <td title="<?php echo $entry['size'] . " byte"; ?>" ><?php echo formatSizeUnits($entry['size']); ?></td>
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

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Highscore</h2>

        <div>

            <h4 class="subhshdr"><a href="worldhighscore.php?id=*">Global</a></h4>
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
					<?php $i=1; foreach (getGlobalHighscores(10) as $entry): ?>
                        <tr>
                            <td><?php echo $i++; ?></td>
                            <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                            <td><?php echo $entry['totalscore']; ?></td>
                            <td title="<?php echo $entry['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", $entry['totaltime']/1000.0); ?></td>
                        </tr>
					<?php endforeach; ?>
                </table>
            </div>

            <?php global $config; foreach (array_unique(array_map(function($k){ return $k[0]; }, $config['levelmapping'])) as $w): ?>
                <?php global $config; if ($w != $config['worldid_0']): ?>
                    <h4 class="subhshdr"><a href="worldhighscore.php?id=<?php echo $w; ?>"><?php echo $w; ?></a></h4>
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
                            <?php $i=1; foreach (getWorldHighscores($w, 10) as $entry): ?>
                                <tr>
                                    <td><?php echo $i++; ?></td>
                                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                                    <td><?php echo $entry['totalscore']; ?></td>
                                    <td title="<?php echo $entry['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", $entry['totaltime']/1000.0); ?></td>
                                </tr>
                            <?php endforeach; ?>
                        </table>
                    </div>
                <?php endif; ?>
            <?php endforeach; ?>

            <h4 class="subhshdr"><a href="worldhighscore.php?id=@">Multiplayer</a></h4>
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
					<?php $i=1; foreach (getMultiplayerHighscores(10) as $entry): ?>
                        <tr>
                            <td><?php echo $i++; ?></td>
                            <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                            <td><?php echo $entry['totalscore']; ?></td>
                            <td title="<?php echo $entry['totaltime']; ?>ms" ><?php echo gmdate("H:i:s", $entry['totaltime']/1000.0); ?></td>
                        </tr>
					<?php endforeach; ?>
                </table>
            </div>

            <h4 class="subhshdr"><a href="worldhighscore.php?id=$">Stars</a></h4>
            <div class="tablebox">
                <table class="sqltab pure-table pure-table-bordered sortable">
                    <thead>
                    <tr>
                        <th>Row</th>
                        <th style='width: 250px'>Username</th>
                        <th>Score</th>
                    </tr>
                    </thead>
					<?php $i=1; foreach (getStarsHighscores(10) as $entry): ?>
                        <tr>
                            <td><?php echo $i++; ?></td>
                            <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                            <td><?php echo $entry['totalscore']; ?></td>
                        </tr>
					<?php endforeach; ?>
                </table>
            </div>

            <h4 class="subhshdr"><a href="worldhighscore.php?id=#">SCCM</a></h4>
            <div class="tablebox">
                <table class="sqltab pure-table pure-table-bordered sortable">
                    <thead>
                    <tr>
                        <th>Row</th>
                        <th style='width: 250px'>Username</th>
                        <th>Score</th>
                    </tr>
                    </thead>
					<?php $i=1; foreach (getSCCMHighscores(10) as $entry): ?>
                        <tr>
                            <td><?php echo $i++; ?></td>
                            <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                            <td><?php echo $entry['totalscore']; ?></td>
                        </tr>
					<?php endforeach; ?>
                </table>
            </div>
        </div>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Level highscores</h2>

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

    <?php printSQLStats(); ?>
</body>
</html>