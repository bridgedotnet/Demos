mcs	/nostdlib \
	/warn:0 \
	/reference:".build/Bridge.dll;.build/Bridge.Html5.dll" \
	/out:.build/Demo.dll \
	/recurse:*.cs

mono .build/bridge.exe -lib .build/Demo.dll
