Missing the contents in the AngularJSDemo/ directory?

If so, just run:
git submodule update --init --remote --rebase

so all submodules are updated and pulled off their respective repos.

To get just AngularJSDemo, it would be:
git submodule update --init --remote --rebase AngularJS/AngularJSDemo
(assuming the command is entered from the repository's root)
