Bridge.Class.define('Demo.BootstrapApp', {
    statics: {
        config:  {
            init: function () {
                $(this.main);
            }
        },
        main: function () {
            document.body.style.padding = "20px";
            $("<form>").addClass("col-md-6").append(Demo.BootstrapApp.createPanel()).appendTo(document.body);
            Demo.BootstrapApp.updateButton_Click(null);
            Demo.BootstrapApp.listenToBootstrapEvents();
        },
        createPanel: function () {
            return $("<div>").addClass("panel panel-default").append(Demo.BootstrapApp.createPanelHeader()).append(Demo.BootstrapApp.createPanelBody()).append(Demo.BootstrapApp.createPanelFooter());
        },
        createPanelHeader: function () {
            return $("<div>").addClass("panel-heading logo").append($("<h1>").html("Bridge.NET Bootstrap Demo"));
        },
        createPanelBody: function () {
            return $("<div>").addClass("panel-body collapse in").append(Demo.BootstrapApp.createFormField("Name", "user")).append(Demo.BootstrapApp.createFormField("Email", "globe")).append(Demo.BootstrapApp.createFormField("Message", "pencil")).append(Demo.BootstrapApp.createDateTimeField());
        },
        createPanelFooter: function () {
            return $("<div>").addClass("panel-footer text-right").append(Demo.BootstrapApp.createProcessButton()).append($(Bridge.merge(document.createElement('button'), {
                className: "btn btn-primary", 
                innerHTML: "Collapse", 
                style: {
                    marginRight: "10px"
                }
            } )).on("click", function (e) {
                $(".panel-body").collapse('toggle');
                $(this).text($(this).text() === "Collapse" ? "Expand" : "Collapse");
                e.preventDefault();
            })).append($(Bridge.merge(document.createElement('button'), {
                className: "btn btn-primary", 
                innerHTML: "Toggle Tooltips and Popovers", 
                style: {
                    marginRight: "10px"
                }
            } )).on("click", function (e) {
                $("[rel=\"popover\"]").popover('toggle');
                $("[rel=\"tooltip\"]").tooltip("toggle");
                e.preventDefault();
            })).append($(Bridge.merge(document.createElement('input'), {
                type: "submit", 
                value: "Submit", 
                className: "btn btn-success", 
                formAction: Demo.Config.SUBMIT_URL, 
                formMethod: "POST"
            } )).attr("rel", "tooltip"));
        },
        createProcessButton: function () {
            return $(Bridge.merge(document.createElement('button'), {
                className: "btn btn-primary", 
                innerHTML: "Async Process", 
                style: {
                    marginRight: "10px"
                }
            } )).on("click", function (e) {
                var $step = 0,
                    $task1, 
                    $taskResult1, 
                    $task2, 
                    e, 
                    i, 
                    title, 
                    intervalId, 
                    result, 
                    msg, 
                    modal, 
                    $asyncBody = function () {
                        for (;;) {
                            switch ($step) {
                                case 0: {
                                    e.preventDefault();
                                    Demo.BootstrapApp.createModalWithProgressBar().appendTo(document.body).modal({ backdrop: "static" });
                                    i = 0;
                                    title = $(".modal-title");
                                    intervalId = window.setInterval(function () {
                                        var percent = (i++ % 20) * 10;
                                        $(".progress-bar").attr("aria-valuenow", percent).attr("style", "width:" + percent + "%;");
                                        title.html("Long running process: " + percent + "% complete");
                                    }, 490);
                                    $task1 = Bridge.Task.fromPromise($.ajax({ url: Demo.Config.LONG_RUNNING_PROCESS, cache: false }), (function () {
                                        return arguments[0].toString();
                                    }));
                                    $step = 1;
                                    $task1.continueWith($asyncBody);
                                    return;
                                }
                                case 1: {
                                    $taskResult1 = $task1.getResult();
                                    result = $taskResult1;
                                    msg = JSON.parse(result);
                                    if (msg === "ok") {
                                        window.clearInterval(intervalId);
                                        $(".progress-bar").attr("aria-valuenow", 100).attr("style", "width: 100%;").html("Done");
                                        $(".modal-title").html("Success!");
                                        $task2 = Bridge.Task.delay(2000);
                                        $step = 2;
                                        $task2.continueWith($asyncBody);
                                        return;
                                    } 
                                    $step = 3;
                                    continue;
                                }
                                case 2: {
                                    $task2.getResult();
                                    modal = $(".modal").on("hidden.bs.modal", function () {
                                        $(this).remove();
                                    }).modal('hide');
                                    $step = 3;
                                    continue;
                                }
                                case 3: {
                                    return;
                                }
                                default: {
                                    return;
                                }
                            }
                        }
                    };

                $asyncBody();
            });
        },
        createModalWithProgressBar: function () {
            return $("<div>").addClass("modal fade").append($("<div>").addClass("modal-dialog").append($("<div>").addClass("modal-content").append($("<div>").addClass("modal-header").append($("<h4>").addClass("modal-title").html("Long running process..."))).append($("<div>").addClass("modal-body").append($("<div>").addClass("progress").append($("<div>").addClass("progress-bar").attr("role", "progressbar").attr("aria-valuemin", "0").attr("aria-valuemax", "100"))))));
        },
        createFormField: function (name, glyph) {
            var input;
            var placeholder = name + "...";
            if (name === "Message") {
                input = Bridge.merge(document.createElement('textarea'), {
                    name: name.toLowerCase(), 
                    placeholder: placeholder, 
                    required: true, 
                    className: "form-control"
                } );
            }
            else  {
                input = Bridge.merge(document.createElement('input'), {
                    type: "text", 
                    name: name.toLowerCase(), 
                    placeholder: placeholder, 
                    required: true, 
                    className: "form-control"
                } );
            }
            return $("<div>").addClass("input-group").css("margin-bottom", "10px").append(Bridge.merge(document.createElement('span'), {
                className: "glyphicon glyphicon-" + glyph + " input-group-addon"
            } )).append($(input).attr("rel", "tooltip").tooltip({ title: "Required Field", container: "body", placement: "right" }));
        },
        createDateTimeField: function () {
            return [Bridge.merge(document.createElement('label'), {
                htmlFor: "dateTimeInput", 
                innerHTML: "Server Date and Time:"
            } ), $(Bridge.merge(document.createElement('div'), {
                className: "input-group"
            } )).append(Bridge.merge(document.createElement('span'), {
                className: "input-group-addon glyphicon glyphicon-time"
            } )).append($(Bridge.merge(document.createElement('input'), {
                id: "dateTimeInput", 
                type: "text", 
                className: "form-control", 
                readOnly: true, 
                name: "datetime"
            } )).attr("rel", "popover").popover({ content: "This is a ReadOnly Field", container: "body", trigger: "hover", placement: "top" })).append($(Bridge.merge(document.createElement('span'), {
                className: "input-group-btn"
            } )).append($(Bridge.merge(document.createElement('button'), {
                type: "button", 
                className: "btn btn-primary", 
                innerHTML: "<span class=\"glyphicon glyphicon-refresh\"></span>"
            } )).attr("data-loading-text", "Loading...").attr("rel", "popover").on("click", Demo.BootstrapApp.updateButton_Click).popover({ title: "Get the server Date and Time", content: "Clicking this Button initiates an AJAX request to the server", container: "body", trigger: "hover" })))];
        },
        updateButton_Click: function (e) {
            var $step = 0,
                $task1, 
                btn, 
                delay, 
                $asyncBody = function () {
                    for (;;) {
                        switch ($step) {
                            case 0: {
                                btn = null;
                                delay = 0;
                                if (e !== null) {
                                    btn = $(e.currentTarget);
                                    btn.button('loading');
                                    delay = 2000;
                                } 
                                // Delaying is to demonstrate the Button Loading state functionality of Bootstrap
                                $task1 = Bridge.Task.delay(delay);
                                $step = 1;
                                $task1.continueWith($asyncBody);
                                return;
                            }
                            case 1: {
                                $task1.getResult();
                                $.ajax({ url: Demo.Config.GET_SERVER_TIME_URL, cache: false, success: function (obj, str, jqXHR) {
                                    if (btn !== null) {
                                        btn.button('reset');
                                    }
                                    var val = JSON.parse(Bridge.cast(obj, String));
                                    var dateTime = Bridge.Date.parse(val);
                                    $("#dateTimeInput").val(Bridge.Date.format(dateTime, "yyyy-MM-dd hh:mm:ss"));
                                } });
                                return;
                            }
                            default: {
                                return;
                            }
                        }
                    }
                };

            $asyncBody();
        },
        showAlert: function (message) {
            var alert = $("<div>").addClass("alert alert-info col-md-4").attr("role", "alert").attr("style", "display: block; position: relative;").html(message).appendTo(document.body);
            window.setTimeout(function () {
                return alert.remove();
            }, 3000);
        },
        listenToBootstrapEvents: function () {
            $(".collapse").on("hidden.bs.collapse", function () {
                Demo.BootstrapApp.showAlert("The Panel's body has been collapsed");
            }).on("shown.bs.collapse", function () {
                return Demo.BootstrapApp.showAlert("The Panel's body has been expanded");
            });
        }
    }
});

