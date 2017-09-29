# webpack-aspnetcore

<span>ASP.NET</span> Core library for building webpack assets' paths at run time, based on the manifest and HTTP request context.
* Provides a single API for injecting webpack static and dev server assets' paths into razor views
* Automatically updates webpack static assets' paths when the manifest file changes
* Supports serving webpack dev server assets through a [reverse proxy middleware](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/src/Webpack.AspNetCore/DevServer/DevServerReverseProxyMiddleware.cs)
* Supports serving webpack static assets through [StaticFiles](https://github.com/aspnet/StaticFiles) middleware or Kestrel's reverse proxies (Nginx, Apache, IIS)
* Uses a single API for static and dev server webpack assets

## Prerequisites

* [.NET Core 2.0 SDK](https://www.microsoft.com/net/download/core)
* [Webpack](https://webpack.js.org) is installed and configured for YourWebApp to produce an [asset manifest](https://github.com/danethurber/webpack-manifest-plugin)

## Installation

Use NuGet Package Manager for Visual Studio or command line

```shell
dotnet add YourWebApp.csproj package Webpack.AspNetCore
```

## Quick Start

In [Startup.cs](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Startup.cs)

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddWebpack();
    // ...
}

public void Configure(IApplicationBuilder app)
{
    // ...
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseWebpackDevServer();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseWebpackStatic();
    }

    app.UseMvcWithDefaultRoute();
}
```

In [_Layout.cshtml](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Views/Shared/_Layout.cshtml) or other views

```html
@using Webpack.AspNetCore
@inject AssetPathMapper assets

<link rel="stylesheet" href="@await assets("index.css")" />
<img src="@await assets("static/media/banner1.svg")" />
<script src="@await assets("index.js")"></script>
```

This **default configuration** works the following way:

* In [development](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments) webpack assets are being served using the dev server reverse proxy middleware, assuming that the dev server is up and running and the manifest is available at http://127.0.0.1:8080/asset-manifest.json. Auto reloading and hot module replacement on the front-end are working as expected through the dev server's web socket.
* In non-development (production, staging, etc.) [StaticFiles](https://github.com/aspnet/StaticFiles) middleware is used to serve static webpack assets, assuming that the manifest is available at a physical location YouWebApp/wwwroot/asset-manifest.json. If the manifest file changes, assets' paths will be automatically updated on the back-end, thus there's no need to restart Kestrel to see the changes.
* Non-webpack assets for the folder, containing the manifest, are being served using StaticFiles middleware.

For more details and examples, check out how it's done in the [sample app](https://github.com/sergeysolovev/webpack-aspnetcore/tree/master/samples/WebApp).

## Dev server vs. Dev middleware

It does not (on purpose) use any kind of [webpack-dev-middleware](https://github.com/webpack/webpack-dev-middleware), adopted for <span>ASP.NET</span> Core to serve the assets in the [dev mode](#the-static-and-the-dev-server-modes).

It means that the dev server has to be started manually, but only once, so you don't have to wait until all the assets get recompiled **every time your web app restarts**. If it's not an issue, check out [Webpack dev middleware](https://github.com/aspnet/JavaScriptServices/tree/dev/src/Microsoft.AspNetCore.SpaServices#webpack-dev-middleware).

Though it's not a big deal to do `npm run start` to start the dev server, there is [NPM Task Runner](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner) extension for Visual Studio, which lets you do this from the IDE and even bind it to project opening.

## The static and the dev server modes

Webpack.AspNetCore can work in two modes: the static mode and the dev server mode. Since it provides a single API for obtaining the assets' paths using their keys from the manifest, it can only work in one mode at the same time.

To make it work in the static mode, use

```csharp
app.UseWebpackStatic();
```

in your the Configure method of your startup class. For the dev server mode use

```csharp
app.UseWebpackDevServer();
```

As a single statement:

```csharp
app.UseWebpack(withDevServer: env.IsDevelopment());
```

This will make it work in the dev server mode for the development environment and in the static mode for other environments.

## Configuration

Several options are provided to configure how Webpack.AspNetCore handles static and dev server assets.

### Public path

Configured [webpack public path](https://webpack.js.org/configuration/output/#output-publicpath) has to match your web app's webpack assets' public path, which consists of:
* [Request path base](https://docs.microsoft.com/en-us/aspnet/core/api/microsoft.aspnetcore.http.httprequest) which is not empty when you, for example, host your web app as an IIS virtual application
* [Request path](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files) of StaticFile middleware for serving static webpack assets. It's default value is an empty path string.

To set the request path of webpack assets

```csharp
services.AddWebpack(options =>
{
    // static
    options.ConfigureStatic(opts => opts.RequestPath = "/public/");

    // dev server
    options.ConfigureDevServer(opts =>
        opts.ConfigureStatic(staticOpts => staticOpts.RequestPath = "/public/")
    )
});
```

This will make Webpack.AspNetCore prepend static assets' paths with a specified request path.

To set public path the dev server assets are served from

```csharp
services.AddWebpack(options =>
{
    options.ConfigureDevServer(opts => opts.PublicPath = "/public/")
});
```

This will make Webpack.AspNetCore request the dev server assets from the specified public path on the dev server.

Check out startup configuration examples from the sample app:
* [StartupWithPathBase.cs](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithPathBase.cs)
* [StartupWithStaticFileOptions.cs](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithStaticFileOptions.cs)

### Changing the manifest path

The manifest path is defined by a `WebpackOptions.ManifestPath` property, which represents a path, relative to ASP.NET Core [Web Root](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/index?tabs=aspnetcore2x#web-root) and defaults to "/asset-manifest.json". The same property is used to define the manifest path on webpack dev server.

```csharp
services.AddWebpack(options => options.ManifestPath = "/dist/webpack-assets.json");
```

In the above example the manifest path is set to "/dist/webpack-assets.json", so static webpack assets will be served from YourWebApp/wwwroot/dist. In this case the dev server assets and the manifest are expected to be served from http://127.0.0.1:8080/dist/.

### Configuring StaticFile middleware

Serving webpack and non-webpack static assets using StaticFile middleware is [enabled](#disabling-statifiles-middleware) by default.

To configure StaticFile middleware, Webpack.AspNetCore provides a limited set of options. These options include all properties of [StaticFileOptions](https://github.com/aspnet/StaticFiles/blob/dev/src/Microsoft.AspNetCore.StaticFiles/StaticFileOptions.cs), except FileProvider, since a file provider for static webpack assets is defined by the [manifest path base](#changing-the-manifest-path) and can not be changed.

Static assets' options are configured separately for the dev server and static webpack assets.

```csharp
services.AddWebpack(options =>
{
    options.ConfigureStatic(opts => {
        opts.RequestPath = "/public/";
        opts.OnPrepareResponse = responseContext =>
            responseContext.Context.Response.Headers.Add(
                key: "Cache-control",
                value: "public,max-age=31536000"
            );
    });

    options.ConfigureDevServer(opts =>
        opts.PublicPath = "/public/"
    );
});
```

In this example the request path and OnPrepareResponse delegate are changed for the static mode, while the same properties have their default values for the dev server mode. To configure static assets' options for the dev server mode

```csharp
services.AddWebpack(options =>
{
    options.ConfigureDevServer(opts =>
        opts.ConfigureStatic(staticOpts => { /* ... */ })
    );
});
```

### Disabling StaticFiles middleware

If for some reason you don't need StaticFiles middleware, it can be disabled

```csharp
services.AddWebpack(options => options.UseStaticFileMiddleware = false);
```

This way StaticFiles middleware won't be used neither in the static nor in the dev server mode.

### Serving static assets with a reverse proxy on production

Although by default Webpack.AspNetCore uses StaticFiles middleware to serve static assets, this is probably not how you want it to be done on production. Nginx, Apache and IIS as reverse proxies in front of Kestrel do it more efficiently by using their dedicated modules for serving static content.

When using a reverse proxy for serving static files, **no changes are needed for Webpack.AspNetCore configuration**, since appropriate requests simply won't get to Kestrel. Though you might want to use some kind of url rewriting from the reverse proxy.

For example, check out how it's done in the sample app [for IIS](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/web.config).

```xml
<rewrite>
    <rules>
    <rule name="wwwroot">
        <match url="([\S]+[.](html|htm|svg|js|css|png|gif|jpg|jpeg|ico|woff[2]?|ttf))" />
        <action type="Rewrite" url="wwwroot/{R:1}" />
    </rule>
    </rules>
</rewrite>
```

These rewrite rules work fine with the [default configuration](#quick-start) shown above.

### Specifying the dev server's port, host and scheme

```csharp
services.AddWebpack(options =>
{
    options.ConfigureDevServer(opts =>
    {
        opts.Host = "localhost";
        opts.Port = 8081;
        opts.Scheme = "https";
    })
});
```

## Examples from the sample app

The [sample web app](https://github.com/sergeysolovev/webpack-aspnetcore/tree/master/samples/WebApp) comes with a few startup configurations and [launch profiles](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Properties/launchSettings.json):

| Profile | Startup | Default environment | Method |
|----------------------------|------------------------------------------------------------------------------------------------------------------------------------------------|---------------------|----------------------|
| default | [Startup](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Startup.cs) | Development | Kestrel, IIS Express |
| withPathBase | [StartupWithPathBase](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithPathBase.cs) | Development | Kestrel, IIS Express |
| withStaticFileOpts | [StartupWithStaticFileOptions](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithStaticFileOptions.cs) | Development | Kestrel, IIS Express |
| default-iisExpr | [Startup](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Startup.cs) | Production | IIS Express |
| withPathBase-iisExpr | [StartupWithPathBase](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithPathBase.cs) | Production | IIS Express |
| withStaticFileOpts-iisExpr | [StartupWithStaticFileOptions](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithStaticFileOptions.cs) | Production | IIS Express |

Each profile can be launched using Visual Studio or dotnet CLI. For example, to launch "withPathBase" profile from command line

```shell
dotnet run --launch-profile withPathBase
```

This will launch the sample app using StartupWithPathBase class on development environment. To do it on production environment

```shell
# unix
export ASPNETCORE_ENVIRONMENT=Production && dotnet run --launch-profile withPathBase

# windows
set ASPNETCORE_ENVIRONMENT=Production & dotnet run --launch-profile withPathBase
```

## Contributing

Pull requests are highly welcomed! Please, open up a discussion [here](https://github.com/sergeysolovev/webpack-aspnetcore/issues) first.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
