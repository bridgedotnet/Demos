start /wait C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe ^
	/target:library ^
	/nostdlib+ ^
	/lib:build ^
	/r:Bridge.dll;Bridge.Html5.dll ^
	/out:bin\Demo.dll ^
	/recurse:*.cs

:: xcopy build\Bridge.dll bin\Bridge.dll

start build\Bridge.Builder.exe -p Demo.csproj -b build\Bridge.dll -o Bridge\output\`