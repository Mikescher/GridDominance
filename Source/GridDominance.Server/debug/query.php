<?php
{$c=require('../internals/config.php'); if (!$c['debug']) exit('Nope.');}

require '../internals/backend.php';

init("");

foreach ($pdo->query("SHOW TABLES")->fetchAll(PDO::FETCH_NUM) as $tabarr)
{
	$tab = $tabarr[0];

	print "<h1>$tab</h1>";

	$data = $pdo->query("SELECT * FROM $tab")->fetchAll();
	$cols = $pdo->query("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$tab' AND TABLE_SCHEMA = 'grid_dominance'")->fetchAll(PDO::FETCH_NUM);

	print "<table class=\"sqltab pure-table pure-table-bordered\">\n";

	print "<thead>\n";
	print "<tr>\n";
	foreach ($cols as $col) {
		print "<th>" . str_replace("_", " ", $col[0]). "</th>\n";
	}
	print "</tr>\n";
	print "</thead>\n";

	foreach ($data as $datum) {
		print "<tr>\n";
		foreach ($datum as $col => $value) {
			print "<td>$value</td>\n";
		}
		print "</tr>\n";
	}

	print "</table>\n";
}