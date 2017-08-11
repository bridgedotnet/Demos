mcs	/nostdlib \
	/warn:0 \
	/reference:".build/libs/Bridge.dll;.build/libs/Bridge.Html5.dll;.build/libs/Newtonsoft.Json.dll;" \
	/out:.build/libs/Demo.dll \
	/recurse:*.cs

mono .build/tools/bridge.exe -b .build/libs/Bridge.dll -lib .build/libs/Demo.dll
