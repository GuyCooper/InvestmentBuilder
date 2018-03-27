'use strict';

module.exports = function(grunt) {
    console.log('`tasks/dhviewbuilder.js` is loaded!');    
    grunt.registerTask('dhviewbuilder', function() {

        var fs = require('fs');

        grunt.verbose.writeln('dhviewbuilder called  ');

        var options = this.options({
            outPath:'',
            configfile: './viewTypes.json',
            templateFile: './dhSingleViewTemplate.html'
        });

        var configstr = fs.readFileSync(options.configfile);
        var configViews = JSON.parse(configstr);

        grunt.verbose.writeln('number of views found = ' + configViews.length);        

        var re = /{placeholder}/gi;
        configViews.forEach( function(element) {
            
            var outputfilestr = fs.readFileSync(options.templateFile, 'utf8');

            var modifiedoutputstr = outputfilestr.replace(re, element.description);
            
            var outfilename = options.outPath + 'dh_' + element.description.replace(' ', '_') + '.html';

            grunt.verbose.writeln('writing file ' + outfilename);
            fs.writeFileSync(outfilename, modifiedoutputstr, {encoding:'utf8', 
                                                             flag:'w+'}) ;
        });
    });
};

module.exports = function(grunt) {
    console.log('`tasks/versionupdater.js` is loaded!');    
    grunt.registerTask('versionupdater', function() {

    var fs = require('fs');

    grunt.verbose.writeln('versionupdater called  ');

    var options = this.options();

    var configstr = fs.readFileSync(options.configfile);

    var configobj = JSON.parse(configstr);

    configobj.version = options.version;

    configstr = JSON.stringify(configobj,null,1)

    fs.writeFileSync(options.configfile, configstr, {encoding:'utf8', 
                                                            flag:'w+'}) ;
    });
};

