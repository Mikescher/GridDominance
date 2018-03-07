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
		$showall = false;
		if (! empty($_GET['a']) && $_GET['a'] == 'y') $showall = true;
		if (! empty($_GET['a']) && $_GET['a'] == 'n') $showall = false;

		$page = 0;
		if (!empty($_GET['page'])) $page = $_GET['page'];


		$entrycount = $showall ? GetSCCMLevelCountAll() : GetSCCMLevelCount();
	 ?>

    <div class="infocontainer">
        <div class="infodiv">
            Levels (uploaded): <?php echo GetSCCMLevelCount(); ?>
        </div>
        <div class="infodiv">
            Levels (created): <?php echo GetSCCMLevelCountAll(); ?>
        </div>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Linked Entries</h2>
		<div>
            <div class="filterlinkrow">
                <?php if (! $showall): ?>
                    <a href="<?php echo suffixGetParams('a', 'y'); ?>">[Show All]</a>
                <?php else: ?>
                    <a href="<?php echo suffixGetParams('a', 'n'); ?>">[Show only uploaded]</a>
                <?php endif; ?>
            </div>
			<table class="sqltab pure-table pure-table-bordered sortable">
				<thead>
				<tr>
					<th>ID</th>
					<th>Author</th>
					<th>Name</th>
					<th>Stars</th>
					<th>Hot</th>
					<th>Uploadtime</th>
					<th>Completed</th>
					<th>Played</th>
					<th>Highscore (D0)</th>
					<th>Highscore (D1)</th>
					<th>Highscore (D2)</th>
					<th>Highscore (D3)</th>
				</tr>
				</thead>

					<?php foreach(GetSCCMLevelByNew(500, $page, $showall) as $entry): ?>
					
						<tr>
							<td><a href="sccmlevelinfo.php?id=<?php echo $entry['id'] ?>"><?php echo $entry['id']; ?></a></td>
							<td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
							<td><a href="sccmlevelinfo.php?id=<?php echo $entry['id'] ?>"><?php echo $entry['name']; ?></a></td>
							<td><?php echo $entry['stars']; ?></td>
							<td><?php echo $entry['hot_ranking']; ?></td>
							<td><?php echo $entry['upload_timestamp']; ?></td>
							<td><?php echo ($entry['d0_completed']+$entry['d1_completed']+$entry['d2_completed']+$entry['d3_completed']); ?></td>
							<td><?php echo ($entry['d0_played']+$entry['d1_played']+$entry['d2_played']+$entry['d3_played']); ?></td>
							<td title="<?php echo $entry['d0_besttime']; ?> @ <?php echo $entry['d0_besttimestamp']; ?>" ><a href="userinfo.php?id=<?php echo $entry['d0_bestuserid']; ?>"><?php echo $entry['d0_bestusername']; ?></a> (<?php echo $entry['d0_bestuserid']; ?>)</td>
							<td title="<?php echo $entry['d1_besttime']; ?> @ <?php echo $entry['d1_besttimestamp']; ?>" ><a href="userinfo.php?id=<?php echo $entry['d1_bestuserid']; ?>"><?php echo $entry['d1_bestusername']; ?></a> (<?php echo $entry['d1_bestuserid']; ?>)</td>
							<td title="<?php echo $entry['d2_besttime']; ?> @ <?php echo $entry['d2_besttimestamp']; ?>" ><a href="userinfo.php?id=<?php echo $entry['d2_bestuserid']; ?>"><?php echo $entry['d2_bestusername']; ?></a> (<?php echo $entry['d2_bestuserid']; ?>)</td>
							<td title="<?php echo $entry['d3_besttime']; ?> @ <?php echo $entry['d3_besttimestamp']; ?>" ><a href="userinfo.php?id=<?php echo $entry['d3_bestuserid']; ?>"><?php echo $entry['d3_bestusername']; ?></a> (<?php echo $entry['d3_bestuserid']; ?>)</td>
						</tr>

					<?php endforeach; ?>
			</table>
            <div class="pagination_row">
                <?php for ($i=0; $i < ceil($entrycount/500); $i++ ): ?>
                    <?php if ($i != $page): ?>
                        <a class="pagination_link" href="<?php echo suffixGetParams('page', $i); ?>"><?php echo ($i+1); ?></a>
                    <?php else: ?>
                        <a class="pagination_curr"><?php echo ($i+1); ?></a>
                    <?php endif; ?>
                <?php endfor; ?>
            </div>
		</div>
    </div>

    <?php printSQLStats(); ?>
</body>
</html>