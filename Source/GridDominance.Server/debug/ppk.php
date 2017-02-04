<?php

require_once '../internals/backend.php';

use \ParagonIE\EasyRSA\KeyPair;

$keyPair = KeyPair::generateKeyPair(2048);

echo nl2br($keyPair->getPrivateKey()->getKey());

echo "<hr>";

echo nl2br($keyPair->getPublicKey()->getKey());