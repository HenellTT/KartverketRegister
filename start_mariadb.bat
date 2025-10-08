@echo off
echo Starting MariaDB (will stop when you close this window)...

docker run --rm --name mariadb_dev_kartverket -e MYSQL_ROOT_PASSWORD=mysecretpassword -e MYSQL_DATABASE=mysql -e MYSQL_USER=root -p 3306:3306 mariadb:latest

pause