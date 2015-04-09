SET _=%~dp0
SET dest="%_%..\..\..\MyWebApplication\resources\js"
IF EXIST %dest% xcopy "%_%..\output" %dest% /Y
