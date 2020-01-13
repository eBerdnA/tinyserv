# TinyServ

Simple web server for serving static content from a directory. TinyServ is based on [EmbedIO](https://github.com/unosquare/embedio).

## Build

Windows

    dotnet publish -c Release --runtime win-x64

macOS 10.14 Mojave

    dotnet publish -c Release --runtime osx.10.14-x64

More identifiers can be found here: [.NET Core RID Catalog](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog)

## Run

TinyServ can simply be started without any command line parameters provided. In this case the context will be served from the assemblies path subdirectory `html`. Hostname and port will default to `http://localhost:8080/.

Possible parameters are:

* p|port= - port to use
* h|hostname= - hostname to use
* d|directory= - directory to serve content from
* b|browser - open browser after launching TinyServ
* c|fileCache - use file cache for content
