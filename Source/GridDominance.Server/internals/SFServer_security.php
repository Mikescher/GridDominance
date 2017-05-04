<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

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