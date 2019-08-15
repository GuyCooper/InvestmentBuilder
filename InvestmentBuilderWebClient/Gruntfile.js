module.exports = function (grunt) {

    grunt.initConfig({

        app: {
            output: {
                folder: "./dist"
            },

            config: "Config.json",

            index: "index.html",

            libs: [
                    "./libs/modernizr-2.6.2.js",
                    "./libs/jquery-1.10.2.js",
                    "./libs/jquery-ui.js",
                    "./libs/bootstrap.js",
                    "./libs/angular.js",
                    "./libs/ag-grid.js",
                    "./libs/ui-bootstrap-tpls-2.5.0.js",
                    "./libs/angular-idle.js",
                    "./Middleware/Middleware.js"
            ],

            libsmin: [
                    "./libs/modernizr-2.6.2.js",
                    "./libsmin/jquery-1.10.2.min.js",
                    "./libsmin/jquery-ui.min.js",
                    "./libsmin/bootstrap.min.js",
                    "./libsmin/angular.min.js",
                    "./libsmin/ag-grid.min.js",
                    "./libsmin/ui-bootstrap-tpls-2.5.0.js",
                    "./libsmin/angular-idle.min.js",
                    "./Middleware/Middleware.js"
            ],

            registerlibsmin: [
                    "./libs/modernizr-2.6.2.js",
                    "./libsmin/jquery-1.10.2.min.js",
                    "./libsmin/jquery-ui.min.js",
                    "./libsmin/bootstrap.min.js",
                    "./libsmin/angular.min.js"
            ],

            registersourcejs : [
                        "./src/RegisterUser.js",
            ],

            sourcejs: [
                        "./src/Utils.js",
                        "./src/NotifyService.js",
                        "./src/Layout.js",
                        "./src/MiddlewareService.js",
                        "./src/CashTransaction.js",
                        "./src/CashFlow.js",
                        "./src/AddInvestment.js",
                        "./src/Portfolio.js",
                        "./src/AddAccount.js",
                        "./src/AccountList.js",
                        "./src/Reports.js",
                        "./src/LastTransaction.js",
                        "./src/Redemptions.js",
                        "./src/RequestRedemption.js",
                        "./src/RegisterModules.js"
                      ],
            jsfiles: "./legacy/index.js",
            viewfiles: [
                           "./Views/AddModalDialog.html",
                           "./Views/AddTransaction.html",
                           "./Views/AddInvestment.html",
                           "./Views/EditTrade.html",
                           "./Views/Parameters.html",
                           "./Views/ReportCompletion.html",
                           "./Views/YesNoChooser.html",
                           "./Views/AddAccount.html",
                           "./Views/LastTransaction.html",
                           "./Views/RequestRedemption.html",
                           "./Views/ShowIdle.html"
                       ],
            styles: {
                source: "./styles/css/**/*.css",
                thirdParty: "./styles/thirdParty/**/*.css",
                images: "./styles/images/*.*",
                fonts: "./styles/fonts/*.*"
            }
        },
        concat: {
            libminjs: {
                src: ['<%= app.libsmin %>'],
                dest: '<%= app.output.folder %>/js/libsmin.js'
            },
            registerlibsjs: {
                src: ['<%= app.registerlibs %>'],
                dest: '<%= app.output.folder %>/js/registerlibs.js'
            },
            registerlibsminjs: {
                src: ['<%= app.registerlibsmin %>'],
                dest: '<%= app.output.folder %>/js/registerlibsmin.js'
            },
            appjs: {
                src: ['<%= app.sourcejs %>'],
                dest: '<%= app.output.folder %>/js/app.js'
            },
            registerappjs: {
                src: ['<%= app.registersourcejs %>'],
                dest: '<%= app.output.folder %>/js/registerUser.js'
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
                        "outputFile": '<%= app.output.folder %>/index.html',
                        "lookup": {}
                    },
                    {
                        "sourceFile": 'userviews/UserTemplate.html',
                        "outputFile": '<%= app.output.folder %>/RegisterUser.html',
                        "lookup": { "templateurl": "userviews/RegisterUser.html", "title" : "Register User"}
                    },
                    {
                        "sourceFile": 'userviews/UserTemplate.html',
                        "outputFile": '<%= app.output.folder %>/ForgottenPassword.html',
                        "lookup": { "templateurl": "userviews/ForgottenPassword.html", "title": "Forgotten Password" }
                    },
                    {
                        "sourceFile": 'userviews/UserTemplate.html',
                        "outputFile": '<%= app.output.folder %>/ChangePassword.html',
                        "lookup": { "templateurl": "userviews/ChangePassword.html", "title": "Change Password" }
                    },
                    {
                        "sourceFile": 'userviews/UserTemplate.html',
                        "outputFile": '<%= app.output.folder %>/ValidateNewUser.html',
                        "lookup": { "templateurl": "userviews/ValidateNewUser.html", "title": "Validate New User" }
                    },
                ]
            }
        },

        uglify: {
            libs: {
                options: {
                    sourceMap: true
                },
                files: {
                    '<%= app.output.folder %>/js/libsmin.js': ['<%= app.libs %>']
                }
            }
        }
    });

    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-typescript');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadTasks('tasks');

    grunt.registerTask('cleandist', ['clean:packaging']);
    grunt.registerTask('buildlib', ['concat:libminjs']);
    //grunt.registerTask('buildlib', ['uglify:libs']);
    grunt.registerTask('buildregisterlib', ['concat:registerlibsminjs']);
    grunt.registerTask('buildapp', ['concat:appjs', 'concat:registerappjs']);
    grunt.registerTask('copyviews', ['copy:views']);
    grunt.registerTask('copyconfig', ['copy:config']);
    grunt.registerTask('buildviews', ['scaffold']);

    grunt.registerTask('buildstyles', ['concat:css', 'copy:css', 'copy:images', 'copy:fonts']);

    grunt.registerTask('default', ['cleandist', 'buildlib', 'buildregisterlib', 'buildapp', 'buildviews', 'copyviews', 'copyconfig', 'buildstyles']);
};