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
	    echo "<a href='#' onclick='ShowExpandedColumn(" . $previd . ", " . str_replace('\n', '<br/>', json_encode($txt)) . ");return false;'>show</a>";
		echo "</td>";
    }
    ?>

    <div class="tablebox">
        <table class="sqltab pure-table pure-table-bordered">
            <thead>
                <tr>
                    <th style='width: 170px'>Username</th>
                    <th>Password</th>
                    <th>Anon</th>
                    <th>Score</th>
                    <th>Rev ID</th>
                    <th style='width: 200px'>Devicename</th>
                    <th style='width: 350px'>Deviceversion</th>
                    <th style='width: 170px'>Last Online</th>
                    <th>App Version</th>

                </tr>
            </thead>
            <?php foreach (getUsers() as $entry): ?>
                <tr>
                    <td><?php echo $entry['username'] . " (" . $entry['userid'] . ")" ; ?></td>
					<?php expansioncell($entry['password_hash']); ?>
                    <td><?php echo $entry['is_auto_generated']; ?></td>
                    <td><?php echo $entry['score']; ?></td>
                    <td><?php echo $entry['revision_id']; ?></td>
                    <td><?php echo $entry['creation_device_name']; ?></td>
                    <td><?php echo $entry['creation_device_version']; ?></td>
                    <td><?php echo $entry['last_online']; ?></td>
                    <td><?php echo $entry['last_online_app_version']; ?></td>
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