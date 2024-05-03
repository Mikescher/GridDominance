#!/bin/bash

    # shellcheck disable=SC2002  # disable useless-cat warning

    set -o nounset   # disallow usage of unset vars  ( set -u )
    set -o errexit   # Exit immediately if a pipeline returns non-zero.  ( set -e )
    set -o errtrace  # Allow the above trap be inherited by all functions in the script.  ( set -E )
    set -o pipefail  # Return value of a pipeline is the value of the last (rightmost) command to exit with a non-zero status
    IFS=$'\n\t'      # Set $IFS to only newline and tab.

    # shellcheck disable=SC2034
    cr=$'\n'

    function black() { if [ -t 1 ] && [ -n "$(tput colors)" ] && [ "$(tput colors)" -ge 8 ]; then echo -e "\\x1B[30m$1\\x1B[0m"; else echo "$1"; fi  }
    function red()   { if [ -t 1 ] && [ -n "$(tput colors)" ] && [ "$(tput colors)" -ge 8 ]; then echo -e "\\x1B[31m$1\\x1B[0m"; else echo "$1"; fi; }
    function green() { if [ -t 1 ] && [ -n "$(tput colors)" ] && [ "$(tput colors)" -ge 8 ]; then echo -e "\\x1B[32m$1\\x1B[0m"; else echo "$1"; fi; }
    function yellow(){ if [ -t 1 ] && [ -n "$(tput colors)" ] && [ "$(tput colors)" -ge 8 ]; then echo -e "\\x1B[33m$1\\x1B[0m"; else echo "$1"; fi; }
    function blue()  { if [ -t 1 ] && [ -n "$(tput colors)" ] && [ "$(tput colors)" -ge 8 ]; then echo -e "\\x1B[34m$1\\x1B[0m"; else echo "$1"; fi; }
    function purple(){ if [ -t 1 ] && [ -n "$(tput colors)" ] && [ "$(tput colors)" -ge 8 ]; then echo -e "\\x1B[35m$1\\x1B[0m"; else echo "$1"; fi; }
    function cyan()  { if [ -t 1 ] && [ -n "$(tput colors)" ] && [ "$(tput colors)" -ge 8 ]; then echo -e "\\x1B[36m$1\\x1B[0m"; else echo "$1"; fi; }
    function white() { if [ -t 1 ] && [ -n "$(tput colors)" ] && [ "$(tput colors)" -ge 8 ]; then echo -e "\\x1B[37m$1\\x1B[0m"; else echo "$1"; fi; }

    # cd "$(dirname "$0")" || exit 1  # (optionally) cd to directory where script is located



############################################################################################################################################


echo ""
blue "================ [ REMOVE EXISTING HTML ] ================="
echo ""

if [ -d "/var/www/html" ]; then

    rm -rf "/var/www/html"

fi

echo ""
blue "================ [ COPY HTML ] ================="
echo ""

cp -rv "/_html" "/var/www/html"

rm "/var/www/html/.gitignore"
rm "/var/www/html/.dockerignore"
rm "/var/www/html/DOCKER_GIT_INFO"
rm "/var/www/html/composer.json"

echo ""
blue "================ [ REPLACE ENV ] ================="
echo ""

repl_config_env() {
    if [ -z "${!1}" ]; then
        red "The environment variable $1 is not set. Exiting."
        exit 1
    fi
    sed -i "s|{{$1}}|${!1}|g" /var/www/html/internals/config.php
    green "Replaced {{$1}} with [${!1}] in /var/www/html/internals/config.php"
}

repl_config_env "DB_HOST"
repl_config_env "DB_USER"
repl_config_env "DB_PASS"
repl_config_env "SIGNATURE_KEY"
repl_config_env "CRON_SECRET"
repl_config_env "SENDMAIL"
repl_config_env "SENDNOTIFICATION"
repl_config_env "DEBUG"
repl_config_env "RUNLOG"

echo ""
blue "================ [ ENSURE DIRS ] ================="
echo ""

mkdir -p "/var/log/gdapi_log"

mkdir -p "/media/gdapi_userlevel/"

echo ""
blue "================ [ TEST PERMISSIONS ] ================="
echo ""

su - "www-data" -s "/bin/bash" -c 'touch "/media/gdapi_userlevel/.docker-perm-test" && rm "/media/gdapi_userlevel/.docker-perm-test"'

su - "www-data" -s "/bin/bash" -c 'touch "/var/log/gdapi_log/.docker-perm-test" && rm "/var/log/gdapi_log/.docker-perm-test"'

echo ""
blue "================ [ DONE ] ================="
echo ""
