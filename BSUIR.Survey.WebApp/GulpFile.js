/// <binding BeforeBuild='scripts' />
"use strict";

var gulp = require("gulp")

var deps = {
    "jquery": {
        "**/*": ""
    },
    "bootstrap": {
        "**/**/*": "",
    }

};

gulp.task("scripts", function () {
    var streams = [];
    for (var dep in deps)
        if (Object.prototype.hasOwnProperty.call(deps, dep)) {
            var thisArg = deps[dep];
            for (var item in thisArg)
                if (Object.prototype.hasOwnProperty.call(thisArg, item))
                    streams.push(gulp.src("node_modules/" + dep + "/" + item)
                        .pipe(gulp.dest("wwwroot/lib/" + dep + "/" + thisArg[item])));
        }

})