BRIDGE ASSEMBLIES:
------------------
The following Bridge assemblies must be located in the Bridge\Builder folder to allow rebuild of stubs:
Bridge.dll
Bridge.Bootstrap3.dll
Bridge.jQuery2.dll
Bridge.Html5.dll
Bridge.WebGL.dll


WHEN BUILDING RELEASE:
----------------------
The release version of LiveApp.dll is copied here automatically by the following Build Event (right click the Live 
project > project > Build Events):

IF "$(ConfigurationName)" == "Release" (
   xcopy  "$(ProjectDir)\..\LiveApp\bin\Release\*.dll" "$(ProjectDir)\Bridge\Builder" /Y 
)
