<?php

if(count(get_included_files()) ==1) exit("Direct access not permitted.");

class SQLStatistics
{
	public static $ExecutionTimes = [];
	
	public static function Add($name, $sql, $time)
	{
		if (array_key_exists($name, self::$ExecutionTimes)) {
			$v = self::$ExecutionTimes[$name];

			$v[0] += 1;
			$v[1] += $time;
			array_push($v[2], $sql);
			$v[2] = array_unique($v[2]);

			self::$ExecutionTimes[$name] = $v;
		} else {
			$v = [ 1, $time, [$sql] ];
			self::$ExecutionTimes[$name] = $v;
		}
	}
}

function sql_query_num($name, $query)
{
	global $pdo;

	$start = microtime(true);

	$r = $pdo->query($query)->fetch(PDO::FETCH_NUM)[0];

	SQLStatistics::Add($name, $query, microtime(true) - $start);
	
	return $r;
}

function sql_query_num_prep($name, $query, $params)
{
	global $pdo;

	$start = microtime(true);

	$stmt = $pdo->prepare($query);
	
	foreach ($params as $p) 
	{
		if (strpos($query, $p[0]) !== FALSE) $stmt->bindValue($p[0], $p[1], $p[2]);
	}

	$stmt->execute();
	$r = $stmt->fetch(PDO::FETCH_NUM)[0];

	SQLStatistics::Add($name, $query, microtime(true) - $start);
	
	return $r;
}

function sql_query_assoc($name, $query)
{
	global $pdo;

	$start = microtime(true);

	$r = $pdo->query($query)->fetchAll(PDO::FETCH_ASSOC);

	SQLStatistics::Add($name, $query, microtime(true) - $start);
	
	return $r;
}

function sql_query_assoc_prep($name, $query, $params)
{
	global $pdo;

	$start = microtime(true);

	$stmt = $pdo->prepare($query);
	
	foreach ($params as $p) 
	{
		if (strpos($query, $p[0]) !== FALSE) $stmt->bindValue($p[0], $p[1], $p[2]);
	}

	$stmt->execute();
	$r = $stmt->fetchAll(PDO::FETCH_ASSOC);

	SQLStatistics::Add($name, $query, microtime(true) - $start);
	
	return $r;
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