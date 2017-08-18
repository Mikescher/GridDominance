<?php

require_once '../internals/backend.php';
require_once '../internals/utils.php';

init("admin");

foreach (listLogFiles() as $log) {
	if ($log['name'] == $_GET['id']) {
		echo "deleted " . $log['name'];

		unlink($log['path']);
		touch($log['path']);

		echo "<script type=\"text/javascript\">setTimeout(function(){location.href = document.referrer;}, 500);</script>";

		return;
	}
}

echo 'LOG FILE NOT FOUND';