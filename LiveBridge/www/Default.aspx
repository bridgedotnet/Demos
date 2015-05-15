<%@ Page
    Language="C#"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="LiveBridge.Default"
    ValidateRequest="false" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Live Bridge Compiler</title>
    
    <link href="resources/live.css" type="text/css" rel="stylesheet" />
 
    <script src="https://code.jquery.com/jquery-1.11.3.min.js"></script>
    <script type="text/javascript" src="resources/js/live.js"></script>
</head>
<body>
    <div class="Header1">
        <img src="resources/images/bridgedotnet-sh.png" />
    </div>
    <div class="Header2">
        <h1>Live Compiler</h1>
    </div>
    <div class="Panel">
        <div style="padding-left: 400px; position: absolute;">
            <button id="btnTranslate" title="Translate" onclick="btnTranslate_click(event);">Translate</button>
            <button id="btnRun" title="Run" onclick="btnRun_click(event);">Run</button>
            <span id="progress" style="margin-left: 10px" />            
        </div>
        <table class="CodeTable" cellspacing="0" cellpadding="0" border="0" style="table-layout: fixed;">
            <tr>
                <th style="padding: 5px">C#</th>
                <th>JavaScript</th>
            </tr>
            <tr>
                <td class="CodeCell Left">
                    <div id="CsEditor" class="CodeEditor ace_editor ace-tm Active" style="height: 400px; width: 465px; position: relative;">public class App
{
    [Ready]
    public static void Main()
    {
        Console.WriteLine("Hello Bridge.NET");
    }
}</div>
                </td>
                <td class="CodeCell Right">                    
                    <div id="JsEditor" class="CodeEditor ace_editor ace-tm Active" style="height: 400px; width: 465px; position: relative;">
                    </div>
                    <input type="hidden" id="hash"/>
                </td>
            </tr>
        </table>
    </div>

    <script src="resources/js/ace/ace.js"></script>

    <script>
        var csEditor = ace.edit("CsEditor"),
            jsEditor;

        csEditor.getSession().setMode("ace/mode/csharp");
        jsEditor = ace.edit("JsEditor");
        jsEditor.getSession().setMode("ace/mode/javascript");
        jsEditor.setValue("");
    </script>
</body>
</html>
