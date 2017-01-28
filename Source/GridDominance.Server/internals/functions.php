<?php

require_once 'backend.php';

function check_commit_hash(string $hash, array $data) {
	global $config;

	$dat = $hash('sha256', join("\n", $data) . "\n" . $config['app_secret']);

	if ($dat !== $hash) outputError(ERRORS::PARAMETER_HASH_MISMATCH, "The hash '$hash' is invalid.");
}