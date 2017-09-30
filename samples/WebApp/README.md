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

## Builing and running the app

The app comes with a few startup configurations and [launch profiles](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Properties/launchSettings.json)

| startup class | env | npm scripts | path base | remarks |
|---------------|-----|----------|----------|-------------|
| [Startup](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Startup.cs) | dev | start:wds <br> start:dotnet | / |
| [Startup](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Startup.cs) | prod | build:assets <br> start:dotnet:prod | / |
| [Startup](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Startup.cs) | prod | build:assets <br> N/A (see remarks) | / | visual studio <br> iis express |
| [StartupWithPublicPath](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithPublicPath.cs) | dev | start:wds:withPublicPath <br> start:dotnet:withPublicPath | /public |
| [StartupWithPublicPath](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithPublicPath.cs) | prod | build:assets:withPublicPath <br> start:dotnet:withPublicPath:prod | /public |
| [StartupWithStaticFileOptions](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithStaticFileOptions.cs) | prod | build:assets <br> start:dotnet:withStaticFileOpts:prod | / |

Each profile can be launched using Visual Studio or dotnet CLI.
1. Pick up a profile from the table
2. Prepare the assets for serving using `npm run start:wds{:startup}|build:assets{:startup}` (see the first script in the "npm scripts" column)
3. Launch the app using `npm run start:dotnet{:profile}` (see the second script in the "npm scripts" column or use visual studio)
4. Use the path base from the column "path base" to open the app in the browser, if it was launched from the command line.

### Example: production, with public path, command line

```shell
npm run build:assets:withPublicPath && npm run start:dotnet:withPublicPath:prod
```

This will launch the sample app using [StartupWithPublicPath](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithPublicPath.cs) class on production environment. To see it working open http://127.0.0.1:5000/public in the browser.

### Example: development, command line

Run these commands in separate sessions

```shell
npm run start:wds
npm run start:dotnet
```

This will launch the sample app using [Startup](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Startup.cs) class on development environment. To see it working open http://127.0.0.1:5000 in the browser.

### Example: IIS Express

Build the assets for the default configuration

```shell
npm run build:assets
```

and launch `default-iisexpr` profile from Visual Studio

![vs-iisexpr](https://user-images.githubusercontent.com/5831301/31053549-621fcd30-a6d2-11e7-8efe-0b362b983755.png)

## Contributing & License

This project is a part of [webpack-aspnetcore](https://github.com/sergeysolovev/webpack-aspnetcore) - see it's [LICENSE](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/LICENSE) file and [Contributing](https://github.com/sergeysolovev/webpack-aspnetcore#contributing) section for details
