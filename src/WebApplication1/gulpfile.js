/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    gulpFilter = require('gulp-filter');

var paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";
paths.areas = "Areas";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("clean:areas", function (cb) {
    rimraf(paths.areas, cb);
});

gulp.task('copy:P7.Main', function () {
    return gulp.src(['../P7.Main/Views/**'])
        .pipe(gulp.dest('Views/'));
});

gulp.task('copy:P7.Main:areas', function () {
    return gulp.src(['../P7.Main/Areas/**', '!../P7.Main/Areas/*/{Controllers,Controllers/**}'])
        .pipe(gulp.dest('Areas/'));
});

gulp.task('copy:brunch:wwwroot', function () {
    return gulp.src(['../brunch/wwwroot/brunched/**'])
        .pipe(gulp.dest('wwwroot/'));
});

gulp.task('copy:P7.Main:static', function () {
    return gulp.src(['../P7.Main/static/**'])
        .pipe(gulp.dest('wwwroot/static/'));
});



gulp.task('watch', [
        'copy:brunch:wwwroot',
        'copy:P7.Main',
        'copy:P7.Main:areas'    
    ],
    function () {
        gulp.watch(['../brunch/wwwroot/brunched/**'], ['copy:brunch:wwwroot']);
        gulp.watch(['../P7.Main/Views/**'], ['copy:P7.Main']);
        gulp.watch(['../P7.Main/Areas/**'], ['copy:P7.Main:areas']);
  
    });

gulp.task("min", ["min:js", "min:css"]);
