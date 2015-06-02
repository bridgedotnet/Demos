WHEN BUILDING RELEASE:
----------------------
The release version of LiveBridgeBuilder.dll is copied here automatically by the following Build Event (right click www 
project > project > Build Events):

IF "$(ConfigurationName)" == "Release" (
   xcopy  "$(ProjectDir)\..\LiveBridgeBuilder\bin\Release\*.dll" "$(ProjectDir)\BridgeTranslator\Builder" /Y 
)