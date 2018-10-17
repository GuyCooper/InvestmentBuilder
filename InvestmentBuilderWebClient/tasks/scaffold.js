'use strict'

//task replaces file placeholders with file contents
module.exports = function (grunt) {
    console.log('`tasks/scaffold.js` is loaded!');
    grunt.registerTask('scaffold', function () {

        var fs = require('fs');

        var parseFile = function (filename) {
            
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
                            outstr += parseFile(linkFile);
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
                }
            ]
        });

        console.log('scaffold task count: ' + options.tasks.length);

        for (var i = 0; i < options.tasks.length; i++) {

            var expandedFile = parseFile(options.tasks[i].sourceFile);

            grunt.verbose.writeln('writing file ' + options.tasks[i].outputFile);
            fs.writeFileSync(options.tasks[i].outputFile, expandedFile, {
                encoding: 'utf8',
                flag: 'w+'
            });
        }
    });
};

