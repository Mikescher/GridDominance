<?php

function includeStyles()
{
	print('<link rel="stylesheet" type="text/css" href="/admin/common/pure-min.css"/>');
	print('<link rel="stylesheet" type="text/css" href="/admin/common/toastr.min.css"/>');
	print('<link rel="stylesheet" type="text/css" href="/admin/common/admin.css">');
}

function includeScripts()
{
	print('<script src="/admin/common/jquery-3.1.0.min.js"></script>');
	print('<script src="/admin/common/Chart.min.js"></script>');
	print('<script src="/admin/common/admin.js"></script>');
	print('<script src="/admin/common/jquery.collapse.js"></script>');
	print('<script src="/admin/common/toastr.min.js"></script>');
	print('<script src="/admin/common/sorttable.js"></script>');
	print('<script src="/admin/common/ac/amcharts.js"></script>');
	print('<script src="/admin/common/ac/serial.js"></script>');
    print('<script src="/admin/common/ac/themes/light.js"></script>');
}

//############
$previd = 0;
//############

function expansioncell($txt) {
	global $previd;

	echo "<td>";
	echo "<a href='#' onclick='ShowExpandedColumn(" . $previd . ", " . str_replace("'", "\\u0027", str_replace('\n', '<br/>', json_encode($txt))) . ");return false;'>show</a>";
	echo "</td>";
}

function expansioncell3($txt, $hdr) {
	global $previd;

	echo "<td>";
	echo "<a href='#' onclick='ShowExpandedColumn(" . $previd . ", " . str_replace("'", "\\u0027", str_replace('\n', '<br/>', json_encode($txt))) . ");return false;'>show (" . $hdr . ")</a>";
	echo "</td>";
}

function expansioncell4($hdr, $dat) {
	global $previd;

	echo "<td>";
	echo "<a href='#' onclick='ShowExpandedColumn(" . $previd . ", " . str_replace("'", "\\u0027", str_replace('\n', '<br/>', json_encode($dat))) . ");return false;'>$hdr</a>";
	echo "</td>";
}

function remoteexpansioncell($txt) {
	global $previd;

	echo "<td>";
	echo "<a href='#' onclick='ShowRemoteExpandedColumn(" . $previd . ", \"" . $txt . "\");return false;'>show</a>";
	echo "</td>";
}

function fmtLevelID($id) {
	if ($id == '{b16b00b5-0001-4000-9999-000000000002}') return "TUTORIAL";

	return (int)substr($id, 25, 6) . " - " . (int)substr($id, 31, 6);
}

function fmtWorldID($w) {
	switch ($w) {
		case '{d34db335-0001-4000-7711-000000100002}': return 'NO_WORLD';
		case '{d34db335-0001-4000-7711-000000100001}': return 'TUTORIAL';
		case '{d34db335-0001-4000-7711-000000200001}': return 'WORLD_1';
		case '{d34db335-0001-4000-7711-000000200002}': return 'WORLD_2';
		case '{d34db335-0001-4000-7711-000000200003}': return 'WORLD_3';
		case '{d34db335-0001-4000-7711-000000200004}': return 'WORLD_4';
		case '{d34db335-0001-4000-7711-000000200005}': return 'WORLD_5';
		case '{d34db335-0001-4000-7711-000000200006}': return 'WORLD_6';
		case '{d34db335-0001-4000-7711-000000200007}': return 'WORLD_7';
		case '{d34db335-0001-4000-7711-000000200008}': return 'WORLD_8';
		case '{d34db335-0001-4000-7711-000000200009}': return 'WORLD_9';
		case '{d34db335-0001-4000-7711-000000300001}': return 'MULTIPLAYER';
		case '{d34db335-0001-4000-7711-000000300002}': return 'ONLINE';
		default:                                       return "????";
	}
}

function getSessionCount() {
	if (! file_exists("/var/log/gdapi_log/proxystate.json")) return "?";

	$string = file_get_contents("/var/log/gdapi_log/proxystate.json");
	$json = json_decode($string, true);

	return count($json['sessions']);
}

function lc($txt) {
	$c = 0;
	foreach (explode("\n", $txt) as $l) { if (!empty($l)) $c++; }
	return $c;
}

function formatSizeUnits($bytes)
{
	if ($bytes === NULL) return "NULL";

	if ($bytes >= 1073741824)
	{
		$bytes = number_format($bytes / 1073741824, 2) . ' GB';
	}
	elseif ($bytes >= 1048576)
	{
		$bytes = number_format($bytes / 1048576, 2) . ' MB';
	}
	elseif ($bytes >= 1024)
	{
		$bytes = number_format($bytes / 1024, 2) . ' KB';
	}
	elseif ($bytes > 1)
	{
		$bytes = $bytes . ' bytes';
	}
	elseif ($bytes == 1)
	{
		$bytes = $bytes . ' byte';
	}
	else
	{
		$bytes = '0 bytes';
	}

	return $bytes;
}

function printSQLStats() {

	echo("<div class=\"tablebox\" data-collapse id='sqlprof_data'>");
	echo("<h2 class=\"open collapseheader\">SQL Data</h2>");
	echo("<table class=\"sqltab pure-table pure-table-bordered sortable\" style='width: 90%'>");
	echo("<thead>");
	echo("<tr>");
	echo("<th width='200px'>Name</th>");
	echo("<th width='50px'>Count</th>");
	echo("<th width='300px'>Time (Sum)</th>");
	echo("</tr>");
	echo("</thead>");

	uasort(SQLStatistics::$ExecutionTimes, function ($a, $b) { return ($a[1] < $b[1]) ? +1 : -1; });

	foreach (SQLStatistics::$ExecutionTimes as $key => $exectime)
	{
		echo("<tr>");
		echo("<td>".htmlspecialchars($key)."</td>");
		echo("<td>".htmlspecialchars($exectime[0])."</td>");
		echo("<td>".htmlspecialchars($exectime[1])."</td>");
		echo("</tr>");

		//foreach ($exectime[2] as $esql)
		//{
		//	echo("<tr class='sqlrow' style=\"visibility: visible; display: table-row;\" >");
		//	echo("<td colspan='3' ><div class='div_prev'>".htmlspecialchars($esql)."</div></td>");
		//	echo("</tr>");
		//}
	}
	echo("</table>");
	echo("</div>");



	echo("<script type=\"text/javascript\">");
	echo("function ShowSQLProfiler() {");
	echo("$(\"#sqlprof_data\").css(\"visibility\", \"visible\");");
	echo("$(\"#sqlprof_data\").css(\"display\", \"flex\");");
	echo("}");
	echo("</script>");
	echo("<div id=\"sqlprof_trigger\"><a href=\"#\" onclick=\"ShowSQLProfiler();window.scrollTo(0,document.body.scrollHeight);return false;\">ѳ</a></div>");
}
