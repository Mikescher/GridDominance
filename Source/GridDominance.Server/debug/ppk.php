<?php
{$c=require('../internals/config.php'); if (!$c['debug']) exit('Nope.');}

$config =
	[
		'config' => 'C:\TOOLS\XAMPP\php\extras\openssl\openssl.cnf',
		'private_key_bits' => 512
	];


$res=openssl_pkey_new($config);

openssl_pkey_export($res, $privkey, null, $config);
echo nl2br($privkey);
echo "\n<hr>\n";

$pubkey=openssl_pkey_get_details($res);
echo nl2br($pubkey["key"]);
echo "\n<hr>\n";



