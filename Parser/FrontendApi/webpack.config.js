/// <binding AfterBuild='Run - Development' /> 

module.exports = {
    entry: "./wwwroot/app/index.js",
    output: {
        filename: "./wwwroot/app/index.min.js"
    },
    devServer: {
        contentBase: ".",
        host: "localhost",
        port: 34358
    },
    module: {
        loaders: [
            {
                test: /\.js$/,
                loader: "babel-loader",
                exclude: /(node_modules|bower_components)/,
                query: {
                    presets: ['es2015']
                }
            }
        ]
    }
};