# webpack-aspnetcore &middot; [![AppVeyor Status](https://ci.appveyor.com/api/projects/status/github/gruntjs/grunt?branch=master&svg=true)](https://ci.appveyor.com/project/sergeysolovev/webpack-aspnetcore) [![Travis](https://img.shields.io/travis/sergeysolovev/webpack-aspnetcore.svg)](https://travis-ci.org/sergeysolovev/webpack-aspnetcore) [![NuGet](https://img.shields.io/nuget/vpre/Webpack.AspNetCore.svg)](https://www.nuget.org/packages/Webpack.AspNetCore/2.0.0-beta4) [![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](#contributing)

<span>ASP.NET</span> Core 2.0 extension for using webpack assets in your views

* **Manifest-based:** Injects assets' paths with correct hashes right into your
  views. This way you can implement
  [long-term caching](https://webpack.js.org/guides/caching/) without pain.
* **Zero configuration dev server support:** It's very likely that you want to
  use webpack-dev-server for development. With webpack-aspnetcore you can do
  that with zero changes to your views and configuration, since it provides a
  single API to work with static and dev server assets.
* **Auto reloading:** When you change your manifest file or it's folder,
  it automatically reloads assets' paths, so you don't need to
  restart the production. Dev server auto reloading also works as expected.

A sample web app is available
[here](https://github.com/sergeysolovev/webpack-aspnetcore/tree/master/samples/WebApp).

## Quick Start

Add a few lines to your
[Startup.cs](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/Startup.cs)

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddWebpack();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseWebpack(withDevServer: env.IsDevelopment());
    app.UseStaticFiles();
    app.UseMvcWithDefaultRoute();
}
```

Inject the asset path mapper into razor views, that need the assets

```html
@using Webpack.AspNetCore
@inject AssetPathMapper assets

<link rel="stylesheet" href="@await assets("index.css")" />
<img src="@await assets("static/media/banner1.svg")" />
<script src="@await assets("index.js")"></script>
```

**Tip**: Add `@using Webpack.AspNetCore` once to
`YourWebApp/Views/_ViewImports.cshtml` to avoid adding this line to every view.

Make sure that a valid asset manifest is available at

* `YouWebApp/wwwroot/dist/manifest.json` for the static assets
* `http://127.0.0.1:8080/manifest.json` if you use the dev server.

Run the [dev server](#dev-server-vs-dev-middleware) and the app. See
[this section](#default-configuration) on how it works and check out the
[sample app](https://github.com/sergeysolovev/webpack-aspnetcore/tree/master/samples/WebApp)
for more examples.

## Installation

Use NuGet Package Manager for Visual Studio to find and install the package
"Webpack.AspNetCore" or use dotnet CLI

```shell
dotnet add package Webpack.AspNetCore
```

## Prerequisites

You can use this extension for a web app that is built with <span>ASP.NET</span>
Core 2.0 and has a
[manifest](https://github.com/danethurber/webpack-manifest-plugin) for static
assets. To use it for serving the dev server assets you need
[webpack-dev-server](https://github.com/webpack/webpack-dev-server) installed.

Use [.NET Core 2.0 SDK](https://www.microsoft.com/net/download/core) to build
the source code and the
[sample app](https://github.com/sergeysolovev/webpack-aspnetcore/tree/master/samples/WebApp)

## Dev server vs. Dev middleware

It does not (on purpose) use any kind of
[webpack-dev-middleware](https://github.com/webpack/webpack-dev-middleware),
adopted for <span>ASP.NET</span> Core, to serve the assets in the
[dev mode](#the-static-and-the-dev-server-modes).

It means the dev server has to be started manually, but only once, so you don't
have to wait until all the assets get recompiled **every time you need to
rebuild or restart your web app**. If it's not an issue, check out
[Webpack dev middleware](https://github.com/aspnet/JavaScriptServices/tree/dev/src/Microsoft.AspNetCore.SpaServices#webpack-dev-middleware).

Though it's not a big deal to do `npm run start` to start the dev server, there
is
[NPM Task Runner](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner)
extension for Visual Studio, which lets you do this from the IDE and even bind
it to project opening.

## Default configuration

Default configuration, that's used in [Quick Start](#quick-start), works the
following way:

* In
  [development](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments)
  webpack assets are being served using the dev server reverse proxy middleware,
  assuming that the dev server is up and running and the manifest is available
  at `http://127.0.0.1:8080/manifest.json`.
* In non-development (production, staging, etc.) it serves the static webpack
  and non-webpack assets using an instance of
  [StaticFile](https://github.com/aspnet/StaticFiles) middleware, configured to
  read the assets from `YouWebApp/wwwroot/dist/` folder. The manifest file is
  expected to exist at `YouWebApp/wwwroot/dist/manifest.json`.

## Auto reloading

* For the dev server assets auto reloading and hot module replacement work as
  expected on the front-end by the means of the dev server's web socket.
* For the static webpack assets their paths are automatically reloaded on the
  back-end when any changes are made to the manifest file or it's folder. It's
  enough to refresh a page to see the changes. There's no need to restart
  Kestrel or recompile razor views.

## The static and the dev server modes

The extension can work in two modes: the static mode and the dev server mode. In
the static mode it serves the assets from a physical location and in the dev
mode it does so from the dev server. Since it provides a single API for
obtaining the dev server and the static assets' paths, it can only work in one
mode at the same time.

Use `app.UseWebpackStatic()` for the static mode and `app.UseWebpackDevServer()`
for the dev server mode in the `Configure` method of your startup class. You can
also use a single statement for the both modes

```csharp
app.UseWebpack(withDevServer: env.IsDevelopment());
```

This will make it work in the dev server mode for the development environment
and in the static mode for other environments.

**Tip**: You can use the static mode for non-webpack static assets, just make
sure that the
[manifest file format](https://github.com/danethurber/webpack-manifest-plugin#usage)
is valid.

**Tip**: Regardless of chosen middleware, make sure that connections to
`localhost` are fast enough on your development environment, as it can take long
time to resolve `localhost` name or establish a TCP connection. That's why
webpack-aspnetcore uses `127.0.0.1` as the dev server's host by default. To
check connection performance on your environment you can use curl with
`-w %{time_namelookup} %{time_connect}`. Here is an example of inadequate
connection time on a windows environment:

```shell
curl -s -w "lookup: %{time_namelookup}\nconnect: %{time_connect}\n" -o NUL http://localhost:8080/manifest.json
lookup: 0.006
connect: 1.009 # <-- something is wrong

curl -s -w "lookup: %{time_namelookup}\nconnect: %{time_connect}\n" -o NUL http://127.0.0.1:8080/manifest.json
lookup: 0.000
connect: 0.001
```

To fix this, check your `etc/hosts` and probably IPv6 configuration.

## Configuration

Here you can find how to tell webpack-aspnetcore where to look for the assets
and how to serve them.

### Modifying the default configuration

The default configuration can be modified in the `ConfigureServices` method of
your startup class using `AddStaticOptions` and `AddDevServerOptions` methods

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc();
    services.AddWebpack()
        .AddStaticOptions(options =>
            options.ManifestFileName = "asset-manifest.json"
        )
        .AddDevServerOptions(options => {
            options.ManifestFileName = "asset-manifest.json";
            options.Host = "192.167.1.100";
            options.Port = 8081;
            options.Scheme = "https";
        });
    // ...
}
```

This example shows how to change the default manifest file name to be
`asset-manifest.json` instead of `manifest.json` and sets up the dev server's
host, port and scheme.

**Tip**: The both `AddStaticOptions` and `AddDevServerOptions` methods can be
called multiple times and in any order.

### Manifest path for the static assets

The manifest path for the static assets is defined by `ManifestFileName` and
`ManifestDirectoryPath` properties and tells webpack-aspnetcore where to look
for the manifest and the assets. The `ManifestDirectoryPath` property defines
the location of the manifest, relative to <span>ASP.NET</span> Core
[Web Root](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/index?tabs=aspnetcore2x#web-root)
and defaults to "/dist/".

```csharp
services.AddWebpack().AddStaticOptions(options =>
    options.ManifestFileName = "webpack-assets.json";
    options.ManifestDirectoryPath = PathString.Empty;
);
```

The above example changes the default manifest file name from `manifest.json` to
`webpack-assets.json` and the default manifest directory path from `/dist/` to
an empty path string. This way the manifest and the assets will be read from
`YourWebApp/wwwroot/` instead of from `YourWebApp/wwwroot/dist/`.

### Manifest path for dev server assets

The manifest path for the dev server assets is defined by `ManifestFileName` and
`PublicPath` properties. The `PublicPath` property is the path the assets and
the manifest are being served from on the dev server and defaults to an empty
path string.

```csharp
services.AddWebpack().AddDevServerOptions(options =>
    options.ManifestFileName = "webpack-assets.json";
    options.PublicPath = "/some/public/path/";
);
```

The idea behind this example is the same as for the
[static assets](#manifest-path-for-the-static-assets).

### Public path

Configured
[webpack public path](https://webpack.js.org/configuration/output/#output-publicpath)
has to match the public path of your web app's webpack assets, which consists
of:

* [Request path base](https://docs.microsoft.com/en-us/aspnet/core/api/microsoft.aspnetcore.http.httprequest)
  which is not empty when you, for example, host your web app as an IIS virtual
  application
* [Request path](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files)
  – public path of the static assets, relative to request path base.

An example of using non-empty request path and request path base can be found
[here](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithPublicPath.cs).

### Configuring StaticFile middleware

Serving webpack and non-webpack static assets using StaticFile middleware is
[enabled](#disabling-statifiles-middleware) by default.

To configure StaticFile middleware, webpack-aspnetcore provides a limited set of
options. These options include all properties of
[StaticFileOptions](https://github.com/aspnet/StaticFiles/blob/dev/src/Microsoft.AspNetCore.StaticFiles/StaticFileOptions.cs),
except FileProvider, since a file provider for static webpack assets is defined
by the [manifest path](#manifest-path-for-the-static-assets) and can not be
modified.

An example of configuring static file options can be found
[here](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/StartupWithStaticFileOptions.cs).

### Serving static assets with a reverse proxy on production

Although webpack-aspnetcore by default uses StaticFile middleware to serve the
static assets, this is probably not how you want it to be done on production.
Nginx, Apache and IIS as reverse proxies in front of Kestrel do it more
efficiently by using their dedicated modules for serving static content.

When using a reverse proxy for serving static files, **no changes are needed for
webpack-aspnetcore configuration**, since these requests simply won't get to
Kestrel. Though you might want to use some kind of url rewriting of the reverse
proxy.

For example, check out how it's done in the sample app
[for IIS](https://github.com/sergeysolovev/webpack-aspnetcore/blob/master/samples/WebApp/web.config).

### Disabling StaticFile middleware

If for some reason you don't need to use StaticFile middleware, it can be
disabled

```csharp
services.AddWebpack(options => options.UseStaticFileMiddleware = false);
```

## Running tests

Use a script

```shell
# unix and osx
test/run.sh
```

This will start the dev server, required for integration tests, run the tests
and stop the server after that. Alternatively, use Visual Studio Test Explorer
and run the dev server before running the tests.

## Contributing

The goal of this project is to give <span>ASP.NET</span> Core users a way to use
all shiny and amazing webpack for production and development without pain. All
your help is highly appreciated!

Please, feel free to open a discussion
[here](https://github.com/sergeysolovev/webpack-aspnetcore/issues) and make your
pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file
for details
