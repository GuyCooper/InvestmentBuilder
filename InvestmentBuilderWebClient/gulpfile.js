var gulp = require("gulp");
var browserify = require("browserify");
var source = require('vinyl-source-stream');
var watchify = require("watchify");
var tsify = require("tsify");
var gutil = require("gulp-util");
var uglify = require('gulp-uglify');
var sourcemaps = require('gulp-sourcemaps');
var buffer = require('vinyl-buffer');
var browserSync = require('browser-sync').create();
var concat = require('gulp-concat');
var clean = require('gulp-clean');

var paths = {
    outpath: "dist",
    outfiles: "dist/**/*",
    pages: ['views/*.html'],
    libpath: 'lib',
    libs: ['libs/jquery-1.10.2.js',
            'libs/angular.js',
            'libs/bootstrap.js',
            'libs/jquery-ui.js',
            'libs/jquery.unobtrusive-ajax.js',
            'libs/jquery.validate-vsdoc.js',
            'libs/jquery.validate.js',
            'libs/jquery.validate.unobtrusive.js',
            'libs/respond.js',
            'libs/ui-bootstrap-tpls-2.5.0.js',
            'libs/ag-grid.js'
    ],
    libsmin: ['libs/jquery-1.10.2.min.js',
            'libs/angular.min.js',
            'libs/bootstrap.min.js',
            'libs/jquery-ui.min.js',
            'libs/jquery.unobtrusive-ajax.min.js',
            'libs/jquery.validate-vsdoc.js',
            'libs/jquery.validate.min.js',
            'libs/jquery.validate.unobtrusive.minjs',
            'libs/respond.min.js',
            'libs/ui-bootstrap-tpls-2.5.0.js',
            'libs/ag-grid.min.js'
    ],
    modernizer: 'libs/modernizr-2.6.2.js',
    styles: ['styles/css/**/*', 'styles/thirdParty/**/*'],
    images: ['styles/images/*.png'],
    fonts: ['styles/fonts/*.eot', 'styles/fonts/*.svg', 'styles/fonts/*.ttf', 'styles/fonts/*.woff']
};

gulp.task('clean', function (done) {
    gulp.src(paths.outpath, { read: false })
        .pipe(clean());
    done(); 
});

var watchedBrowserify = watchify(browserify({
    basedir: '.',
    debug: true,
    entries: ['src/main.ts'],
    cache: {},
    packageCache: {}
}).plugin(tsify));

gulp.task("copy-html", function () {
    return gulp.src(paths.pages)
        .pipe(gulp.dest(paths.outpath));
});

gulp.task('copy-modernizer', function () {
    return gulp.src(paths.modernizer)
        .pipe(concat('modernizer.js'))
        .pipe(gulp.dest(paths.outpath + "/js"));
});

gulp.task("copy-libsmin", function () {
    return gulp.src(paths.libsmin)
        .pipe(concat('libsmin.js'))
        .pipe(gulp.dest(paths.outpath + "/js"));
});

gulp.task("copy-styles", function () {
    return gulp.src(paths.styles)
        .pipe(concat('styles.css'))
        .pipe(gulp.dest(paths.outpath + "/styles/css"));
});

gulp.task("copy-images", function () {
    return gulp.src(paths.images)
        .pipe(gulp.dest(paths.outpath + "/styles/images"));
});

gulp.task("copy-fonts", function () {
    return gulp.src(paths.fonts)
        .pipe(gulp.dest(paths.outpath + "/styles/fonts"));
});


gulp.task("build",  function () {
    return browserify({
        basedir: '.',
        debug: true,
        entries: ['legacy/index.js'],
        cache: {},
        packageCache: {}
    })
    .plugin(tsify)
    .bundle()
    .pipe(source('app.js'))
    .pipe(gulp.dest(paths.outpath));
});

gulp.task('browser-sync', function () {
    browserSync.init({
        server: {
            baseDir: paths.outpath
        }
    });
});

function bundle() {
    return watchedBrowserify
        .bundle()
        .pipe(source('app.js'))
        .pipe(buffer())
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(uglify())
        .pipe(sourcemaps.write('./'))
        .pipe(gulp.dest(paths.outpath));
}

gulp.task("watch", bundle);
gulp.task("copy-files", ["copy-libsmin", "copy-styles", "copy-images", "copy-fonts", "copy-html"])


gulp.task("default", ["watch"], function () {
    browserSync.init({
        server: {
            baseDir: paths.outpath
        }
    });
});

watchedBrowserify.on("update", bundle);
watchedBrowserify.on("log", gutil.log);