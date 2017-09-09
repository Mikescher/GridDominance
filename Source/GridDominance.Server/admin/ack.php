<?php require_once '../internals/backend.php'; ?>
<?php require_once '../internals/utils.php'; ?>
<?php init("admin"); ?>

<?php


global $pdo;

$nojs=0;
if (!empty($_GET["nojs"])) $nojs = $_GET["sim"];

if (empty($_GET["sim"])) 
{
	echo "Error: Missing param";
	return;
}

$sim = $_GET["sim"];

if ($sim == 1)
{
	$id = $_GET["id"];

	$stmt = $pdo->prepare("UPDATE error_log SET error_log.acknowledged=1 WHERE error_log.error_id = :id");
	$stmt->bindValue(':id', $id, PDO::PARAM_INT);
	$stmt->execute();

	echo "Acknowledged $id. (" . $stmt->rowCount() . " affected)";
}
else if ($sim == 2)
{
	$eid = $_GET["exid"];

	$stmt = $pdo->prepare("UPDATE error_log SET error_log.acknowledged=1 WHERE error_log.exception_id LIKE :eid");
	$stmt->bindValue(':eid', $eid, PDO::PARAM_INT);
	$stmt->execute();

	echo "Acknowledged $eid. (" . $stmt->rowCount() . " affected)";
}
else if ($sim == 3)
{
	$eid = $_GET["exid"];
	$ems = $_GET["exmsg"];

	$stmt = $pdo->prepare("UPDATE error_log SET error_log.acknowledged=1 WHERE error_log.exception_id LIKE :eid AND error_log.exception_message LIKE :ems");
	$stmt->bindValue(':eid', $eid, PDO::PARAM_INT);
	$stmt->bindValue(':ems', $ems, PDO::PARAM_STR);
	$stmt->execute();

	echo "Acknowledged $eid + message. (" . $stmt->rowCount() . " affected)";
}
else if ($sim == 99)
{
	$stmt = $pdo->prepare("UPDATE error_log SET error_log.acknowledged=1 WHERE error_log.acknowledged<>1");
	$stmt->execute();

	echo "Acknowledged all (" . $stmt->rowCount() . " affected)";
}
else 
{
	echo "Error: Unknown Sim";
	return;
}

if ($nojs == 0) echo "<script type=\"text/javascript\">setTimeout(function(){location.href = document.referrer;}, 500);</script>"

?>
