
API is represented by several methods:
* [/d2char](https://github.com/pvpgn/api.pvpgn.pro/wiki/d2char-method)
* [/bnethash](https://github.com/pvpgn/api.pvpgn.pro/wiki/bnethash-method)
* [/bnethash/crack](https://github.com/pvpgn/api.pvpgn.pro/wiki/bnethash-method#get-bnethashcrackhash)

[Diablo 2 Character Editor](https://api.pvpgn.pro/example/d2edit) is an example of usage /d2char.

For more info see [Wiki](https://github.com/pvpgn/api.pvpgn.pro/wiki)

# Installation

## Docker

`docker run -d -p 8080:80 harpywar/api.pvpgn.pro:latest`

## Binaries

Go to [Releases](https://github.com/pvpgn/api.pvpgn.pro/releases) and download static binary package for Windows or Linux. There is a single executable file.

By default the API is available to serve on http port 5000.

Required port numbers can be changed or disabled by passing a start parameter `--urls=http://0.0.0.0:8080`)

## From source code

1. [Install .NET Core SDK](https://dotnet.microsoft.com/download) for your platform
2. Clone this repository, cd into `WebAPI` directory and execute `dotnet run`
