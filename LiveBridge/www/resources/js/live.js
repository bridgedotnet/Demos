// Live Bridge.NET 

function translate(cs, complete) {
    $.ajax({
        url: "Default.aspx?ajax=1",
        data: {
            cs: cs
        },
        success: function (result) {
            complete(result);
        },
        type: "POST"
    });
}

function btnTranslate_click(e) {
    progress("Compiling...");
    var cs = csEditor.getValue();
    translate(cs, complete);
}

function complete(result) {
    progress(null);
    if (!result.Success) {
        jsEditor.setValue(result.ErrorMessage);
        Log("Finished with Error(s)");
    }
    else {
        jsEditor.setValue(result.JsCode);
        progress("Finished");
    }
}

function progress(message) {
    $("#progress").text((message != null ? message : ""));
};