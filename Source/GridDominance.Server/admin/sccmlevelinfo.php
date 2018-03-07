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

    <h1><a href="index.php">Cannon Conquest | Admin Page</a></h1>4
	
    <?php
        global $pdo;
        
		$levelid = $_GET['id'];

        $level = GetSCCMLevelInfo($levelid);
		$recalc = GetSCCMLevelMetadataRecalculated($levelid);

	    $levelid = "{B16B00B5-0001-4001-0000-".str_pad(strtoupper(dechex($levelid)), 12, '0', STR_PAD_LEFT).'}';
        $level['filepath'] = $config['userlevel_directory'].$levelid;
	    $content = file_get_contents($level['filepath']);
        $recalc['filesize'] = strlen($content);
        $recalc['datahash'] = strtoupper(hash('sha256', $content))
    ?>

    <div class="infocontainer">
        <div class="infodiv">
            ID: {B16B00B5-0001-4001-0000-<?php echo $level['id']; ?>}
        </div>
        <div class="infodiv">
            Name: <?php echo $level['name']; ?>
        </div>
        <div class="infodiv">
            Stars: <?php echo $level['stars']; ?>
        </div>
        <div class="infodiv">
            Hot: <?php echo $level['hot_ranking']; ?>
        </div>
    </div>

    <div class="infocontainer">
        <div class="infodiv">
            Author: <a href="userinfo.php?id=<?php echo $level['userid']; ?>"><?php echo $level['username']; ?></a> (<?php echo $level['userid']; ?>)
        </div>
        <div class="infodiv">
            Hash: <?php echo $level['datahash']; ?>
        </div>
        <div class="infodiv">
            Filesize: <?php echo formatSizeUnits($level['filesize']); ?>byte
        </div>
    </div>

    <div class="infocontainer">
        <div class="infodiv">
            Created at: <?php echo $level['creation_timestamp']; ?>
        </div>
        <div class="infodiv">
            Uploaded at: <?php echo $level['upload_timestamp']; ?>
        </div>
        <div class="infodiv">
            Uploaded version: <?php echo $level['upload_version']; ?> (<?php echo $level['upload_decversion']; ?>)
        </div>
    </div>

    <div class="infocontainer">
        <div class="infodiv">
            Width: <?php echo $level['grid_width']; ?>
        </div>
        <div class="infodiv">
            Height: <?php echo $level['grid_height']; ?>
        </div>
        <div class="infodiv">
            Author time: <?php echo gmdate("H:i:s", $level['author_time']/1000.0); ?>
        </div>
    </div>
	
    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Metainformation</h2>

        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th style='width: 170px'>Key</th>
                <th>Value</th>
                <th>Value (Recalculated)</th>
            </tr>
            </thead>

                <tr class="<?php if ($level['stars'] != $recalc['stars']) echo "td_err"; ?>">
					<td>Stars</td>
					<td><?php echo $level['stars']; ?></td>
					<td><?php echo $recalc['stars']; ?></td>
				</tr>

                <tr class="<?php if ($level['d0_completed'] != $recalc['d0_completed']) echo "td_err"; ?>">
					<td>Count completed (Diff_0)</td>
					<td><?php echo $level['d0_completed']; ?></td>
					<td><?php echo $recalc['d0_completed']; ?></td>
				</tr>
                <tr class="<?php if ($level['d0_played'] != $recalc['d0_played']) echo "td_err"; ?>">
					<td>Count Played (Diff_0)</td>
					<td><?php echo $level['d0_played']; ?></td>
					<td><?php echo $recalc['d0_played']; ?></td>
				</tr>
                <tr class="<?php if ($level['d0_bestuserid'] != $recalc['d0_bestuserid']) echo "td_err"; ?>">
					<td>BestUserID (Diff_0)</td>
					<td><a href="userinfo.php?id=<?php echo $level['d0_bestuserid']; ?>"><?php echo getUsernameOrEmpty($level['d0_bestuserid']); ?></a> (<?php echo $level['d0_bestuserid']; ?>)</td>
					<td><?php echo $recalc['d0_bestuserid']; ?></td>
				</tr>
                <tr>
					<td>BestTime (Diff_0)</td>
					<td title="<?php echo $entry['d0_besttime']; ?>ms @ <?php echo $entry['d0_besttimestamp']; ?>" ><?php echo gmdate("H:i:s", $entry['d0_besttime']/1000.0); ?></td>
					<td></td>
				</tr>

                <tr class="<?php if ($level['d1_completed'] != $recalc['d1_completed']) echo "td_err"; ?>">
					<td>Count completed (Diff_1)</td>
					<td><?php echo $level['d1_completed']; ?></td>
					<td><?php echo $recalc['d1_completed']; ?></td>
				</tr>
                <tr class="<?php if ($level['d1_played'] != $recalc['d1_played']) echo "td_err"; ?>">
					<td>Count Played (Diff_1)</td>
					<td><?php echo $level['d1_played']; ?></td>
					<td><?php echo $recalc['d1_played']; ?></td>
				</tr>
                <tr class="<?php if ($level['d1_bestuserid'] != $recalc['d1_bestuserid']) echo "td_err"; ?>">
					<td>BestUserID (Diff_1)</td>
					<td><a href="userinfo.php?id=<?php echo $level['d1_bestuserid']; ?>"><?php echo getUsernameOrEmpty($level['d1_bestuserid']); ?></a> (<?php echo $level['d1_bestuserid']; ?>)</td>
					<td><?php echo $recalc['d1_bestuserid']; ?></td>
				</tr>
                <tr>
					<td>BestTime (Diff_1)</td>
					<td title="<?php echo $entry['d1_besttime']; ?>ms @ <?php echo $entry['d1_besttimestamp']; ?>" ><?php echo gmdate("H:i:s", $entry['d1_besttime']/1000.0); ?></td>
					<td></td>
				</tr>

                <tr class="<?php if ($level['d2_completed'] != $recalc['d2_completed']) echo "td_err"; ?>">
					<td>Count completed (Diff_2)</td>
					<td><?php echo $level['d2_completed']; ?></td>
					<td><?php echo $recalc['d2_completed']; ?></td>
				</tr>
                <tr class="<?php if ($level['d2_played'] != $recalc['d2_played']) echo "td_err"; ?>">
					<td>Count Played (Diff_2)</td>
					<td><?php echo $level['d2_played']; ?></td>
					<td><?php echo $recalc['d2_played']; ?></td>
				</tr>
                <tr class="<?php if ($level['d2_bestuserid'] != $recalc['d2_bestuserid']) echo "td_err"; ?>">
					<td>BestUserID (Diff_2)</td>
					<td><a href="userinfo.php?id=<?php echo $level['d2_bestuserid']; ?>"><?php echo getUsernameOrEmpty($level['d2_bestuserid']); ?></a> (<?php echo $level['d2_bestuserid']; ?>)</td>
					<td><?php echo $recalc['d2_bestuserid']; ?></td>
				</tr>
                <tr>
					<td>BestTime (Diff_2)</td>
					<td title="<?php echo $entry['d2_besttime']; ?>ms @ <?php echo $entry['d2_besttimestamp']; ?>" ><?php echo gmdate("H:i:s", $entry['d2_besttime']/1000.0); ?></td>
					<td></td>
				</tr>

                <tr class="<?php if ($level['d3_completed'] != $recalc['d3_completed']) echo "td_err"; ?>">
					<td>Count completed (Diff_3)</td>
					<td><?php echo $level['d3_completed']; ?></td>
					<td><?php echo $recalc['d3_completed']; ?></td>
				</tr>
                <tr class="<?php if ($level['d3_played'] != $recalc['d3_played']) echo "td_err"; ?>">
					<td>Count Played (Diff_3)</td>
					<td><?php echo $level['d3_played']; ?></td>
					<td><?php echo $recalc['d3_played']; ?></td>
				</tr>
                <tr class="<?php if ($level['d3_bestuserid'] != $recalc['d3_bestuserid']) echo "td_err"; ?>">
					<td>BestUserID (Diff_3)</td>
					<td><a href="userinfo.php?id=<?php echo $level['d3_bestuserid']; ?>"><?php echo getUsernameOrEmpty($level['d3_bestuserid']); ?></a> (<?php echo $level['d3_bestuserid']; ?>)</td>
					<td><?php echo $recalc['d3_bestuserid']; ?></td>
				</tr>
                <tr>
					<td>BestTime (Diff_3)</td>
					<td title="<?php echo $entry['d3_besttime']; ?>ms @ <?php echo $entry['d3_besttimestamp']; ?>" ><?php echo gmdate("H:i:s", $entry['d3_besttime']/1000.0); ?></td>
					<td></td>
				</tr>

                <tr>
					<td>Filepath</td>
					<td><?php echo $level['filepath']; ?></td>
					<td></td>
				</tr>

                <tr class="<?php if ($level['datahash'] != $recalc['datahash']) echo "td_err"; ?>">
					<td>Filehash</td>
					<td><?php echo $level['datahash']; ?></td>
					<td><?php echo $recalc['datahash']; ?></td>
				</tr>
        </table>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Content</h2>

        <div style="width:100px; word-wrap:break-word; display:inline-block;background:lightgray;">
            <?php echo base64_encode($content) ?>
        </div>

    </div>
	
    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Linked Entries</h2>

        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th >User</th>
                <th>Difficulty 0</th>
                <th>Difficulty 1</th>
                <th>Difficulty 2</th>
                <th>Difficulty 3</th>
                <th>starred</th>
            </tr>
            </thead>

				<?php foreach(getLevelSCCMEntries() as $entry): ?>
				
					<tr>
						<td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
						<td title="<?php echo $level['d0_time']; ?>ms"><?php echo gmdate("H:i:s", $level['d0_time']/1000.0); ?>  ($level['d0_lastplayed'])</td>
						<td title="<?php echo $level['d1_time']; ?>ms"><?php echo gmdate("H:i:s", $level['d1_time']/1000.0); ?>  ($level['d1_lastplayed'])</td>
						<td title="<?php echo $level['d2_time']; ?>ms"><?php echo gmdate("H:i:s", $level['d2_time']/1000.0); ?>  ($level['d2_lastplayed'])</td>
						<td title="<?php echo $level['d3_time']; ?>ms"><?php echo gmdate("H:i:s", $level['d3_time']/1000.0); ?>  ($level['d3_lastplayed'])</td>
						<td><?php echo $entry['starred']?'yes':'no'; ?></td>
					</tr>

				<?php endforeach; ?>
        </table>
    </div>

    <?php printSQLStats(); ?>
</body>
</html>