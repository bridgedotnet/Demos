WHEN BUILDING RELEASE:
----------------------
The release version of LiveApp.dll is copied here automatically by the following Build Event (right click the Live 
project > project > Build Events):

IF "$(ConfigurationName)" == "Release" (
   xcopy  "$(ProjectDir)\..\LiveApp\bin\Release\*.dll" "$(ProjectDir)\Bridge\Builder" /Y 
)