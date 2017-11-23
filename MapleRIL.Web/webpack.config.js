"use strict";

var IS_PROD = process.env.NODE_ENV === "production";

var fs = require("fs");
var path = require("path");
var webpack = require("webpack");

var WebpackOnBuildPlugin = require("on-build-webpack");

var config = {
    entry: "./js/main.js",
    output: {
        filename: "./Static/js/build.js"
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                loader: "babel-loader",
                exclude: /node_modules/
            },
            {
                test: /\.vue$/,
                loader: "vue-loader"
            }
        ]
    },
    resolve: {
        alias: {
            vue: IS_PROD ? 'vue/dist/vue.min' : 'vue/dist/vue.js'
        }
    },
    plugins: []
};

if (IS_PROD) {
    config.plugins.push(
        new webpack.DefinePlugin({
            "process.env": {
                NODE_ENV: '"production"'
            }
        }),
        new webpack.optimize.UglifyJsPlugin()
    );
} else {
    // after the build copy to bin
    var jsPath = path.join("Static", "js", "build.js");
    config.plugins.push(
        new WebpackOnBuildPlugin(function (stats) {
            if (!fs.existsSync(path.resolve("./", "bin", "Debug")))
                return;

            fs
                .createReadStream(path.resolve("./", jsPath))
                .pipe(
                    fs.createWriteStream(path.resolve("./", "bin", "Debug", jsPath))
                );

            console.log("(Also copied build to bin.)");
        })
    );
}

module.exports = config;