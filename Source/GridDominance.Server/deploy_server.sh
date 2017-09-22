#!/bin/bash

if [ $(id -u) != "0" ]
then
	echo ""
	echo "================================"
	echo " FORCING ROOT"
	echo "================================"
	echo ""
	sudo "$0" "$@"
	exit $?
fi


echo ""
echo "================================"
echo " PULLING PROJECT"
echo "================================"
echo ""

cd /home/janitor/gdapi_deploy

chown root:root GridDominance -R

cd GridDominance

git checkout master
git fetch
git reset --hard origin/master
git checkout master
git pull --force
git reset --hard

cd ..

echo ""
echo "================================"
echo " STATUS PROJECT"
echo "================================"
echo ""

cd GridDominance
git status
cd ..

echo ""
echo "================================"
echo " REMOVE DEV FILES"
echo "================================"
echo ""

chown janitor:janitor GridDominance -R

cd GridDominance
cd Source
cd GridDominance.Server

rm -rf .idea
#rm -rf data
rm .gitignore
rm internals/config.php

echo ""
echo "================================"
echo " COPY DATA TO APACHE"
echo "================================"
echo ""

cp * /var/www/gd_api/ -R
chown www-data:www-data /var/www/gd_api -R

echo "Success."
