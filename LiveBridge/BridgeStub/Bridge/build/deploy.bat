SET _=%~dp0
SET dest="%_%..\..\..\www\resources\js\bridge"
IF EXIST %dest% xcopy "%_%..\output" %dest% /Y
