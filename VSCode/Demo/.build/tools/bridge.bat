start /wait C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe ^
	/nostdlib ^
	/warn:0 ^
	/reference:.build\libs\Bridge.dll;.build\libs\Bridge.Html5.dll ^
	/out:.build\Demo.dll ^
	/recurse:*.cs
	
start .build\tools\bridge.exe -b .build\libs\Bridge.dll -lib .build\Demo.dll