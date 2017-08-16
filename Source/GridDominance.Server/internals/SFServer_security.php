<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

/**
 * @param string $sig_in
 * @param array $data
 */
function check_commit_signature($sig_in, $data) {
	global $config;

	if ($config['debug']) return;

	$sigbuilder = $config['signature_key'] . join("", array_map(function ($a){return "\n$a";}, $data));

	$sig_real = hash('sha256', $sigbuilder);

	if (strcasecmp($sig_real, $sig_in) !== 0) {
		logDebug("[1] [[Signature error]] $sig_real <> $sig_in");
		logDebug("[2] \n========\n" . $config['signature_key'] . join("", array_map(function ($a){return "\n$a";}, $data)) . "========\n");
		logDebug("[3] \n========\n" . $sigbuilder . "\n========\n");
		logDebug("[4] \n========\n" . base64_encode($sigbuilder) . "\n========\n");

		outputError(ERRORS::PARAMETER_HASH_MISMATCH, "The signature '$sig_in' is invalid.");
	};
}