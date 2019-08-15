'use strict'

/*
task replaces file placeholders with file contents

placeholder : @[value]

where value can be one of the following:

filename: In this case the file contents will be inserted into the placeholder
parameter: This  works in confunction with the lookup option. If the lookup object contains a field
           with this parameter it will substitute it with the fields value. 

*/

module.exports = function (grunt) {
    console.log('`tasks/scaffold.js` is loaded!');
    grunt.registerTask('scaffold', function () {

        var fs = require('fs');

        var decodeLinkName = function (linkName, lookup) {

            var result = lookup[linkName];
            return result ? result : linkName;
        }

        var parseFile = function (filename, lookup) {
            
            if (fs.existsSync(filename) === false) {
                return filename;
            }

            let outstr = "";
            let linkMode = false;
            let linkFile = "";
           let instr = fs.readFileSync(filename, 'utf8');
            for (let i = 0; i < instr.length; i++) {
                if (instr[i] === '@') {
                    if (instr[i + 1] === '[') {
                        i++;
                        linkMode = true;
                        continue;
                    }
                }
                else if (instr[i] === ']') {
                    if (linkMode === true) {
                        linkMode = false;
                        if(linkFile.length > 0) {
                            outstr += parseFile(decodeLinkName(linkFile, lookup), lookup);
                            linkFile = "";
                        }
                        continue;
                    }
                }
                //if we get here we are just at a normal character - add it to outstr or linkfile
                if (linkMode === true) {
                    linkFile += instr[i];
                }
                else {
                    outstr += instr[i];
                }
            }
            return outstr;
        }

        grunt.verbose.writeln('scaffold called  ');

        //var options = this.options({
        //    sourceFile: '',
        //    outputFile: '',
        //});

        var options = this.options({
            tasks: [
                {
                    sourceFile: '',
                    outputFile: '',
                    lookup: {}
                }
            ]
        });

        console.log('scaffold task count: ' + options.tasks.length);

        for (var i = 0; i < options.tasks.length; i++) {

            console.log('scaffold lookup params: ' + options.tasks[i].lookup);

            var expandedFile = parseFile(options.tasks[i].sourceFile, options.tasks[i].lookup);

            grunt.verbose.writeln('writing file ' + options.tasks[i].outputFile);
            fs.writeFileSync(options.tasks[i].outputFile, expandedFile, {
                encoding: 'utf8',
                flag: 'w+'
            });
        }
    });
};

