"use strict";

var IS_PROD = process.env.NODE_ENV === "production";

var webpack = require("webpack");

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
    )
}

module.exports = config;