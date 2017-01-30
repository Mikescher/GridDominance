<?php

use \ParagonIE\EasyRSA\PublicKey;

return [
	'database_host' =>  'localhost',
	'database_name' =>  'grid_dominance',
	'database_user' =>  'root',
	'database_pass' =>  '',

	'public_key' => new PublicKey(file_get_contents(__DIR__  . '/masterkey.public')),

	'logfile-normal' => __DIR__ . '/../server-debug.log',
	'logfile-debug'  => __DIR__ . '/../server-debug.log',
	'email-error-target' => 'mailport@mikescher.de',
	'email-error-sender' => 'gdserver-error@mikescher.com',

	'maxsize-logfile-normal' => 512 * 1024 * 1024, // 512MB
	'maxsize-logfile-debug'  => 128 * 1024 * 1024, // 128MB

	'debug' => true,
];