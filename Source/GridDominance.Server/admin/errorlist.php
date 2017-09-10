<?php require_once '../internals/backend.php'; ?>
<?php require_once '../internals/utils.php'; ?>
<?php init("admin"); ?>
<!doctype html>

<html lang="en">
<head>
	<meta charset="utf-8">
    <link rel="stylesheet" href="pure-min.css"/>
	<link rel="stylesheet" type="text/css" href="admin.css">
    <link href="toastr.min.css" rel="stylesheet"/>
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

	<?php if (empty($_GET["filter"])) $filter="0"; else $filter=$_GET["filter"] ?>
	<?php if (empty($_GET["version"])) $versionfilter=""; else $versionfilter=$_GET["version"] ?>

    <div class="infocontainer">
        <div class="infodiv">
            New Errors: <?php echo getRemainingErrorCount(); ?>
        </div>
        <div class="infodiv">
            All Errors: <?php echo getErrorCount(); ?>
        </div>
    </div>

	<?php

	$filtered_errors = ($filter == 1) ? getRemainingErrors($versionfilter) : getAllErrors($versionfilter);

	$filtercount = [];

	foreach ($filtered_errors as $entry) {
		if (array_key_exists($entry['exception_id'], $filtercount))
			$filtercount[$entry['exception_id']]++;
		else
			$filtercount[$entry['exception_id']] = 1;
	}

	?>

    <div class="tablebox" data-collapse>

        <h2 class="open collapseheader">Overview [+/-]</h2>

        <table class="sqltab pure-table pure-table-bordered">
            <thead>
            <tr>
                <th style='width: 300px'>error id</th>
                <th style='width: 110px'>Count (All)</th>
                <th style='width: 150px'>Count (Non Ack)</th>
                <th style='width: 150px'>Count (Filtered)</th>
            </tr>
            </thead>
			<?php $e2 = ($filter == 1) ? getNewErrorOverviewByID() : getErrorOverviewByID(); foreach ($e2 as $entry): ?>
            <tr>
                <td><?php echo $entry['exception_id']; ?></td>
                <td><?php echo $entry['count_all']; ?></td>
                <td><?php echo $entry['count_noack']; ?></td>
                <td><?php echo array_key_exists($entry['exception_id'], $filtercount) ? $filtercount[$entry['exception_id']] : "0"; ?></td>
            </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Errors [+/-]</h2>

        <div  class="tablebox">
            <h2 style="padding-top: 5px;">
				<?php
                if ($filter != 1){
                    echo " <a href=\"errorlist.php?filter=1&version=$versionfilter\">[Show only new]</a>";
				} else {
                    echo "<a href=\"errorlist.php\">[Show all]</a>";
                }

                echo "&nbsp;&nbsp;&nbsp;&nbsp;";

                echo " <a href=\"ack.php?sim=99&all=true\">[Acknowledge all]</a>";

				echo "&nbsp;&nbsp;&nbsp;&nbsp;";

				if (!$versionfilter) {
					global $pdo;
					$latestversion = $pdo->query('SELECT error_log.app_version FROM error_log ORDER BY error_log.app_version DESC LIMIT 1')->fetch(PDO::FETCH_NUM)[0];
					echo "&nbsp;&nbsp;&nbsp;&nbsp;";
					echo "<a href=\"errorlist.php?filter=$filter&version=$latestversion\">[Show only latest version]</a>";
				}

				?>
            </h2>

            <table class="sqltab pure-table pure-table-bordered">
                <thead>
                <tr>
                    <th>error id</th>
                    <th style='width: 170px'>username</th>
                    <th>anon</th>
                    <th>resolution</th>
                    <th>version</th>
                    <th style="width: 200px;">exception id</th>
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
                    <td><?php echo $entry['password_verified']?0:1; ?></td>
					<?php expansioncell($entry['screen_resolution']); ?>
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
						echo " <a href=\"#\" onclick='HideExpandedColumn(" . $previd . ");AjaxAck(".$entry['error_id'].");return false;'>(ack)</a>";
						echo "<br/>";
						echo " <a href=\"ack.php?sim=2&exid=" . urlencode($entry['exception_id']) . "\">(ack similiar (id))</a>";
						echo "<br/>";
						echo " <a href=\"ack.php?sim=3&exid=" . urlencode($entry['exception_id']) . "&exmsg=" . urlencode($entry['exception_message']) . "\">(ack similiar (id+msg))</a>";
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
        <div>

    </div>

    <script type="text/javascript">
		<?php echo file_get_contents('admin.js'); ?>
    </script>

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

    <script src="jquery.collapse.js"></script>
    <script src="toastr.min.js"></script>
    <script src="sorttable.js"></script>

</body>
</html>