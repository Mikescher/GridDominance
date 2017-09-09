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

	<?php if (empty($_GET["filter"])) $filter="0"; else $filter=$_GET["filter"] ?>

    <div class="infocontainer">
        <div class="infodiv">
            New Errors: <?php echo getRemainingErrorCount(); ?>
        </div>
        <div class="infodiv">
            All Errors: <?php echo getErrorCount(); ?>
        </div>
    </div>


    <div class="tablebox" data-collapse>

        <h2 class="open collapseheader">Overview [+/-]</h2>

        <table class="sqltab pure-table pure-table-bordered">
            <thead>
            <tr>
                <th style='width: 300px'>error id</th>
                <th style='width: 110px'>Count (All)</th>
                <th style='width: 110px'>Count (New)</th>
            </tr>
            </thead>
			<?php $e2 = ($filter == 1) ? getNewErrorOverviewByID() : getErrorOverviewByID(); foreach ($e2 as $entry): ?>
            <tr>
                <td><?php echo $entry['exception_id']; ?></td>
                <td><?php echo $entry['count_all']; ?></td>
                <td><?php echo $entry['count_noack']; ?></td>
            </tr>
			<?php endforeach; ?>
        </table>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Errors [+/-]</h2>

        <div  class="tablebox">
            <h2 style="padding-top: 5px;">
				<?php if ($filter != 1): ?>
                    <a href="errorlist.php?filter=1">[Show only new]</a>
				<?php else: ?>
                    <a href="errorlist.php?filter=0">[Show all]</a>
				<?php endif; ?>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <a href="ack.php?sim=99&all=true">[Acknowledge all]</a>
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
				<?php $e = ($filter == 1) ? getRemainingErrors() : getAllErrors(); foreach ($e as $entry): ?>
					<?php
					if ($entry['acknowledged'] == 1 || $filter == 1) echo "<tr>"; else echo "<tr style=\"background-color: lightsalmon\">";
					?>
                    <td><?php echo $entry['error_id']; ?></td>
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
						echo " <a href=\"ack.php?sim=1&id=" . $entry['error_id'] . "\">(ack)</a>";
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
        <?php echo file_get_contents('jquery.collapse.js'); ?>
    </script>

    <script src="sorttable.js"></script>

</body>
</html>