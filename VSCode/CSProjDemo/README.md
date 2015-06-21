How to successfully open this on VSCode

-= Opening the project =-
=========================

On VSCode, you cannot just open the .sln file, sit back and wait for
it to download your NuGet packages.

This applies for Visual Studio solutions. Although limited, project.json
(ASP.Net 5.0) projects have NuGet packages update functionality integrated
on VSCode.

So, before opening this project on Visual Studio Code, you need to manually
restore the NuGet packages. For that, supposing you have Mono installed on
your system, you just need to:

$ nuget restore

Run this command on the same directory where the 'CSProjDemo.sln' file is
and it should fix the NuGet packages for you.

Once this is done DON'T OPEN THE .SLN FILE with VSCode. Instead, open THE
FOLDER where CSProjDemo.sln is. This way, VSCode will scan the folder for
supported project types and finally load the .sln file, enabling intelli-
sense correctly and enabling you to build the project using 'xbuild'.

-= Building the solution =-
===========================

This project comes pre-shipped with the build task necessary to build it
using 'xbuild' on the solution file.

Once loaded on Visual Studio Code, to build the solution run the build
task (shift+command+B on OS X and ctrl+shift+B anywhere else -- or via
the VSCode's command palette).

-= Testing the results =-
=========================

Once built, you can check the result simply by opening
'CSProjDemo/Demo/Bridge/www/demo.html' on your favorite web browser.
