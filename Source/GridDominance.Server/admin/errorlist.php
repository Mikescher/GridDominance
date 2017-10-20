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
    global $config;

    if (empty($_GET["filter"]))  $filter        ="0"; else $filter        = $_GET["filter"];
    if (empty($_GET["version"])) $versionfilter = ""; else $versionfilter = $_GET["version"];
    if (empty($_GET["page"]))    $page          = 0;  else $page          = $_GET["page"];
    if (empty($_GET["eid"]))     $idfilter      = ""; else $idfilter      = $_GET["eid"];

	$filtered_errors =   getErrors($versionfilter, ($filter == 1), $idfilter, $page, 1000);
    $entrycount      = countErrors($versionfilter, ($filter == 1), $idfilter);
    $entrygroups     = groupErrors($versionfilter, ($filter == 1), $idfilter);

    $overview = getErrorOverviewByID();

	$filtercount = [];
	foreach ($entrygroups as $entry) $filtercount[$entry['exception_id']] = $entry['count'];

	$latestversion = $config['latest_version'];
	?>

    <div class="infocontainer">
        <div class="infodiv">
            All Errors: <?php echo countErrors("", false, ""); ?>
        </div>
        <div class="infodiv">
            New Errors: <?php echo countErrors("", true, ""); ?>
        </div>
        <div class="infodiv">
            Filtered Errors: <?php echo $entrycount; ?>
        </div>
    </div>

    <div class="tablebox" data-collapse>

        <h2 class="open collapseheader">Overview</h2>

        <table class="sqltab pure-table pure-table-bordered sortable">
            <thead>
            <tr>
                <th style='width: 300px'>error id</th>
                <th style='width: 110px'>Count (All)</th>
                <th style='width: 150px'>Count (Non Ack)</th>
                <th style='width: 150px'>Count (Filtered)</th>
            </tr>
            </thead>
			<?php foreach ($overview as $entry): ?>
            <tr class="<?php if (array_key_exists($entry['exception_id'], $filtercount)) if ($filtercount[$entry['exception_id']] > 0) echo 'td_highlight'; ?>">
                <td><a href="<?php echo suffixGetParams('eid', $entry['exception_id']); ?>"><?php echo $entry['exception_id']; ?></a></td>
                <td><?php echo $entry['count_all']; ?></td>
                <td><?php echo $entry['count_noack']; ?></td>
                <td><?php echo array_key_exists($entry['exception_id'], $filtercount) ? $filtercount[$entry['exception_id']] : "0"; ?></td>
            </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Errors</h2>

        <div  class="tablebox">
            <div class="filterlinkrow">
                <?php if ($filter != 1): ?>
                    <a href="<?php echo suffixGetParams('filter', '1'); ?>">[Show only NoAck]</a>
				<?php else: ?>
                    <a href="<?php echo suffixGetParams('filter', '0'); ?>">[Show also Ack]</a>
				<?php endif; ?>

                <?php if ($versionfilter != $latestversion): ?>
                    <a href="<?php echo suffixGetParams('version', $latestversion); ?>">[Show only latest]</a>
				<?php else: ?>
                    <a href="<?php echo suffixGetParams('version', ''); ?>">[Show all versions]</a>
                <?php endif; ?>
                
                <?php if ($idfilter != ""): ?>
                    <a href="<?php echo suffixGetParams('idfilter', ''); ?>">[Show all IDs]</a>
                <?php endif; ?>
                
                <a href="ack.php?sim=99&all=true">[Acknowledge all]</a>
            </div>

            <table class="sqltab pure-table pure-table-bordered">
                <thead>
                <tr>
                    <th>error id</th>
                    <th style='width: 210px'>username</th>
                    <th>version</th>
                    <th style="width: 225px;">exception id</th>
                    <th>msg</th>
                    <th>trace</th>
                    <th style='width: 160px'>timestamp</th>
                    <th>additional info</th>
                    <th style='width: 160px'>acknowledged</th>
                </tr>
                </thead>
				<?php foreach ($filtered_errors as $entry): ?>
					<?php
					if ($entry['acknowledged'] == 1 || $filter == 1) echo "<tr id=\"err_row_".$entry['error_id']."\">"; else echo "<tr id=\"err_row_".$entry['error_id']."\" style=\"background-color: lightsalmon\">";
					?>
                    <td><a href="errorinfo.php?id=<?php echo $entry['error_id']; ?>"><?php echo $entry['error_id']; ?></a></td>
                    <td><a href="userinfo.php?id=<?php echo $entry['userid']; ?>"><?php echo $entry['username']; ?></a> (<?php echo $entry['userid']; ?>)</td>
                    <td><?php echo $entry['app_version']; ?></td>
                    <td><?php echo $entry['exception_id']; ?></td>
					<?php expansioncell($entry['exception_message']); ?>
					<?php expansioncell($entry['exception_stacktrace']); ?>
                    <td><?php echo $entry['timestamp']; ?></td>
					<?php expansioncell($entry['additional_info']); ?>
					<?php

					if ($entry['acknowledged'] == 0)
					{
						echo "<td>";
						echo $entry['acknowledged'] . " ";
						echo " <a href=\"#\" onclick='HideExpandedColumn(" . $previd . ");AjaxAck(".$entry['error_id'].");return false;'>(ack (this))</a>";
						echo "<br/>";
						echo " <a href=\"ack.php?sim=2&version=".urlencode($entry['app_version'])."&exid=" . urlencode($entry['exception_id']) . "\">(ack (v+id))</a>";
						echo "<br/>";
						echo " <a href=\"ack.php?sim=3&version=".urlencode($entry['app_version'])."&exid=" . urlencode($entry['exception_id']) . "&exmsg=" . urlencode($entry['exception_message']) . "\">(ack (v+id+msg))</a>";
						echo "</td>";
					}
					else
					{
						echo "<td>" . $entry['acknowledged'] . "</td>";
					}
					?>
                    </tr>
                    <tr class='tab_prev' id='tr_prev_<?php echo $previd; ?>'><td colspan='12' id='td_prev_<?php echo $previd; ?>' style='text-align: left;' ></td></tr>
					<?php $previd++; ?>
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
        <div>

    </div>

    <?php printSQLStats(); ?>

    <script type="text/javascript">
        function AjaxAck(id) {
            $.get('ack.php?sim=1&nojs=1&id=' + id, function( data ) {
                $("#td_prev_"+id).html(data);
                <?php if ($filter == 1): ?>
                    $("#err_row_"+id).css("visibility", "collapse");
                    $("#err_row_"+id).css("display", "none");
				<?php else: ?>
                    $("#err_row_"+id).css("background-color", "unset");
				<?php endif; ?>

                toastr.success(data, 'Acknowledged')
            });
        }
    </script>

</body>
</html>