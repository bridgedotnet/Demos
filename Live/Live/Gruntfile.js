module.exports = function (grunt) {

    // Project configuration.
    grunt.initConfig({

        pkg: grunt.file.readJSON('package.json'),

        less: {
            core: {
                options: {
                    compress: true,
                    paths: [
                        "resources/css/less/bootstrap",
                        "resources/css/less/livebridge"
                    ]
                },
                files: {
                    "resources/css/main.min.css": "resources/css/less/main.less"
                }

            }
        },

        watch: {
            less: {
                files: ['resources/css/**/*.less'],
                tasks: ['less']
            },
        }

    });

    // Load plugins
    grunt.loadNpmTasks('grunt-contrib-less');
    grunt.loadNpmTasks('grunt-contrib-watch');

    // Default task(s).
    grunt.registerTask('default', ['less']);

};
