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
			'{7018e8a1-6b04-447c-8972-c2c2e118fc49}',
			'{9887f46b-b27e-4dba-b9f3-92a8d6ec26e6}',
			'{f9f79506-f15a-47d4-abcc-afdc116b7b3c}',
		],

	'difficulties' => [0x10, 0x11, 0x12, 0x13],

	'debug' => true,
];