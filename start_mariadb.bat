@echo off
echo Starting MariaDB (will stop when you close this window)...

:: Set the data directory relative to this batch file
set DATA_DIR=%~dp0mariadb_data

:: Create the directory if it doesn't exist
if not exist "%DATA_DIR%" mkdir "%DATA_DIR%"

docker run --rm --name mariadb_dev_kartverket ^
    -e MYSQL_ROOT_PASSWORD=mysecretpassword ^
    -e MYSQL_DATABASE=mysql ^
    -e MYSQL_USER=root ^
    -p 3306:3306 ^
    -v "%DATA_DIR%:/var/lib/mysql" ^
    mariadb:latest

pause