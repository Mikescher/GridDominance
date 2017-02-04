<?php

/**
 * @param string $sig
 * @param array $data
 */
function check_commit_signature($sig, $data) {
	global $config;

	$dat = hash('sha256', join("\n", $data));

	return true;
}

/**
 * @param string $input
 * @return bool|string
 */
function decrypt_rsa($input) {
	global $config;

	$pkey = openssl_pkey_get_private($config['parameterkey']);

	$decrypted = "";
	$encrypted = base64_decode($input);

	if(!openssl_private_decrypt($encrypted, $decrypted, $pkey)) return false;

	return $decrypted;
}