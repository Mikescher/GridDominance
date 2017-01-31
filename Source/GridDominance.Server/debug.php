<!doctype html>

<html lang="en">
<head>
    <meta charset="utf-8">

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
    </style>
</head>

<body>

<script src="https://code.jquery.com/jquery-3.1.0.min.js" integrity="sha256-cCueBR6CsyA4/9szpPfrX3s49M9vUU5BgtiJj06wt/s=" crossorigin="anonymous"></script>

<div id="rootbox">

    <div class="flowbox">
        <form class="form" data-apitarget="create_user">
            <h3>Create_User</h3>

            Password:<br>      <input type="text" data-apiparam="password">
            Device Name:<br>   <input type="text" data-apiparam="device_name">
            Device Version:<br><input type="text" data-apiparam="device_version">

            <button type="button" id="btnCreate" onclick="apicall(this);">Query</button>
        </form>

        <form class="form" data-apitarget="upgrade_user">
            <h3>Upgrade_User</h3>

            UserID:<br>              <input type="text" data-apiparam="userid">
            Password (Old):<br>      <input type="text" data-apiparam="password_old">
            Password (New):<br>      <input type="text" data-apiparam="password_new">
            Username:<br>            <input type="text" data-apiparam="username_new">

            <button type="button" id="btnCreate" onclick="apicall(this);">Query</button>
        </form>
    </div>


    <textarea id="result"></textarea>


</div>

<script type="text/javascript">

    function apicall(sender) {
        form = $(sender).closest('.form');
        url = form.attr('data-apitarget') + ".php?msgk=DEBUG";

        form.children('input').each(function() {
            url = url + "&" + $(this).attr('data-apiparam') + "=" + $(this).val();
        });

        jQuery.get(url, undefined, function(data) { $("#result").val(url + "\n\n" + data); }, "text");
    }

</script>

</body>
</html>




