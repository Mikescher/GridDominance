#!/bin/bash

if [ $(id -u) != "0" ]
    then
    sudo "$0" "$@"
    exit $?
fi


cd /home/janitor/gdapi_deploy
rm -rf /home/janitor/gdapi_deploy/GridDominance

git clone https://github.com/Mikescher/GridDominance --depth 1

chown janitor:janitor GridDominance -R

cd GridDominance
cd Source
cd GridDominance.Server

rm -rf .idea
rm -rf data
rm .gitignore
rm internals/config.php

cp * /var/www/gd_api/ -R
chown www-data:www-data /var/www/gd_api -R

echo "Success."
