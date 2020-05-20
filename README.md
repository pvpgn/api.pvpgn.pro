
API is represented by several methods:
* [/d2char](https://github.com/pvpgn/api.pvpgn.pro/wiki/d2char-method)
* [/bnethash](https://github.com/pvpgn/api.pvpgn.pro/wiki/bnethash-method)
* [/bnethash/crack](https://github.com/pvpgn/api.pvpgn.pro/wiki/bnethash-method#get-bnethashcrackhash)

[Diablo 2 Character Editor](https://api.pvpgn.pro/example/d2edit) is an example of usage /d2char.

For more info see [Wiki](https://github.com/pvpgn/api.pvpgn.pro/wiki)

# Installation

Go to [Releases](https://github.com/pvpgn/api.pvpgn.pro/releases) and download static binary package for Windows or Linux. There is a single executable file.

By default the API is available to serve on ports 5000 (http) and 5001 (https).

Required port numbers can be changed or disabled by passing a start parameter `--urls=http://0.0.0.0:8080;https://0.0.0.0:8081`)

## Alternative way to run from source code

1. [Install .NET Core SDK](https://dotnet.microsoft.com/download) for your platform
2. Clone this repository, cd into `WebAPI` directory and execute `dotnet run`
