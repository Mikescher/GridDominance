<?php

require 'internals/backend.php';


function run() {
	global $pdo;
	global $config;

	$userid        = getParamUIntOrError('userid');
	$password      = getParamSHAOrError('password');
	$resolution    = getParamStrOrError('screen_resolution');
	$appversion    = getParamStrOrError('app_version');
	$identifier    = getParamStrOrError('exception_id');
	$message       = getParamDeflOrError('exception_message', true);
	$stacktrace    = getParamDeflOrError('exception_stacktrace', true);
	$additional    = getParamDeflOrError('additional_info', true);

	//----------

	$user = GDUser::QueryOrNull($pdo, $password, $userid);

	//----------

	$stmt = $pdo->prepare("INSERT INTO error_log(userid, password_verified, screen_resolution, app_version, exception_id, exception_message, exception_stacktrace, additional_info) VALUES (:ui, :pv, :sr, :av, :id, :mg, :st, :ai)");
	$stmt->bindValue(':ui', $userid,        PDO::PARAM_INT);
	$stmt->bindValue(':pv', $user !== NULL, PDO::PARAM_INT);
	$stmt->bindValue(':sr', $resolution,    PDO::PARAM_STR);
	$stmt->bindValue(':av', $appversion,    PDO::PARAM_STR);
	$stmt->bindValue(':id', $identifier,    PDO::PARAM_STR);
	$stmt->bindValue(':mg', $message,       PDO::PARAM_STR);
	$stmt->bindValue(':st', $stacktrace,    PDO::PARAM_STR);
	$stmt->bindValue(':ai', $additional,    PDO::PARAM_STR);
	executeOrFail($stmt);

	$errid = $pdo->lastInsertId();

	//----------

	$subject = "Client send log '$identifier' at " . date("Y-m-d H:i:s");

	$content = "";

	$content .= 'Error ID: '             . $errid                                                    . "\n";
	$content .= 'User: '                 . $userid . (($user===NULL)?(""):(" ($user->Username)"))    . "\n";
	$content .= 'Screen Resolution: '    . $resolution                                               . "\n";
	$content .= 'App Version: '          . $appversion                                               . "\n";
	$content .= 'Error Identifier: '     . $identifier                                               . "\n";
	$content .= 'Error Message: '        . $message                                                  . "\n";
	$content .= 'Stacktrace: '           . "\n" . $stacktrace                                        . "\n";
	$content .= 'Additional: '           . "\n" . $additional                                        . "\n";
	$content .=                                                                                        "\n";
	$content .= '--------------------------------------------------------------------------------'   . "\n";
	$content .=                                                                                        "\n";
	$content .= 'HTTP_HOST: '            . ParamServerOrUndef('HTTP_HOST')                           . "\n";
	$content .= 'REQUEST_URI: '          . ParamServerOrUndef('REQUEST_URI')                         . "\n";
	$content .= 'TIME: '                 . date('Y-m-d H:i:s')                                       . "\n";
	$content .= 'REMOTE_ADDR: '          . ParamServerOrUndef('REMOTE_ADDR')                         . "\n";
	$content .= 'HTTP_X_FORWARDED_FOR: ' . ParamServerOrUndef('HTTP_X_FORWARDED_FOR')                . "\n";
	$content .= 'HTTP_USER_AGENT: '      . ParamServerOrUndef('HTTP_USER_AGENT')                     . "\n";

	try	{
		sendmail($subject, $content, $config['email-clientlog-target'], $config['email-clientlog-sender']);
	} catch (Exception $e) {
		outputErrorException(Errors::INTERNAL_EXCEPTION, 'Cannot send mail', $e, LOGLEVEL::ERROR);
	}

	//----------

	logDebug("registered log event for $userid : $identifier (v: $appversion)");
	outputResultSuccess(['errid' => $errid]);
}



try {
	init("log-client");
	run();
} catch (Exception $e) {
	outputErrorException(Errors::INTERNAL_EXCEPTION, 'InternalError', $e, LOGLEVEL::ERROR);
}