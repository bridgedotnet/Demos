start /wait xcopy build\*.dll bin\ /Y

start /wait C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe ^
	/target:library ^
	/nostdlib+ ^
	/lib:build ^
	/r:Bridge.dll;Bridge.Html5.dll ^
	/out:bin\Demo.dll ^
	/recurse:*.cs
	
start build\Bridge.Builder.exe -p Demo.csproj -b build\Bridge.dll -o Bridge\output\`