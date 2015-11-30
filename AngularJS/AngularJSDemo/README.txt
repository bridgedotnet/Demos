AngularJS Demo using Bridge.NET

To run this example, follow this walkthrought:

1. Get Bridge.NET sources and demo in a same directory
mkdir bridge
cd bridge
git clone --branch AngularJS --depth 1 https://www.github.com/bridgedotnet/Bridge
git clone --depth 1 https://www.github.com/bridgedotnet/Builder
git clone --branch AngularJS --depth 1 https://www.github.com/bridgedotnet/Frameworks

2. Clone also the Demos repository to the same directory Bridge repos were
   cloned into
git clone --detph 1 https://www.github.com/bridgedotnet/Demos

3. Pull the AngularJS sample submodule on the Demos repository
cd Demos
git submodule update --init --remote --rebase --depth 1
cd ..

4. Build Bridge, opening with VS 2013/2015 either:
Bridge/Bridge.Frameworks.Builder.sln
Bridge/Bridge.Frameworks.Builder.Testing.sln

4.1. You need to rebuild the solution if you skipped step 1 for example,
     if you already had the project cloned and built on your computer
     See Appendix 2 if you get errors trying to rebuild.

5. Open Demos/AngularJS/AngularJSDemo/AngularJSDemo.sln

6. Build the project (F6 key).
6.1. After it finishes, you can refresh the 'WebInterface' project so it shows
     the generated JavaScript files that, for convenience are auto deployed
     there.
6.1.1. To refresh the view, click the 'Refresh' button on the Solution Explorer
       toolbar when you have the 'WebInterface' project selected.

7. Set the startup project to 'WebInterface'
7.1. right-click on the project in solution explorer,
7.2. click 'Set startup project')

8. Run the project (without debugging! -- hotkey ctrl+F5) Your browser will open
   in the test AngularJS page.

Appendix 1: pure AngularJS page.
 If you want to see a similar result with pure AngularJS code (no Bridge built
code) run the project (ctrl+F5) while looking at the contents of the
'index-angular.html' file in the WebInterface project.

Appendix 2: rebuilding bridge to refresh the project -- DLLs already in use
 Because of a known limitation of the UsingTask mechanism used to build Bridge,
the built DLLs become stuck once the client project (this) is built once.
 As the project points directly to Bridge's DLLs inside the main project, you
will be unable to rebuild bridge (if changed something) until you completely
close the Visual Studio instance where the AngularJSDemo has been opened.
 So, whenever you want to rebuild Bridge, ensure the Visual Studio window that
opened AngularJSDemo project is closed.

DISCLAIMER: The contents of the test files are just random strings and have
            absolutely no useful value other than ensuring their content is
            reflected on the resulting web page, resembling the containing
            variables they were originated from.
