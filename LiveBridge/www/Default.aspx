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
    <style>
        body {
            font-family: "Open Sans", sans-serif;
            font-size: 13px;
            margin: 0;
            padding: 0;
        }

        .CodeTable {
            width: 100%;
            border-collapse: collapse;
        }

        .CodeCell {
            padding-bottom: 10px;
            padding-top: 3px;
        }

            .CodeCell.Left {
                padding-right: 5px;
            }

            .CodeCell Right {
                padding-left: 5px;
            }

        .CodeEditor {
            background-color: #FFF;
        }

            .CodeEditor.Active {
                border: 1px solid #2C7898;
            }

        .ace_editor {
            overflow: hidden;
            font-family: "Monaco", "Menlo", "Courier New", monospace;
            font-size: 12px;
        }

        .Panel {
            width: 950px;
            margin: auto;
        }

        .Header1 {
            min-height: 40px;
            padding-top: 10px;
        }

        .Header2 {
            height: 70px;
            background: url("http://bridge.net/wp-content/themes/bridge/img/bg-pattern-light.svg") no-repeat scroll center center / cover #5095BA;
            text-align: center;
            padding: 5px 0px;
            color: white;
            margin-bottom: 20px;
        }

        h1 {
            font-weight: 300 !important;
            font-size: 30px;
            color: #FFF !important;
        }
    </style>

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
            <span id="progress" style="margin-left: 10px" />            
        </div>
        <table class="CodeTable" cellspacing="0" cellpadding="0" border="0" style="table-layout: fixed;">
            <tr>
                <th style="padding: 5px">C#</th>
                <th>JavaScript <a href="run.html" target="_blank">Run</a></th>
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
