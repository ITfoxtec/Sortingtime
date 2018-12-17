"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cleanCSS = require('gulp-clean-css'),
    less = require("gulp-less"),
    uglify = require("gulp-uglify"),
    chmod = require('gulp-chmod');

var paths = {
    webroot: "./wwwroot/"
};

paths.translateOnceJsDest = paths.webroot + "lib/angular-translate-once/src/translate-once.js";
paths.GroupTaskDropdownJsDest = paths.webroot + "applib/group-task-dropdown-directive.js";
paths.MutexJsDest = paths.webroot + "lib/async-mutex/mutex.js";
paths.navController = paths.webroot + "applib/layout/nav-controller.js";
paths.appJs = paths.webroot + "app/**/*.js";
paths.siteJs = paths.webroot + "js/**/*.js";
paths.appMainJs = paths.webroot + "app/app.js";
paths.appStartMainJs = paths.webroot + "app/app-start.js";
paths.css = paths.webroot + "ui/css/**/*.css";
paths.minCss = paths.webroot + "ui/css/**/*.min.css";
paths.concatTranslateOnceJsDest = paths.webroot + "lib/angular-translate-once/src/translate-once.min.js";
paths.concatMutexJsDest = paths.webroot + "lib/async-mutex/mutex.min.js";
paths.concatAppJsDest = paths.webroot + "app/app.min.js";
paths.concatAppStartJsDest = paths.webroot + "app/app-start.min.js";
paths.concatCssDest = paths.webroot + "ui/css/main.min.css";

gulp.task("clean:angular.translate.once.js", function (cb) {
    rimraf(paths.concatTranslateOnceJsDest, cb);
});

gulp.task("clean:mutex.js", function (cb) {
    rimraf(paths.concatMutexJsDest, cb);
});

gulp.task("clean:app.js", function (cb) {
    rimraf(paths.concatAppJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:angular.translate.once.js", "clean:mutex.js", "clean:app.js", "clean:css"]);

gulp.task("min:angular.translate.once.js", function () {
    return gulp.src([paths.translateOnceJsDest], { base: "." })
        .pipe(concat(paths.concatTranslateOnceJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:mutex.js", function () {
    return gulp.src([paths.MutexJsDest], { base: "." })
        .pipe(concat(paths.concatMutexJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:app.js", function () {
    return gulp.src([paths.appJs, "!" + paths.appMainJs, "!" + paths.appStartMainJs, paths.webroot + "applib/**/*.js", "!" + paths.GroupTaskDropdownJsDest], { base: "." })
        .pipe(concat(paths.concatAppJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:app.start.js", function () {
    return gulp.src([paths.navController], { base: "." })
        .pipe(concat(paths.concatAppStartJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss], { base: "." })
        .pipe(concat(paths.concatCssDest))
        .pipe(cleanCSS({ compatibility: 'ie8' }))
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:angular.translate.once.js", "min:mutex.js", "min:app.js", "min:app.start.js", "min:css"]);

gulp.task("less", function () {
    return gulp.src('Styles/main.less')
        .pipe(less())
        // handel that the output file is not read-only
        .pipe(chmod(755))
        .pipe(gulp.dest(paths.webroot + '/ui/css'));
});