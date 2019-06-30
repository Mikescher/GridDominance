<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

$config_levelids = require 'config_levelids.php';
$config_auto     = require 'config_auto.php';

return [
	'database_host' =>  'localhost',
	'database_name' =>  'gdapi_data',
	'database_user' =>  'root',
	'database_pass' =>  '',

	'signature_key' => 'smth',
	'cron-secret'   => 'cron',

	'logfile-normal' => __DIR__ . '\\..\\log\\server.log',
	'logfile-debug'  => __DIR__ . '\\..\\log\\server_[{action}]_debug.log',
	'logfile-error'  => __DIR__ . '\\..\\log\\server_error.log',
	'logfile-cron'   => __DIR__ . '\\..\\log\\cron.log',
	'email-error-target' => 'virtualadmin@mikescher.de',
	'email-error-sender' => 'gdserver-error@mikescher.com',

	'email-clientlog-target' => 'virtualadmin@mikescher.de',
	'email-clientlog-sender' => 'gd-log@mikescher.com',
	'sendmail'         => false,
	'sendnotification' => true,

	'scn_id'  => '56',
	'scn_key' => '5B27a01fHhq6BvrWT5HrYS6xt4IAha0a6qA5TpoG20Iti1b3522VxvgjMo4UA7CS',

	'maxsize-logfile-normal' =>  128 * 1024 * 1024, // 512MB
	'maxsize-logfile-debug'  =>   16 * 1024 * 1024, // 128MB
	'maxsize-logfile-error'  =>  128 * 1024 * 1024, // 512MB

	'levelmapping'       => $config_levelids,
	'levelids'           => array_keys($config_levelids),
	'latest_version'     => $config_auto['latest_version'],
	'latest_version_alt' => isset($config_auto['latest_version_alt']) ? $config_auto['latest_version_alt'] : NULL,

	'worldid_0' => '{d34db335-0001-4000-7711-000000100001}',
	'worldid_1' => '{d34db335-0001-4000-7711-000000200001}',
	'worldid_2' => '{d34db335-0001-4000-7711-000000200002}',
	'worldid_3' => '{d34db335-0001-4000-7711-000000200003}',
	'worldid_4' => '{d34db335-0001-4000-7711-000000200004}',

	'difficulties' => [0x00, 0x01, 0x02, 0x03],
	'diff_scores'  => [11,   13,   17,   23  ],
	'hot_factor'   => 1.8,

	'userlevel_maxsize'   => 256 * 1024,
	'userlevel_directory' => 'F:\\Stash\\gd_server_upload\\',

	'debug'  => true,
	'runlog' => true,

	'ping_emulation' => 0.0, // sec
];