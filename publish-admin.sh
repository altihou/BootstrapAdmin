#! /bin/bash

cd ~/BootstrapAdmin
git pull
dotnet publish src/admin/Bootstrap.Admin -c Release

rm -f ~/BootstrapAdmin/src/admin/Bootstrap.Admin/bin/Release/netcoreapp2.2/publish/appsettings*.json
rm -f ~/BootstrapAdmin/src/admin/Bootstrap.Admin/bin/Release/netcoreapp2.2/publish/BootstrapAdmin.db

systemctl stop ba.admin
\cp -fr ~/BootstrapAdmin/src/admin/Bootstrap.Admin/bin/Release/netcoreapp2.2/publish/* /usr/local/ba/admin/
systemctl start ba.admin
systemctl status ba.admin
