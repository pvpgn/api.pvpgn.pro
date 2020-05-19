
API is represented by several methods:
* [/d2char](https://github.com/pvpgn/api.pvpgn.pro/wiki/d2char-method)
* [/bnethash](https://github.com/pvpgn/api.pvpgn.pro/wiki/bnethash-method)
* [/bnethash/crack](https://github.com/pvpgn/api.pvpgn.pro/wiki/bnethash-method#get-bnethashcrackhash)

[Diablo 2 Character Editor](https://api.pvpgn.pro/example/d2edit) is an example of usage /d2char.

For more info see [Wiki](https://github.com/pvpgn/api.pvpgn.pro/wiki)

# Installation

Go to [Releases](https://github.com/pvpgn/api.pvpgn.pro/releases) and download static binary package for Windows or Linux.

## Alternative way to run from source code

1. [Install .NET Core SDK](https://dotnet.microsoft.com/download) for your platform
2. Clone this repository, cd into `WebAPI` directory and execute `dotnet run`
3. The server is running and available to serve on port 8080 (port can be changed in Program.cs)


## How to build static binaries
```
cd WebAPI
dotnet publish -c release --self-contained --runtime win-x64 --framework netcoreapp2.2
dotnet publish -c release --self-contained --runtime linux-x64 --framework netcoreapp2.2
```
