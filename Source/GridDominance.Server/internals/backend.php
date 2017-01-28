<?php

$config = require('config.php');

if (!$config['debug']) error_reporting(E_STRICT);

$dsn = 'mysql:host=' . $config['database_host'] . ';dbname=' . $config['database_name'] . ';charset=utf8';
$opt = [
    PDO::ATTR_ERRMODE            => PDO::ERRMODE_EXCEPTION,
    PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
    PDO::ATTR_EMULATE_PREPARES   => false,
];
$pdo = new PDO($dsn, $config['database_user'], $config['database_pass'], $opt);

