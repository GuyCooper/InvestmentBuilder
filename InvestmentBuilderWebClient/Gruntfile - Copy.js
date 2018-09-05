module.exports = function(grunt) {
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        app: grunt.file.readJSON('config.app.json'),
        framework: grunt.file.readJSON('config.framework.json'),
        lib: grunt.file.readJSON('config.lib.json'),
        modules: grunt.file.readJSON('config.modules.json'),
        uglify: {
            options: {
                banner: '/*! <%= pkg.name %> <%= grunt.template.today("yyyy-mm-dd") %> */\n'
            },
            app: {
                src: '<%= app.output.js %>',
                dest: '<%= app.output.jsmin %>'
            },
            framework: {
                src: '<%= framework.output.js %>',
                dest: '<%= framework.output.jsmin %>'
            }
        },

        typescript: {
            app: {
                src: ['<%= app.ts %>'],
                dest: '<%= app.output.js %>',
                options: {
                    target: 'es5',
                    sourceMap: true,
                    declaration: true
                }
            },
            modules: {
                src: ['<%= modules.ts %>'],
                dest: '<%= modules.output.js %>',
                options: {
                    target: 'es5',
                    sourceMap: true,
                    declaration: true
                }
            },
            framework: {
                src: ['<%= framework.tsfiles %>'],
                dest: '<%= framework.output.js %>',
                options: {
                    target: 'es5',
                    sourceMap: true,
                    declaration: true,
                    comments: true
                }
            },
            apptest: {
                src: ['<%= app.test.tsfiles %>'],
                dest: '<%= app.test.output.js %>',
                options: {
                    target: 'es5',
                    sourceMap: true,
                    declaration: false
                }
            },
            frameworktest: {
                src: ['<%= framework.test.tsfiles %>'],
                dest: '<%= framework.test.output.js %>',
                options: {
                    target: 'es5',
                    sourceMap: true,
                    declaration: false
                }
            },
            modulestest: {
                src: ['<%= modules.test.ts %>'],
                dest: '<%= modules.test.output.js %>',
                options: {
                    target: 'es5',
                    sourceMap: true,
                    declaration: false
                }
            },
            appcontracts: {
                src: ['<%= app.contracttsfiles %>'],
                dest: '<%= app.output.contractjs %>',
                options: {
                    target: 'es5',
                    sourceMap: false,
                    declaration: true
                }
            }
        },

        watch: {
            appjs: {
                files: ['<%= app.tsfiles %>'],
                tasks: ['typescript:app','buildtest','test','jshint:app']
            },
            frameworkjs: {
                files: ['<%= framework.tsfiles %>'],
                tasks: ['buildframework','typescript:app','buildtest','test', 'jshint:framework']
            },
            modulesjs: {
                files: ['<%= modules.tsfiles %>'],
                tasks: ['buildmodules','buildmodulestest','testmodules', 'jshint:modules']
            },
            apptestjs: {
                files: ['<%= app.test.tsfiles %>'],
                tasks: ['buildapptest','testapp']
            },
            frameworktestjs: {
                files: ['<%= framework.test.tsfiles %>'],
                tasks: ['buildframeworktest','testframework']
            },
            modulestestjs: {
                files: ['<%= modules.test.tsfiles %>'],
                tasks: ['buildmodulestest','testmodules']
            },
            appcontractsts:{
                files:['<%= app.contracttsfiles %>'],
                tasks:['buildallandtest']
            },
            appviews: {
                files: ['<%= app.viewfiles %>'],
                tasks: ['buildmodules', 'buildappviews']
            },
            moduleviews: {
                files: ['<%= modules.viewfiles %>'],
                tasks: ['buildmodules', 'buildappviews']
            },
            apphtml: {
                files: ['<%= app.html %>'],
                tasks: ['copy:apphtml']
            },
            styles: {
                files: ['<%= app.scssfiles %>'],
                tasks: ['buildshellcss', 'buildappcss']
            },
            modulestyles: {
                files: ['<%= modules.scssfiles %>'],
                tasks: ['buildmodules', 'buildappcss']
            },
            appimages: {
                files: ['<%= app.imagefiles %>'],
                tasks: ['copy:appimages']
            },
            dhviewhtml: {
                files: ['<%= app.dhviewfiles %>'],
                tasks: ['builddhview']
            },
            dhviewscript: {
                files: ['<%= app.dhviewscriptfiles %>'],
                tasks: ['builddhview']
            }
        },


        jshint: {
           	options: {
           		"jshintrc": "jshint.jshintrc",
           		"force": true
           	},
           	app: {
           		src: ['<%= app.output.js %>'],
           		options: {
           			reporterOutput: "jshint-app.log"
           		}
           	},
           	framework: {
           		src: ['<%= framework.output.js %>'],
           		options: {
           			reporterOutput: "jshint-framework.log"
           		}
           	},
           	modules: {
           		src: ['<%= modules.output.js %>'],
           		options: {
           			reporterOutput: "jshint-modules.log"
           		}
           	}
        },       

        concat: {
            views: {
                options: {
                    separator: ',' + grunt.util.linefeed,
                    banner: '[',
                    footer: ']',
                    process: function (src, filepath) {
                        var name = filepath.replace(/^.*[\\\/]/, '').replace('.html', '');
                        return "{ \"id\": \"" + name + "\", \"data\": \"" + src.replace(/(\r\n|\n|\r)/gm, "").replace(/"/g, '\\"') + "\"}";
                    }                    
                },
                src: ['<%= app.outputdir.views %>*.html'],
                dest: '<%= app.output.template %>'
            },
            dhviews: {
                options: {
                    separator: ',' + grunt.util.linefeed,
                    banner: '[',
                    footer: ']',
                    process: function (src, filepath) {
                        var name = filepath.replace(/^.*[\\\/]/, '').replace('.html', '');
                        return "{ \"id\": \"" + name + "\", \"data\": \"" + src.replace(/\r/g, "\\\\r").replace(/\n/g, "\\\\n").replace(/\t/g, "\\\\t").replace(/"/g, '\\"') + "\"}";
                    }                    
                },
                src: ['<%= app.outputdir.dhviews %>*.html'],
                dest: '<%= app.output.dhviewtemplate %>'
            },

            libjs: {
                src: ['<%= lib.jsfiles %>'],
                dest: '<%= lib.output.js %>'
            },
            libjsmin: {
                src: ['<%= lib.jsminfiles %>'],
                dest: '<%= lib.output.jsmin %>'
            }
        },
        karma: {
            app: {
                configFile: '<%= app.test.config %>'
            },
            framework: {
                configFile: '<%= framework.test.config %>'
            },
            modules: {
                configFile: '<%= modules.test.config %>'
            }
        },
        copy: {
            views: {
                files: [
                    // flattens results to a single level
                    {expand: true, flatten: true, src: ['<%= app.viewfiles %>', '<%= framework.viewfiles %>'], dest: '<%= app.outputdir.views %>', filter: 'isFile'}
                ]
            },
            appimages: {
                files: [
                    {expand: true, flatten: true, src: ['<%= app.imagefiles %>'], dest: '<%= app.outputdir.images %>', filter: 'isFile'}
                ]
            },
            themeimages: {
                files: [
                    {expand: true, flatten: true, src: ['<%= app.scssthemed %><%= grunt.option("theme")%>/images/*.*'], dest: '<%= app.outputdir.images %>', filter: 'isFile'}
                ]
            },            
            appresources: {
                files: [
                    {expand: true, flatten: true, src: ['<%= app.resourcefiles %>'], dest: '<%= app.outputdir.resources %>', filter: 'isFile'}
                ]
            },			
            apphtml: {
                files: [
                    {expand: true, flatten: true, src: ['<%= app.html %>'], dest: '<%= app.outputdir.packaging %>', filter: 'isFile'}
                ]
            },
            settings: {
				files: [
					{expand: true, flatten: true, src: ['<%= app.settings %>'], dest: '<%= app.outputdir.packaging %>', filter: 'isFile'}
				]
			},
            dhview_html: {
                files: [
                    {expand: true, flatten: true, src: ['<%= app.dhview_main_html %>', '<%= app.dhview_view_types %>'], dest: '<%= app.outputdir.packaging %>', filter: 'isFile'}
                ]
            },            
            dhview_templates: {
                files: [
                    {expand: true, flatten: true, src: ['<%= app.dhview_templates %>'], dest: '<%= app.outputdir.dhviews %>', filter: 'isFile'}
                ]
            },            
            dhview_scripts: {
                files: [
                    {expand: true, flatten: true, src: ['<%= app.dhview_scripts %>'], dest: '<%= app.outputdir.scripts %>', filter: 'isFile'}
                ]
            },            
            dhview_style: {
                files: [
                    {expand: true, flatten: true, src: ['<%= app.dhview_style %>'], dest: '<%= app.outputdir.style %>', filter: 'isFile'}
                ]
            },   
            web_config: {
                files: [
                    {expand: true, flatten: true, src: ['<%= app.webconfig %>'], dest: '<%= app.outputdir.packaging %>', filter: 'isFile'}
                ]
            },
            translator: {
				files: [
					{expand: true, flatten: true, src: ['<%= app.translator %>'], dest: '<%= app.outputdir.packaging %>', filter: 'isFile'}
				]
            },            
            fonts: {
                files: [
                    { expand: true, flatten: true, src: ['<%= app.fonts %>'], dest: '<%= app.outputdir.fonts %>', filter: 'isFile' }
                ]
            }            ,
            fontawesome: {
                files: [
                    { expand: true, flatten: true, src: ['<%= app.fontawesome %>'], dest: '<%= app.outputdir.fontawesome %>', filter: 'isFile' }
                ]
            }
        },
        clean: {
            options: { force: true },
            packaging: ["<%= app.outputdir.packaging %>"],
            views: ["<%= app.outputdir.views %>"],
            dhviews: ["<%= app.outputdir.dhviews %>"]
        },
        sass: {
            app: {
                files: {
                    "<%= app.output.css %>": '<%= app.scss %>'
                }
            },
            appthemed: {
                files: {
                    "<%= app.output.css %>": '<%= app.scssthemed %><%= grunt.option("theme")%>/app.scss'
                }
            },
            shell: {
                files: {
                    '<%= app.output.shellcss %>': '<%= app.shellscss %>'
                }
            }
        },
        browserSync: {
            dev: {
                bsFiles: {
                    src : '<%= app.syncfiles %>'
                },
                options: {
                    watchTask: true,
                    server: {
                        baseDir: '<%= app.serverdirs %>',
                        index: "index.html"
                    },
                    ghostMode: {
                        clicks: false,
                        scroll: false,
                        links: false,
                        forms: false
                    }
                }
            }
        },
        modulebuilder:{
            options:{
                "modules":"<%= modules.list %>",
                "modulesInputDir":"<%= modules.dir %>",
                "appOutputDir":"<%= modules.output.dir %>"
            }
        },
        dhviewbuilder: {
            options: {    
                "outPath": '<%= app.outputdir.packaging %>',
                "configfile": '<%= app.dhview_view_types %>',
                "templateFile": '<%= app.dhview_template_html %>'
            }
        },
        versionupdater: {
            options: {
                "configfile": '<%= app.versionupdated %>',
                "version":'<%= grunt.option("ver")%>'
            }
        }
    });

    // Load the plugins
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-typescript');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-jshint');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-karma');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-sass');
    grunt.loadNpmTasks('grunt-browser-sync');
    grunt.loadTasks('tasks');

    // Default task(s).
    grunt.registerTask('buildlib', ['concat:libjs']);
    grunt.registerTask('buildframework', ['typescript:framework']);
    grunt.registerTask('buildappcontracts', ['typescript:appcontracts']);
    grunt.registerTask('buildshellcss', ['sass:shell']);    
    grunt.registerTask('buildappcss', function() { 
		if (!grunt.option('theme')) { 
			grunt.task.run(['sass:app']); 
		} else {
			grunt.task.run(['sass:appthemed']); 
		}
	});
    grunt.registerTask('buildshell', ['buildlib','buildframework','buildshellcss']);
    grunt.registerTask('buildapp', ['buildappcontracts', 'buildmodules', 'buildappcss', 'copy:apphtml', 'copy:settings', 'buildappviews', 'copy:appimages', 'copy:themeimages', 'copy:appresources', 'copy:fonts', 'copy:fontawesome', 'typescript:app', 'copy:translator' ]);
    grunt.registerTask('buildappviews', ['copy:views', 'concat:views', 'clean:views']);
    // grunt.registerTask('builddhview', ['dhviewbuilder','copy:dhview_html', 'copy:dhview_templates', 'copy:dhview_style', 'copy:dhview_scripts', 'copy:web_config', 'concat:dhviews', 'clean:dhviews']);
    grunt.registerTask('buildapptest', ['typescript:apptest' ]);
    grunt.registerTask('buildframeworktest', ['typescript:frameworktest' ]);
    grunt.registerTask('buildmodules',['modulebuilder','typescript:modules']);
    grunt.registerTask('buildmodulestest', ['typescript:modulestest' ]);
    grunt.registerTask('buildtest', ['buildframeworktest','buildapptest', 'buildmodulestest' ]);
    grunt.registerTask('testapp', ['karma:app' ]);
    grunt.registerTask('testframework', ['karma:framework' ]);
    grunt.registerTask('testmodules', ['karma:modules' ]);
    grunt.registerTask('test', ['testframework', 'testapp', 'testmodules' ]);
    grunt.registerTask('buildall', ['clean:packaging', 'buildshell', 'buildapp', 'buildtest']);
    grunt.registerTask('buildallandtest', ['buildall','test' ]);
    grunt.registerTask('buildallandtestandlint', ['buildall','test','jshint' ]);
    grunt.registerTask('builddhviewmodules',['dhviewbuilder']);
	//grunt.registerTask('default', ['buildall' ]);
	//grunt.registerTask('default', ['buildallandtest' ]);
	//grunt.registerTask('default', ['buildallandtestandlint']);
    grunt.registerTask('updateversion', ['versionupdater']);
    grunt.registerTask('default', ['buildallandtestandlint', 'browserSync', 'watch']);
    //grunt.registerTask('default', ['browserSync', 'watch']);
    grunt.registerTask('run', ['browserSync', 'watch']);    
};