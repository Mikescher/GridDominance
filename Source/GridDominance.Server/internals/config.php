<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

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
	'logfile-cron'  => __DIR__ . '\\..\\log\\cron.log',
	'email-error-target' => 'mailport@mikescher.de',
	'email-error-sender' => 'gdserver-error@mikescher.com',

	'email-clientlog-target' => 'mailport@mikescher.de',
	'email-clientlog-sender' => 'gd-log@mikescher.com',

	'maxsize-logfile-normal' =>  128 * 1024 * 1024, // 512MB
	'maxsize-logfile-debug'  =>   16 * 1024 * 1024, // 128MB
	'maxsize-logfile-error'  =>  128 * 1024 * 1024, // 512MB

	'levelmapping' => require 'config_levelids.php',
	'levelids' => array_map(function($k){ return $k[1]; }, require 'config_levelids.php'),

	'worldid_0' => '{d34db335-0001-4000-7711-000000100001}',
	'worldid_1' => '{d34db335-0001-4000-7711-000000200001}',
	'worldid_2' => '{d34db335-0001-4000-7711-000000200002}',
	'worldid_3' => '{d34db335-0001-4000-7711-000000200003}',
	'worldid_4' => '{d34db335-0001-4000-7711-000000200004}',

	'difficulties' => [0x00, 0x01, 0x02, 0x03],
	//'diff_scores'  => [11,   13,   17,   23  ],

	'debug' => true,
	'ping_emulation' => 0, // sec
];