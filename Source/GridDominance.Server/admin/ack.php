<?php require_once '../internals/backend.php'; ?>
<?php require_once '../internals/utils.php'; ?>
<?php init("admin"); ?>

<?php


global $pdo;

if (!empty($_GET["id"])) 
{
	$id = $_GET["id"];

	$stmt = $pdo->prepare("UPDATE error_log SET error_log.acknowledged=1 WHERE error_log.error_id = :id");
	$stmt->bindValue(':id', $id, PDO::PARAM_INT);
	$stmt->execute();

	echo "Acknowledged $id.";
}
else if (!empty($_GET["exid"])) 
{
	$eid = $_GET["exid"];

	$stmt = $pdo->prepare("UPDATE error_log SET error_log.acknowledged=1 WHERE error_log.exception_id LIKE :eid");
	$stmt->bindValue(':eid', $eid, PDO::PARAM_INT);
	$stmt->execute();

	echo "Acknowledged $eid.";
}
else if (!empty($_GET["all"]) && $_GET["all"] == "true") 
{
	$stmt = $pdo->prepare("UPDATE error_log SET error_log.acknowledged=1");
	$stmt->execute();

	echo "Acknowledged All.";
}
else 
{
	echo "Error: Unknown Param";
}

echo "<script type=\"text/javascript\">setTimeout(function(){location.href = document.referrer;}, 500);</script>"

?>
