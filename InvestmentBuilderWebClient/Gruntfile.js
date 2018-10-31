module.exports = function (grunt) {

    grunt.initConfig({

        app: {
            output: {
                folder: "./dist"
            },

            config: "Config.json",

            index: "index.html",
            topviewfiles: [
                "register.html",
                "password.html"
            ],

            libs: [
                    "./libs/modernizr-2.6.2.js",
                    "./libs/jquery-1.10.2.js",
                    "./libs/jquery-ui.js",
                    "./libs/bootstrap.js",
                    "./libs/angular.js",
                    "./libs/ag-grid.js",
                    "./libs/ui-bootstrap-tpls-2.5.0.js",
                    "./Middleware/Middleware.js"
            ],
            registerlibs: [
                    "./libs/modernizr-2.6.2.js",
                    "./libs/jquery-1.10.2.js",
                    "./libs/jquery-ui.js",
                    "./libs/bootstrap.js",
                    "./libs/angular.js"
            ],

            registersourcejs : [
                        "./src/Register.js",
            ],

            sourcejs: [
                        "./src/Utils.js",
                        "./src/NotifyService.js",
                        "./src/Layout.js",
                        "./src/MiddlewareService.js",
                        "./src/CashFlow.js",
                        "./src/CreateTrade.js",
                        "./src/Portfolio.js",
                        "./src/AddAccount.js",
                        "./src/AccountList.js",
                        "./src/Reports.js",
                        "./src/RegisterModules.js"
                      ],
            jsfiles: "./legacy/index.js",
            viewfiles: [
                           "./Views/AddModalDialog.html",
                           "./Views/AddTransaction.html",
                           "./Views/CreateTrade.html",
                           "./Views/EditTrade.html",
                           "./Views/Parameters.html",
                           "./Views/RaiseAlert.html",
                           "./Views/ReportCompletion.html",
                           "./Views/YesNoChooser.html",
                           "./Views/AddAccount.html"
                       ],
            styles: {
                source: "./styles/css/**/*.css",
                thirdParty: "./styles/thirdParty/**/*.css",
                images: "./styles/images/*.*",
                fonts: "./styles/fonts/*.*"
            }
        },
        concat: {
            libjs: {
                src: ['<%= app.libs %>'],
                dest: '<%= app.output.folder %>/js/libs.js'
            },
            registerlibsjs: {
                src: ['<%= app.registerlibs %>'],
                dest: '<%= app.output.folder %>/js/registerlibs.js'
            },
            appjs: {
                src: ['<%= app.sourcejs %>'],
                dest: '<%= app.output.folder %>/js/app.js'
            },
            registerappjs: {
                src: ['<%= app.registersourcejs %>'],
                dest: '<%= app.output.folder %>/js/register.js'
            },
            
            css: {
                src: ['<%= app.styles.source %>'],
                dest: '<%= app.output.folder %>/styles/investmentBuilder.css'
            }
        },

        copy: {
            views: {
                files: [
                    // flattens results to a single level
                    { expand: true, flatten: true, src: '<%= app.viewfiles %>', dest: '<%= app.output.folder%>/views', filter: 'isFile' }
                ]
            },
            topViews: {
                files: [
                    // flattens results to a single level
                    { expand: true, flatten: true, src: '<%= app.topviewfiles %>', dest: '<%= app.output.folder%>', filter: 'isFile' }
                ]
            },

            config: {
                files: [
                    // flattens results to a single level
                    { expand: true, flatten: true, src: '<%= app.config %>', dest: '<%= app.output.folder%>', filter: 'isFile' }
                ]
            },

            css: {
                files: [
                    // flattens results to a single level
                    { expand: true, flatten: true, src: ['<%= app.styles.thirdParty %>'], dest: '<%= app.output.folder%>/styles', filter: 'isFile' }
                ]
            },
            images: {
                files: [
                    // flattens results to a single level
                    { expand: true, flatten: true, src: ['<%= app.styles.images %>'], dest: '<%= app.output.folder%>/styles/images', filter: 'isFile' }
                ]
            },
            fonts: {
                files: [
                    // flattens results to a single level
                    { expand: true, flatten: true, src: ['<%= app.styles.fonts %>'], dest: '<%= app.output.folder%>/styles/fonts', filter: 'isFile' }
                ]
            }
        },

        clean: {
            options: { force: false },
            packaging: ['<%= app.output.folder %>']
        },

        typescript: {
            app: {
                include: ['<%= app.jsfiles %>'],
                dest: '<%= app.output.folder %>',
                options: {
                    target: 'es5',
                    allowJs: true,
                    outDir: '<%= app.output.folder %>'
                }
            },
        },

        scaffold: {
            options: {
                tasks: [
                    {
                        "sourceFile": '<%= app.index %>',
                        "outputFile": '<%= app.output.folder %>/index.html'
                    },
                ]
            }
        }
    });

    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-typescript');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadTasks('tasks');

    grunt.registerTask('cleandist', ['clean:packaging']);
    grunt.registerTask('buildlib', ['concat:libjs']);
    grunt.registerTask('buildregisterlib', ['concat:registerlibsjs']);
    grunt.registerTask('buildapp', ['concat:appjs', 'concat:registerappjs']);
    grunt.registerTask('copyviews', ['copy:views', 'copy:topViews']);
    grunt.registerTask('copyconfig', ['copy:config']);
    grunt.registerTask('buildviews', ['scaffold']);

    grunt.registerTask('buildstyles', ['concat:css', 'copy:css', 'copy:images', 'copy:fonts']);

    grunt.registerTask('default', ['cleandist', 'buildlib', 'buildregisterlib', 'buildapp', 'buildviews', 'copyviews', 'copyconfig', 'buildstyles']);
};