#!/bin/bash

pkgbasedir="${HOME}/.dnx/packages/Bridge"
lastver="$(ls "${pkgbasedir}" | sort -n | tail -n 1)"
if [ -z "${lastver}" ]; then
 echo "ERROR: Unable to locate Bridge package in local packages' directory."
 echo "Did you run 'dnu restore'?"
 exit 1
fi

pkgdir="/${pkgbasedir}/${lastver}"

if [ ! -d "${pkgdir}" ]; then
 echo "ERROR: Unable to locate Bridge package in local packages directory."
 echo "Did you run 'dnu restore'?"
 exit 1
fi

# Build using mono.
mcs	-nostdlib \
	-warn:0 \
	-reference:"${pkgdir}/lib/net45/Bridge.dll;${pkgdir}/lib/net45/Bridge.Html5.dll" \
	-out:build/Demo.dll \
	-recurse:*.cs

# Run Bridge builder to translate into JavaScript.
#mono ${pkgdir}/Bridge.Builder.exe -lib build/Demo.dll
mono build/Bridge.Builder.exe -lib build/Demo.dll
