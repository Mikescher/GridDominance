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
            width: calc(100% - 50px);
            height: 400px;
            background: black none repeat scroll 0% 0%;
            color: lime;
        }

        .sqltab {
            align-self: center;
        }

        .tab_prev {
            visibility: collapse;
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
<script type="text/javascript" src="sha256.js"></script>

<div id="rootbox">

    <div class="flowbox">
        <form class="form" data-apitarget="create-user">
            <h3>Create User</h3>

            Password:<br>            <input type="text" data-apiparam="password"             data-apiformat="enc" data-apisig="true">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str" data-apisig="true">
            Device Name:<br>         <input type="text" data-apiparam="device_name"          data-apiformat="str" data-apisig="true">
            Device Version:<br>      <input type="text" data-apiparam="device_version"       data-apiformat="str" data-apisig="true">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="upgrade-user">
            <h3>Upgrade User</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int" data-apisig="true">
            Password (Old):<br>      <input type="text" data-apiparam="password_old"         data-apiformat="enc" data-apisig="true">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str" data-apisig="true">
            Password (New):<br>      <input type="text" data-apiparam="password_new"         data-apiformat="enc" data-apisig="true">
            Username:<br>            <input type="text" data-apiparam="username_new"         data-apiformat="str" data-apisig="true">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="ping">
            <h3>Ping</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int" data-apisig="true">
            Password:<br>            <input type="text" data-apiparam="password"             data-apiformat="enc" data-apisig="true">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str" data-apisig="true">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="download-data">
            <h3>Download Data</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int" data-apisig="true">
            Password:<br>            <input type="text" data-apiparam="password"             data-apiformat="enc" data-apisig="true">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str" data-apisig="true">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="change-password">
            <h3>Change Password</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int" data-apisig="true">
            Password (Old):<br>      <input type="text" data-apiparam="password_old"         data-apiformat="enc" data-apisig="true">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str" data-apisig="true">
            Password (New):<br>      <input type="text" data-apiparam="password_new"         data-apiformat="enc" data-apisig="true">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="set-score">
            <h3>Set Score</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int" data-apisig="true">
            Password:<br>            <input type="text" data-apiparam="password"             data-apiformat="enc" data-apisig="true">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str" data-apisig="true">
            Level ID:<br>            <input type="text" data-apiparam="levelid"              data-apiformat="str" data-apisig="true">
            Difficulty:<br>          <input type="text" data-apiparam="difficulty"           data-apiformat="int" data-apisig="true">
            Level Time:<br>          <input type="text" data-apiparam="leveltime"            data-apiformat="int" data-apisig="true">
            Total Score:<br>         <input type="text" data-apiparam="totalscore"           data-apiformat="int" data-apisig="true">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="log-client">
            <h3>Log Client</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int" data-apisig="true">
            Password:<br>            <input type="text" data-apiparam="password"             data-apiformat="enc" data-apisig="true">
            App Version:<br>         <input type="text" data-apiparam="app_version"          data-apiformat="str" data-apisig="true">
            Screen Resolution:<br>   <input type="text" data-apiparam="screen_resolution"    data-apiformat="str" data-apisig="true">
            Identifier:<br>          <input type="text" data-apiparam="exception_id"         data-apiformat="str" data-apisig="true">
            Message:<br>             <input type="text" data-apiparam="exception_message"    data-apiformat="b64" data-apisig="true">
            Stacktrace:<br>          <input type="text" data-apiparam="exception_stacktrace" data-apiformat="b64" data-apisig="true">
            Additional:<br>          <input type="text" data-apiparam="additional_info"      data-apiformat="b64" data-apisig="true">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="cron">
            <h3>Cron</h3>

            Secret:<br>              <input type="text" data-apiparam="cronsecret" value="cron" data-apiformat="str">

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="get-highscores">
            <h3>Get Highscores</h3>

            <button type="button" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="get-ranking">
            <h3>Get Ranking</h3>

            UserID:<br>              <input type="text" data-apiparam="userid"               data-apiformat="int" data-apisig="true">
            World ID:<br>            <input type="text" data-apiparam="world_id"              data-apiformat="int" data-apisig="true">

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
        let url = "../" + form.attr('data-apitarget') + ".php";
        let urlparams = '';

        let sigBuilder = 'OZothFoshCiHyPhebMyGheVushNopTyg';

        form.children('input').each(function() {
            let v = $(this).val();

            if ($(this).attr('data-apiformat') === 'b64') {
                if ($(this).attr('data-apisig')) sigBuilder = sigBuilder + "\n" + v;

                v = WTKBase64.encode(v);

                v = replaceAll(v, '+', '-');
                v = replaceAll(v, '\\', '_');
                v = replaceAll(v, '=', '.');
            } else if ($(this).attr('data-apiformat') === 'enc') {
                let shaObj = new jsSHA("SHA-256", "TEXT");
                shaObj.update(v);
                v = shaObj.getHash("HEX").toUpperCase();

                if ($(this).attr('data-apisig')) sigBuilder = sigBuilder + "\n" + v;
            } else {

                if ($(this).attr('data-apisig')) sigBuilder = sigBuilder + "\n" + v;
            }

            urlparams = urlparams + "&" + $(this).attr('data-apiparam') + "=" + v;
        });
        let shaObj = new jsSHA("SHA-256", "TEXT");
        shaObj.update(sigBuilder);
        let sig = shaObj.getHash("HEX");

        url = url + "?msgk=" + sig + urlparams;

        $("#result").val(url);

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




