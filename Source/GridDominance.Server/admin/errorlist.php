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
            <?php foreach (getAllErrors() as $entry): ?>
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


</body>
</html>