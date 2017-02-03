<!doctype html>

<html lang="en">
<head>
    <meta charset="utf-8">

    <link rel="stylesheet" href="pure-min.css"/>

    <style>
        #rootbox {
            text-align: left;
            max-width: 1200px;
            margin: 0 auto;
        }

        body {
            text-align: center;
        }

        .form {
            display: inline-block;
            background: rgb(238, 238, 238) none repeat scroll 0% 0%;
            padding: 5px;
            margin: 10px;
            min-width: 170px;
        }

        .form > * {
            display: block;
            margin: 2px 0px
        }

        .form > h3 {
            margin: 19px 0px
        }

        .form > button {
            margin-top: 10px
        }

        .flowbox {
            display: flex;
            flex-wrap: wrap;
        }

        #result {
            margin: 10px;
            width: calc(100% - 20px);
            height: 400px;
            background: black none repeat scroll 0% 0%;
            color: lime;
        }

        .sqltab {
            align-self: center;
        }

        #querypreview {
            display:flex;
            flex-direction: column;
            justify-content:center;
            margin-bottom: 10em;
        }

        .sqltab td {
            max-width: 100px;
            word-break: break-all;
        }
    </style>
</head>

<body>

<script src="jquery-3.1.0.min.js"></script>

<div id="rootbox">

    <div class="flowbox">
        <form class="form" data-apitarget="create-user">
            <h3>Create User</h3>

            Password:<br>             <input type="text" data-apiparam="password">
            Device Name:<br>          <input type="text" data-apiparam="device_name">
            Device Version:<br>       <input type="text" data-apiparam="device_version">
            App Version:<br>          <input type="text" data-apiparam="app_version">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="upgrade-user">
            <h3>Upgrade User</h3>

            UserID:<br>              <input type="text" data-apiparam="userid">
            Password (Old):<br>      <input type="text" data-apiparam="password_old">
            Password (New):<br>      <input type="text" data-apiparam="password_new">
            Username:<br>            <input type="text" data-apiparam="username_new">
            App Version:<br>         <input type="text" data-apiparam="app_version">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="ping">
            <h3>Ping</h3>

            UserID:<br>              <input type="text" data-apiparam="userid">
            Password:<br>            <input type="text" data-apiparam="password">
            App Version:<br>         <input type="text" data-apiparam="app_version">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="change-password">
            <h3>Change Password</h3>

            UserID:<br>              <input type="text" data-apiparam="userid">
            Password (Old):<br>      <input type="text" data-apiparam="password_old">
            Password (New):<br>      <input type="text" data-apiparam="password_new">
            App Version:<br>         <input type="text" data-apiparam="app_version">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="set-score">
            <h3>Set Score</h3>

            UserID:<br>              <input type="text" data-apiparam="userid">
            Password:<br>            <input type="text" data-apiparam="password">
            App Version:<br>         <input type="text" data-apiparam="app_version">
            Level ID:<br>            <input type="text" data-apiparam="levelid">
            Difficulty:<br>          <input type="text" data-apiparam="difficulty">
            Level Time:<br>          <input type="text" data-apiparam="leveltime">
            Total Score:<br>         <input type="text" data-apiparam="totalscore">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>
    </div>


    <textarea id="result"></textarea>

</div>

<div id="querypreview">
	<?php require('query.php'); ?>
</div>

<script type="text/javascript">

    function apicall(sender) {
        let form = $(sender).closest('.form');
        let url = "../" + form.attr('data-apitarget') + ".php?msgk=DEBUG";

        form.children('input').each(function() {
            url = url + "&" + $(this).attr('data-apiparam') + "=" + $(this).val();
        });

        jQuery.get(url, undefined, function(data)
        {
            $("#result").val(url + "\n\n" + data);

            jQuery.get("query.php", undefined, function(data)
            {

                $("#querypreview").html(data);

            }, "text");

        }, "text");
    }

</script>

</body>
</html>




