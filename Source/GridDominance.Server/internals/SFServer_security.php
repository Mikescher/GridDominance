<?php

use \ParagonIE\EasyRSA\EasyRSA;


function check_commit_signature(string $sig, array $data) {
	global $config;

	if ($config['debug']) return; // no checking while debug

	$dat = hash('sha256', join("\n", $data));

	if (!EasyRSA::verify($dat, $sig, $config['public_key'])) {
		outputError(ERRORS::PARAMETER_HASH_MISMATCH, "The signature '$sig' is invalid.");
	}
}