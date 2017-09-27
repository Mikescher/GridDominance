<?php
{$c=require('../internals/config.php'); if (!$c['debug']) exit('Nope.');}

require '../internals/backend.php';

init("");

$collapsed_cols = ['exception_message', 'exception_stacktrace', 'additional_info', 'password_hash'];
$long_cols_256 = ['screen_resolution', 'timestamp', 'last_changed', 'best_last_changed', 'last_online', 'creation_time'];
$long_cols_384 = ['levelid'];

$gid = 0;
foreach ($pdo->query("SHOW TABLES")->fetchAll(PDO::FETCH_NUM) as $tabarr)
{
	$tab = $tabarr[0];

	print "<h1>$tab</h1>";

	$data = $pdo->query("SELECT * FROM $tab LIMIT 100")->fetchAll();
	$cols = $pdo->query("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$tab' AND TABLE_SCHEMA = 'gdapi_data'")->fetchAll(PDO::FETCH_NUM);

	print "<table class=\"sqltab pure-table pure-table-bordered\">\n";

	print "<thead>\n";
	print "<tr>\n";
	foreach ($cols as $col) {
		print "<th>" . str_replace("_", " ", $col[0]). "</th>\n";
	}
	print "</tr>\n";
	print "</thead>\n";

	$functions = "";

	foreach ($data as $datum) {
		print "<tr>\n";
		$i = 0;
		foreach ($datum as $col => $value) {
			if (in_array($cols[$i][0], $collapsed_cols))
			{
				print "<td><a href='#' onclick='ShowExpandedColumn_".$gid."_".$i."();return false;'>show</a></td>\n";

				$functions .= "\n\n";
				$functions .= "function ShowExpandedColumn_".$gid."_".$i."() {" . "\n";
				$functions .= '$(".tab_prev").css("visibility", "collapse");' . "\n";
				$functions .= '$("#td_prev_'.$gid.'").html(' . str_replace('\n', '<br/>', json_encode($value)) . ');' . "\n";
				$functions .= '$("#tr_prev_'.$gid.'").css("visibility", "visible");' . "\n";
				$functions .= "}";

			}
			else if (in_array($cols[$i][0], $long_cols_256))
			{
				print "<td style='max-width: 256px'>$value</td>\n";
			}
			else if (in_array($cols[$i][0], $long_cols_384))
			{
				print "<td style='max-width: 384px'>$value</td>\n";
			}
			else
			{
				print "<td>$value</td>\n";
			}

			$i++;
		}
		print "</tr>\n";
		print "<tr class='tab_prev' id='tr_prev_$gid'><td colspan='" . sizeof($datum) . "' id='td_prev_$gid' style='text-align: left;' ></td></tr>\n";
		$gid++;
	}

	print "</table>\n";

	print "<script type=\"text/javascript\">";
	print $functions;
	print "</script>";
}