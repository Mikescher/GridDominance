<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

$config_levelids = require 'config_levelids.php';
$config_auto     = require 'config_auto.php';

return [
	'database_host' =>  '{{DB_HOST}}',
	'database_name' =>  'gdapi_data',
	'database_user' =>  '{{DB_USER}}',
	'database_pass' =>  '{{DB_PASS}}',

	'signature_key' => '{{SIGNATURE_KEY}}',
	'cron-secret'   => '{{CRON_SECRET}}',

	'logfile-normal' => '/var/log/gdapi_log/server.log',
	'logfile-debug'  => '/var/log/gdapi_log/server_[{action}]_debug.log',
	'logfile-error'  => '/var/log/gdapi_log/server_error.log',
	'logfile-cron'   => '/var/log/gdapi_log/cron.log',

	'email-error-target' => 'virtualadmin@mikescher.de',
	'email-error-sender' => 'gdserver-error@mikescher.com',

	'email-clientlog-target' => 'virtualadmin@mikescher.de',
	'email-clientlog-sender' => 'gd-log@mikescher.com',

	'sendmail'         => {{SENDMAIL}},
	'sendnotification' => {{SENDNOTIFICATION}},

	'scn_id'  => '{{SCN_ID}}',
	'scn_key' => '{{SCN_KEY}}',

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
	'userlevel_directory' => '/media/gdapi_userlevel/',

	'debug'  => {{DEBUG}},
	'runlog' => {{RUNLOG}},

	'ping_emulation' => 0.0, // sec
];