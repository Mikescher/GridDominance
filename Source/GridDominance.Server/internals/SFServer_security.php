<?php

/**
 * @param string $sig_in
 * @param array $data
 * @return true
 */
function check_commit_signature($sig_in, $data) {
	global $config;

	$sigbuilder = $config['signature_key'] . join("", array_map(function ($a){return "\n$a";}, $data));

	$sig_real = hash('sha256', $sigbuilder);

	if (strcasecmp($sig_real, $sig_in) !== 0) {
		outputError(ERRORS::PARAMETER_HASH_MISMATCH, "The signature '$sig_in' is invalid.");
	};
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