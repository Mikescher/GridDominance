<?php

use \ParagonIE\EasyRSA\PublicKey;

return [
	'database_host' =>  'localhost',
	'database_name' =>  'grid_dominance',
	'database_user' =>  'root',
	'database_pass' =>  '',

	'public_key' => new PublicKey(file_get_contents('masterkey.php')),

	'debug' => true,
];