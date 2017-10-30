<?php require_once '../internals/backend.php'; ?>
<?php require_once '../internals/utils.php'; ?>
<?php require_once 'common/libadmin.php'; ?>
<?php init("admin"); ?>

<?php


global $pdo;

if (empty($_GET["newpw"]) || empty($_GET["id"]))
{
	echo "Error: Missing param";
	return;
}

$pw = $_GET["newpw"];
$id = $_GET["id"];

$user = GDUser::QueryByIDOrNull($pdo, $_GET['id']);

if ($user == null) {echo "user not found"; return; }


$stmt = $pdo->prepare("SELECT * FROM users WHERE userid=:id");
$stmt->bindValue(':id', $_GET['id'], PDO::PARAM_INT);
$stmt->execute();
$userdata = $stmt->fetch(PDO::FETCH_ASSOC);

$sha = strtoupper(hash('sha256', $pw));
$hash = password_hash($sha, PASSWORD_BCRYPT);

echo "<span style='min-width: 250px; display: inline-block;'>UserID:</span> " . $user->ID . "<br>\n";
echo "<span style='min-width: 250px; display: inline-block;'>User:</span> " . $user->Username . "<br>\n";
echo "<span style='min-width: 250px; display: inline-block;'>Current password:</span> " . $userdata['password_hash'] . "<br>\n";
echo "<span style='min-width: 250px; display: inline-block;'>New password (raw):</span> " . $pw . "<br>\n";
echo "<span style='min-width: 250px; display: inline-block;'>New password (SHA-256):</span> " . $sha . "<br>\n";
echo "<span style='min-width: 250px; display: inline-block;'>New password (HASH): </span>" . $hash . "<br>\n";
echo"<br />";

$stmt = $pdo->prepare("UPDATE users SET users.password_hash=:pwh WHERE users.userid = :uid");
$stmt->bindValue(':uid', $user->ID, PDO::PARAM_INT);
$stmt->bindValue(':pwh', $hash, PDO::PARAM_STR);
$stmt->execute();

echo "Password changed, redirecting in 20s";

echo "<script type=\"text/javascript\">setTimeout(function(){location.href = document.referrer;}, 20000);</script>"

?>
