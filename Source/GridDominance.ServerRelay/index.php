<?php

// error_reporting(E_STRICT);
// ini_set('display_errors', 1);

$db_host     = 'localhost';
$db_dbname   = 'gdapi_relay';
$db_user     = 'gdapi_user';
$db_password = '?';

$mail_to   = 'virtualadmin@mikescher.de';
$mail_from = 'gdserver-relay-error@mikescher.com';

function ParamServerOrUndef($idx) {
	return isset($_SERVER[$idx]) ? $_SERVER[$idx] : 'NOT_SET';
}

function sendMail($subject, $content, $to, $from) {
	mail($to, $subject, $content, 'From: ' . $from);
}

try
{
	$requri = parse_url($_SERVER["REQUEST_URI"], PHP_URL_PATH);

	$query = isset($_SERVER["QUERY_STRING"]) ? $_SERVER["QUERY_STRING"] : "";

	$host = 'gdapi.cannonconquest.net';

	$newurl = 'http://' . $host . $requri . ($query == '' ? '' : ('?'.$query));

	if ($requri == '/') die('No more!');
	if ($requri == '/favicon.ico') die('No more!');
	if ($requri == '/admin') die('No more!');
	if ($requri == '/admin/') die('No more!');
	if ($requri == '/admin/index.php') die('No more!');
	if ($requri == '/savesessionstate.php') die('No more!');
	if ($requri == '/robots.txt') die("User-agent: *\nDisallow: /");

	$options =
	[
		'http' =>
		[
			'header'  => "Content-type: application/x-www-form-urlencoded\r\n",
			'method'  => 'POST',
			'content' => http_build_query($_POST)
		]
	];

	$context  = stream_context_create($options);
	$result = file_get_contents($newurl, false, $context);
	if ($result === FALSE) 
	{ 
		throw new Exception('query returned ' . print_r($result, true));
	}
	else
	{
		ob_start();
		print($result);
   		header("Content-Encoding: none");
		header('Content-Length: '.ob_get_length());
		header('Connection: close');
		ob_end_flush();
		ob_flush();
		flush();

		// --------------- after work ---------------

		$dsn = "mysql:host=$db_host;dbname=$db_dbname;charset=utf8";
		$opt = 
		[
			PDO::ATTR_ERRMODE	    => PDO::ERRMODE_EXCEPTION,
			PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
			PDO::ATTR_EMULATE_PREPARES   => false,
		];
		$pdo = new PDO($dsn, $db_user, $db_password, $opt);
		$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
		
		$stmt = $pdo->prepare("INSERT INTO relay_log(logdate, target, logcount) VALUES (DATE(NOW()), :target, 1) ON DUPLICATE KEY UPDATE logcount=logcount+1");
		$stmt->bindValue(':target', $requri, PDO::PARAM_STR);
		$stmt->execute();
	}
}
catch (Exception $e)
{
	$subject = "SFRelayServer has encountered an Error at " . date("Y-m-d H:i:s") . "] ";

	$content = "";

	$content .= 'HTTP_HOST: '            . ParamServerOrUndef('HTTP_HOST')            . "\n";
	$content .= 'REQUEST_URI: '          . ParamServerOrUndef('REQUEST_URI')          . "\n";
	$content .= 'TIME: '                 . date('Y-m-d H:i:s')                        . "\n";
	$content .= 'REMOTE_ADDR: '          . ParamServerOrUndef('REMOTE_ADDR')          . "\n";
	$content .= 'HTTP_X_FORWARDED_FOR: ' . ParamServerOrUndef('HTTP_X_FORWARDED_FOR') . "\n";
	$content .= 'HTTP_USER_AGENT: '      . ParamServerOrUndef('HTTP_USER_AGENT')      . "\n";
	$content .= 'MESSAGE:'               . "\n" . $e->getMessage()                    . "\n";
	$content .= 'TRACE:'                 . "\n" . $e->getTraceAsString ()             . "\n";
    $content .= 'EXCEPTION:'             . "\n" . $e                                  . "\n";
	$content .= '$_GET:'                 . "\n" . print_r($_GET, true)                . "\n";
	$content .= '$_POST:'                . "\n" . print_r($_POST, true)               . "\n";
	$content .= '$_FILES:'               . "\n" . print_r($_FILES, true)              . "\n";

	sendMail($subject, $content, $mail_to, $mail_from);

	header('HTTP/1.1 500 Internal Server Error');
	print("Relay communication failed");
}