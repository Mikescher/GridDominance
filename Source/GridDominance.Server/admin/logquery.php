<?php

require_once '../internals/backend.php';
require_once '../internals/utils.php';
require_once 'common/libadmin.php';

init("admin");

foreach (listLogFiles() as $log) {
	if ($log['name'] == $_GET['id']) { echo ($log['content']); return; }
}

echo 'LOG FILE NOT FOUND';
