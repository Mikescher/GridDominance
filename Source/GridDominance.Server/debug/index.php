<?php $c=require('../internals/config.php'); if (!$c['debug']) exit('Think of how stupid the average person is, and realize half of them are stupider than that.'); ?>
<!doctype html>

<html lang="en">
<head>
    <meta charset="utf-8">

    <link rel="stylesheet" href="pure-min.css"/>

    <style>
        #rootbox {
            text-align: left;
            //max-width: 1200px;
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
            min-width: 150px;
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
            max-width: 128px;
            word-wrap: break-word;
        }
    </style>
</head>

<body>

<script src="jquery-3.1.0.min.js"></script>

<script src="webtoolkit.base64.js"></script>

<script type="text/javascript" src="jsbn.js"></script>
<script type="text/javascript" src="jsbn2.js"></script>
<script type="text/javascript" src="rsa.js"></script>

<div id="rootbox">

    <div class="flowbox">
        <form class="form" data-apitarget="create-user">
            <h3>Create User</h3>

            Password:<br>            <input type="text" data-apiparam="password"             data-apiformat="ppk">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str">
            Device Name:<br>         <input type="text" data-apiparam="device_name"          data-apiformat="str">
            Device Version:<br>      <input type="text" data-apiparam="device_version"       data-apiformat="str">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="upgrade-user">
            <h3>Upgrade User</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int">
            Password (Old):<br>      <input type="text" data-apiparam="password_old"         data-apiformat="ppk">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str">
            Password (New):<br>      <input type="text" data-apiparam="password_new"         data-apiformat="ppk">
            Username:<br>            <input type="text" data-apiparam="username_new"         data-apiformat="str">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="ping">
            <h3>Ping</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int">
            Password:<br>            <input type="text" data-apiparam="password"             data-apiformat="ppk">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="download-data">
            <h3>Download Data</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int">
            Password:<br>            <input type="text" data-apiparam="password"             data-apiformat="ppk">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="change-password">
            <h3>Change Password</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int">
            Password (Old):<br>      <input type="text" data-apiparam="password_old"         data-apiformat="ppk">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str">
            Password (New):<br>      <input type="text" data-apiparam="password_new"         data-apiformat="ppk">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="set-score">
            <h3>Set Score</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int">
            Password:<br>            <input type="text" data-apiparam="password"             data-apiformat="ppk">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str">
            Level ID:<br>            <input type="text" data-apiparam="levelid"              data-apiformat="str">
            Difficulty:<br>          <input type="text" data-apiparam="difficulty"           data-apiformat="int">
            Level Time:<br>          <input type="text" data-apiparam="leveltime"            data-apiformat="int">
            Total Score:<br>         <input type="text" data-apiparam="totalscore"           data-apiformat="int">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="log-client">
            <h3>Log Client</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int">
            Password:<br>            <input type="text" data-apiparam="password"             data-apiformat="ppk">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str">
            Screen Resolution:<br>   <input type="text" data-apiparam="screen_resolution"    data-apiformat="str">
            Identifier:<br>          <input type="text" data-apiparam="exception_id"         data-apiformat="str">
            Message:<br>             <input type="text" data-apiparam="exception_message"    data-apiformat="b64">
            Stacktrace:<br>          <input type="text" data-apiparam="exception_stacktrace" data-apiformat="b64">
            Additional:<br>          <input type="text" data-apiparam="additional_info"      data-apiformat="b64">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="cron">
            <h3>Cron</h3>

            Secret:<br>              <input type="text" data-apiparam="cronsecret"           data-apiformat="str">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="get-highscores">
            <h3>Get Highscores</h3>

            <button type="button" onclick="apicall(this);">Query</button>
        </form>
    </div>


    <textarea id="result"></textarea>

</div>

<div id="querypreview">
	<?php require('query.php'); ?>
</div>

<script type="text/javascript">

    function escapeRegExp(str) {
        return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
    }

    function replaceAll(str, find, replace) {
        return str.replace(new RegExp(escapeRegExp(find), 'g'), replace);
    }

    function getURL(url){
        return $.ajax({
            type: "GET",
            url: url,
            cache: false,
            async: false
        }).responseText;
    }

    function apicall(sender) {
        let form = $(sender).closest('.form');
        let url = "../" + form.attr('data-apitarget') + ".php?msgk=TODO";

        form.children('input').each(function() {
            let v = $(this).val();

            if ($(this).attr('data-apiformat') === 'b64') {
                v = WTKBase64.encode_urlsafe(v);
            } else if ($(this).attr('data-apiformat') === 'ppk') {
                let keystr = getURL('parameterkey.public');
                let key = RSA.getPublicKey(keystr)
                v = RSA.encrypt(v, key);

                v = replaceAll(v, '+', '-');
                v = replaceAll(v, '\\', '_');
                v = replaceAll(v, '=', '.');
            }

            url = url + "&" + $(this).attr('data-apiparam') + "=" + v;
        });

        jQuery.get(url, undefined, function(data)
        {
            try {
                $("#result").val(url + "\n\n" + JSON.stringify(JSON.parse(data), null, 4));
            } catch (e) {
                $("#result").val(url + "\n\n" + data);
            }

            jQuery.get("query.php", undefined, function(data)
            {

                $("#querypreview").html(data);

            }, "text");

        }, "text");
    }

</script>

</body>
</html>




