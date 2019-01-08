#!/bin/sh

cd /var/www/api.pvpgn.pro/WebAPI
dotnet build -c Release
systemctl restart api.pvpgn.pro.service