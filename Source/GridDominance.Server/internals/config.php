<?php

use \ParagonIE\EasyRSA\PublicKey;

return [
	'database_host' =>  'localhost',
	'database_name' =>  'grid_dominance',
	'database_user' =>  'root',
	'database_pass' =>  '',

	'public_key' => new PublicKey(file_get_contents(__DIR__  . '/masterkey.public')),

	'logfile-normal' => __DIR__ . '/../server.log',
	'logfile-debug'  => __DIR__ . '/../server_[{action}]_debug.log',
	'logfile-error'  => __DIR__ . '/../server_error.log',
	'email-error-target' => 'mailport@mikescher.de',
	'email-error-sender' => 'gdserver-error@mikescher.com',

	'email-clientlog-target' => 'mailport@mikescher.de',
	'email-clientlog-sender' => 'gd-log@mikescher.com',

	'maxsize-logfile-normal' =>  128 * 1024 * 1024, // 512MB
	'maxsize-logfile-debug'  =>   16 * 1024 * 1024, // 128MB
	'maxsize-logfile-error'  =>  128 * 1024 * 1024, // 512MB

	'debug' => true,
];