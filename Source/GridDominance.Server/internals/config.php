<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

return [
	'database_host' =>  'localhost',
	'database_name' =>  'gdapi_data',
	'database_user' =>  'root',
	'database_pass' =>  '',

	'signature_key' => 'smth',
	'cron-secret'   => 'cron',

	'logfile-normal' => __DIR__ . '/../log/server.log',
	'logfile-debug'  => __DIR__ . '/../log/server_[{action}]_debug.log',
	'logfile-error'  => __DIR__ . '/../log/server_error.log',
	'email-error-target' => 'mailport@mikescher.de',
	'email-error-sender' => 'gdserver-error@mikescher.com',

	'email-clientlog-target' => 'mailport@mikescher.de',
	'email-clientlog-sender' => 'gd-log@mikescher.com',

	'maxsize-logfile-normal' =>  128 * 1024 * 1024, // 512MB
	'maxsize-logfile-debug'  =>   16 * 1024 * 1024, // 128MB
	'maxsize-logfile-error'  =>  128 * 1024 * 1024, // 512MB

	'levelmapping' => require 'config_levelids.php',
	'levelids' => array_map(function($k){ return $k[1]; }, require 'config_levelids.php'),

	'difficulties' => [0x00, 0x01, 0x02, 0x03],

	'debug' => true,
	'ping_emulation' => 0, // sec
];