#!/bin/sh

cd /var/www/api.pvpgn.pro/WebAPI
systemctl stop api.pvpgn.pro.service
dotnet clean && dotnet build -c Release
systemctl start api.pvpgn.pro.service
