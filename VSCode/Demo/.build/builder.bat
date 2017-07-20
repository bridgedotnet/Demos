start /wait C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe ^
	/nostdlib ^
	/warn:0 ^
	/reference:.build\Bridge.dll;.build\Bridge.Html5.dll ^
	/out:.build\Demo.dll ^
	/recurse:*.cs
	
start /b .build\bridge.exe -lib .build\Demo.dll
