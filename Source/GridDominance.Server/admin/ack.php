<?php require_once '../internals/backend.php'; ?>
<?php require_once '../internals/utils.php'; ?>
<?php init("admin"); ?>

<?php


global $pdo;

$stmt = $pdo->prepare("UPDATE error_log SET error_log.acknowledged=1 WHERE error_log.error_id = :id");
$stmt->bindValue(':id', $_GET["id"], PDO::PARAM_INT);
$stmt->execute();

echo "Acknowledged.";

?>
