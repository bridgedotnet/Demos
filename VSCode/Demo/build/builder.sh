#!/bin/bash

# Build using mono.
mcs	-nostdlib \
	-warn:0 \
	-reference:"${pkgdir}/lib/net45/Bridge.dll;${pkgdir}/lib/net45/Bridge.Html5.dll" \
	-out:build/Demo.dll \
	-recurse:*.cs

# Run Bridge builder to translate into JavaScript.
#mono ${pkgdir}/Bridge.Builder.exe -lib build/Demo.dll
mono build/Bridge.Builder.exe -lib build/Demo.dll