start /wait C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe ^
	/target:library ^
	/nostdlib+ ^
	/warn:0 ^
	/lib:build ^
	/r:Bridge.dll;Bridge.Html5.dll ^
	/out:build\Demo.dll ^
	/recurse:*.cs
	
start /b build\Bridge.Builder.exe -p Demo.csproj -o Bridge\output\