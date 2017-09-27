<?php // SAMFramework Server

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

require_once 'backend.php';


require_once 'SFServer_log.php';
require_once 'SFServer_io.php';
require_once 'SFServer_security.php';
require_once 'SFServer_db.php';
require_once 'SFServer_sql.php';

