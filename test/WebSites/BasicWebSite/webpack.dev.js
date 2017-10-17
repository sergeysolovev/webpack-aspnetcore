const path = require('path');
const webpack = require('webpack');
const ManifestPlugin = require('webpack-manifest-plugin');

const publicPath = process.env.PUBLIC_PATH || '/';

const config = {
  bail: true,
  devtool: 'cheap-module-source-map',
  entry: {
    index: path.resolve(__dirname, 'app/index')
  },
  output: {
    filename: '[name].6d4f63cc.js',
    publicPath
  },
  devServer: {
    contentBase: false,
    compress: true,
    quiet: false,
    inline: true,
    lazy: false,
    port: 8081,
    https: false,
    host: "0.0.0.0",
  },
  module: {
    rules: []
  },
  plugins: [
    new webpack.DefinePlugin({
      'process.env': {
        'NODE_ENV': JSON.stringify('development'),
        'process.env.PUBLIC_PATH': JSON.stringify(publicPath)
      }
    }),
    new ManifestPlugin({
        fileName: 'webpack-assets.json'
    })
  ]
};

module.exports = config;
