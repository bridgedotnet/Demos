SET _=%~dp0
xcopy "%_%..\output\*.js" "%_%..\..\..\Demo.MVC\resources\js" /Y
xcopy "%_%..\output\*.js" "%_%..\..\..\Demo.WebForms\resources\js" /Y