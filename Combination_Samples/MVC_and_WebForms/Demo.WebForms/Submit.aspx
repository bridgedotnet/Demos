<%@ Page Language="C#" %>

<!DOCTYPE html>
<html>
<head>
    <title>Bridge.NET</title>

    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="resources/css/demo.css" />
</head>
<body style="padding: 20px;">
    <div class="col-md-6">
        <div class="panel panel-default">
            <div class="panel-heading logo">
                <h1>Bridge.NET Demo</h1>
            </div>
            <div class="panel-body">
                <div class="alert alert-success">The message has been successfully submitted.</div>

                <h3>
                    <span class="label label-info">Name: <%= Request.Params["name"] %></span>
                </h3>

                <h3>
                    <span class="label label-info">Email: <%= Request.Params["email"] %></span>
                </h3>

                <h3>
                    <span class="label label-info">Message: <%= Request.Params["message"] %></span>
                </h3>

                <h3>
                    <span class="label label-info">Server Date and Time: <%= Request.Params["datetime"] %></span>
                </h3>
            </div>
            <div class="panel-footer" style="text-align: right;">
                <button class="btn btn-primary" onclick="window.history.back();">Back</button>
            </div>
        </div>
    </div>
</body>
</html>
