How to successfully open this on VSCode

-= Requirements =-
==================

- Windows 7+, OS X or Linux operating system.
- Building environment:
 - If Windows, Visual Studio 2013 + .NET 4.0 (msbuild)
 - If OS X or Linux, Mono (xbuild) http://www.mono-project.com/
- Visual Studio Code - http://code.visualstudio.com/

-= Opening the project =-
=========================

On VSCode, you cannot just open the .sln file. You open a 'folder' con-
taining it.

Before opening this project on Visual Studio Code, you need to manually
restore the NuGet packages. For that, supposing you have Mono installed on
your system, you just need to:

$ nuget restore

Run this command on the same directory where the 'CSProjDemo.sln' file is
and it should fix the NuGet packages for you.

Again, DON'T OPEN THE .SLN FILE with VSCode. Instead, open THE
FOLDER where CSProjDemo.sln is. This way, VSCode will scan the folder for
supported project types and finally load the .sln file, enabling intelli-
sense correctly and enabling you to build the project using 'xbuild'.

On Windows platform, MSBuild from microsoft .NET is used. On the other
platforms, xbuild from mono (www.mono-project.com) is used.

-= Building the solution =-
===========================

This project comes pre-shipped with the build task necessary to build it
using 'xbuild' on the solution file ('msbuild' if windows).

Once loaded on Visual Studio Code, to build the solution run the build
task (shift+command+B on OS X and ctrl+shift+B anywhere else -- or via
the VSCode's command palette).

-= Testing the results =-
=========================

Once built, you can check the result simply by opening
'CSProjDemo/Demo/Bridge/www/demo.html' on your favorite web browser.

-= Additional support =-
========================

This project is also compatible with Visual Studio (2013) and also works
with Xamarin Studio (MonoDevelop), so it works with VSCode on Windows,
Linux and OS X, and also with Visual Studio on Windows, and Xamarin Stu-
dio on the platforms it runs.

Xamarin Studio (or MonoDevelop) can be obtained from:
http://www.mono-project.com/

-= Troubleshooting =-
=====================

If you cannot build this on Windows because it can't find 'msbuild',
locate the right directory on your machine (or note if it is on %PATH%)
and edit '.settings\tasks.json'. About line 21, there's the reference
to 'msbuild'. If you have it in your path, you can just change it to
'msbuild.exe' without path, or update the path to the correct one.

If you cannot build this on Linux or OS X because it can't find 'xbuild',
the recommended action is to load Mono environment variables as other
things might break if not set up correctly. If your mono installation is
not on path (/opt, for example), there might be a 'env.sh' file to load
this information.

The guide for installing mono on OS X is here:
http://www.mono-project.com/docs/getting-started/install/mac/

You may also use HomeBrew to install packages on OS X (community software
package manager for OS X).
- HomeBrew Installation: http://brew.sh/
- Mono Installation: http://brewformulas.org/Mono

For linux, it depends on your distribution, so a starting point is here:
http://www.mono-project.com/download/#download-lin

-= More information =-
======================

For anything not covered in this document, please refer to:
- Bridge.NET Knowledge base: http://bridge.net/kb/
- Bridge.NET Forums: http://forums.bridge.net/
- Bridge.NET Github Issue tracker:
  https://github.com/bridgedotnet/Bridge/issues/
