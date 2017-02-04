<?php

use \ParagonIE\EasyRSA\EasyRSA;
use ParagonIE\EasyRSA\Exception\InvalidChecksumException;
use ParagonIE\EasyRSA\Exception\InvalidCiphertextException;


/**
 * @param string $sig
 * @param array $data
 */
function check_commit_signature($sig, $data) {
	global $config;

	$dat = hash('sha256', join("\n", $data));

	if (!EasyRSA::verify($dat, $sig, $config['masterkey'])) {
		outputError(ERRORS::PARAMETER_HASH_MISMATCH, "The signature '$sig' is invalid.");
	}
}

/**
 * @param string $input
 * @return bool|string
 */
function decrypt_rsa($input) {
	global $config;

	try {
		$td = EasyRSA::encrypt("qwe", $config['parameterkey']->getPublicKey());

		return EasyRSA::decrypt($input, $config['parameterkey']);
	} catch (InvalidCiphertextException $e) {
		return false;
	} catch (InvalidChecksumException $e) {
		return false;
	}
}