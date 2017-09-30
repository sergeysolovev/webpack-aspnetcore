# webapp

A sample ASP.NET Core 2.0 app, showing some usage scenarios of [webpack-aspnetcore](https://github.com/sergeysolovev/webpack-aspnetcore)

To represent a real-world example, this app

* Serves a mix of static webpack and non-webpack assets
* Serves an asynchronous webpack [chunk](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/app/components/carousel.js), which loads using dynamic import
* Provides a [web.config](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/web.config), showing how serve static assets on production with IIS as a reverse proxy
* Has separate webpack configurations for [development](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/webpack.dev.js) and [production](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/webpack.prod.js)

The app is based on a default template, that comes with Visual Studio 2017.

## Prerequisites

Install [.NET Core 2.0 SDK](https://www.microsoft.com/net/download/core) to build the app. Install local npm packages to work with webpack assets

```shell
npm install
```

## Working with webpack assets

To build webpack assets

```shell
npm run build

# with non-empty public path (unix)
export PUBLIC_PATH=/public/path/ && npm run build

# with non-empty public path (windows)
set "PUBLIC_PATH=/public/path/" & npm run build
```

To start the dev server

```shell
npm run start

# with non-empty public path (unix)
export PUBLIC_PATH=/public/path/ && npm run start

# with non-empty public path (windows)
set "PUBLIC_PATH=/public/path/" & npm run start
```

## Launch profiles

The app comes with a few startup configurations and [launch profiles](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Properties/launchSettings.json)

| Profile | Startup class | Environment | Remarks |
|----------------------------|------------------------------------------------------------------------------------------------------------------------------------------------|---------------------|----------------------|
| default | [Startup](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Startup.cs) | Development |  |
| default-prod | [Startup](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Startup.cs) | Production |  |
| default-iisexpr | [Startup](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Startup.cs) | Production | IIS Express only |
| withPublicPath | [StartupWithPublicPath](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithPublicPath.cs) | Development | set PUBLIC_PATH=/public/ |
| withPublicPath-prod | [StartupWithPublicPath](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithPublicPath.cs) | Production | set PUBLIC_PATH=/public/assets/ |
| withStaticFileOpts-prod | [StartupWithStaticFileOptions](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithStaticFileOptions.cs) | Production |  |
| withStaticFileOpts-iisexpr | [StartupWithStaticFileOptions](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithStaticFileOptions.cs) | Production | IIS Express only |

Each profile can be launched using Visual Studio or dotnet CLI. For example, to launch "withPathBase" profile from command line

```shell
export PUBLIC_PATH=/public/ && npm run start
dotnet run --launch-profile withPublicPath
```

This will launch the sample app using [StartupWithPublicPath](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithPublicPath.cs) class on development environment.

Please notice, that all profiles for development environment require launching the dev server with a correct public path.

## Contributing & License

This project is a part of [webpack-aspnetcore](https://github.com/sergeysolovev/webpack-aspnetcore) - see it's [LICENSE](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/LICENSE) file and [Contributing](https://github.com/sergeysolovev/webpack-aspnetcore#contributing) section for details
