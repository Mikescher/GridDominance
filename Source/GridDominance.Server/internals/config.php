<?php

//TODO make prod version
return [
	'database_host' =>  'localhost',
	'database_name' =>  'grid_dominance',
	'database_user' =>  'root',
	'database_pass' =>  '',

	'signature_key' => 'OZothFoshCiHyPhebMyGheVushNopTyg',                    //TODO change me for prod
	'cron-secret' => 'cron',                                                  //TODO change me for prod

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

	'levelids' =>
		[
			'{b16b00b5-0001-4000-0000-000001000001}',
			'{b16b00b5-0001-4000-0000-000001000002}',
			'{b16b00b5-0001-4000-0000-000001000003}',
			'{b16b00b5-0001-4000-0000-000001000004}',
			'{b16b00b5-0001-4000-0000-000001000005}',
			'{b16b00b5-0001-4000-0000-000001000006}',
			'{b16b00b5-0001-4000-0000-000001000007}',
			'{b16b00b5-0001-4000-0000-000001000008}',
			'{b16b00b5-0001-4000-0000-000001000009}',
			'{b16b00b5-0001-4000-0000-000001000010}',
			'{b16b00b5-0001-4000-0000-000001000011}',
			'{b16b00b5-0001-4000-0000-000001000012}',
			'{b16b00b5-0001-4000-0000-000001000013}',
			'{b16b00b5-0001-4000-0000-000001000014}',
		],

	'difficulties' => [0x10, 0x11, 0x12, 0x13],

	'debug' => true,
	'ping_emulation' => 0, // sec
];