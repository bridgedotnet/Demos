mcs	/nostdlib \
	/warn:0 \
	/reference:".build/libs/Bridge.dll;.build/libs/Bridge.Html5.dll" \
	/out:.build/Demo.dll \
	/recurse:*.cs

mono .build/tools/bridge.exe -b .build/libs/Bridge.dll -lib .build/Demo.dll
