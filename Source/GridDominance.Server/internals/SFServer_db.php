<?php

function executeOrFail(PDOStatement $stmt) {
	$r = $stmt->execute();
	if (! $r) outputError(ERRORS::SQL_FAILED, "SQL Statement failed: '$stmt->queryString'", LOGLEVEL::ERROR);
}