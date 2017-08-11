START /wait .build\tools\csc.exe ^
	/nostdlib ^
	/warn:0 ^
	/reference:.build\lib\Bridge.dll;.build\lib\Bridge.Html5.dll;.build\lib\Newtonsoft.Json.dll; ^
	/recurse:*.cs

START .build\tools\bridge.exe -b .build\lib\Bridge.dll -lib .build\lib\Demo.dll