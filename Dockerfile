# docker build -t harpywar/api.pvpgn.pro:latest .
#
# docker run -d -p 8080:80 harpywar/api.pvpgn.pro:latest

FROM mcr.microsoft.com/dotnet/sdk:3.1 as build-env

WORKDIR /app
COPY . ./

RUN dotnet restore
RUN dotnet publish -c Release -o out

RUN mkdir -p out/CharacterEditor/Resources
RUN cp -r CharacterEditor/Resources out/CharacterEditor


FROM mcr.microsoft.com/dotnet/aspnet:3.1-alpine

WORKDIR /app

COPY --from=build-env /app/out /app

ENTRYPOINT ["sh", "-c", "dotnet WebAPI.dll"]