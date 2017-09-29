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
        global $pdo;

        $id = $_GET['id'];

        $data = getErrorData($id);
	    $user = GDUser::QueryByIDOrNull($pdo, $data['userid']);
    ?>

    <div class="infocontainer">
        <div class="infodiv">
            Error: <?php echo $id ; ?>
        </div>
        <div class="infodiv">
            UserID: <?php echo $user==null ? $data['userid'] : "<a href='userinfo.php?id=$user->ID'>$user->ID</a>" ; ?>
        </div>
        <div class="infodiv">
            Username: <?php echo $user==null ? "?" : $user->Username; ?>
        </div>
    </div>
    <div class="infocontainer">
        <div class="infodiv">
            ExceptionID: <?php echo $data['exception_id']; ?>
        </div>
        <div class="infodiv">
            Version: <?php echo $data['app_version']; ?>
        </div>
        <div class="infodiv">
            Time: <?php echo $data['timestamp']; ?>
        </div>
    </div>
    <div class="infocontainer">
        <div class="infodiv">
            PW Verified: <?php echo $data['password_verified']; ?>
        </div>
        <div class="infodiv">
            Resolution: <?php echo $data['screen_resolution']; ?>
        </div>
        <div class="infodiv">
            Acknowledged: <?php echo $data['acknowledged']; ?>
            <?php if ($data['acknowledged'] == 0) echo "&nbsp;<a href='ack.php?sim=1&id=$id'>(ack now)</a>" ?>
        </div>
    </div>


    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Message</h2>

        <div class="errordatabox">
            <?php echo nl2br(htmlspecialchars($data['exception_message'])); ?>
        </div>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Stacktrace</h2>

        <div class="errordatabox">
			<?php echo nl2br(htmlspecialchars($data['exception_stacktrace'])); ?>
        </div>
    </div>

    <div class="tablebox" data-collapse>
        <h2 class="open collapseheader">Additional</h2>

        <div class="errordatabox">
			<?php echo nl2br(htmlspecialchars($data['additional_info'])); ?>
        </div>
    </div>

    <?php printSQLStats(); ?>


    <script type="text/javascript">
		<?php echo file_get_contents('admin.js'); ?>
    </script>

    <script src="sorttable.js"></script>
    <script src="jquery.collapse.js"></script>
</body>
</html>