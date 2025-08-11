@echo off
set TIMESTAMP=%date:~0,4%%date:~5,2%%date:~8,2%
set BACKUP_PATH=C:\Users\imsadmin\Desktop\backup
set FILENAME=%BACKUP_PATH%\%TIMESTAMP%_backup.sql

"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqldump.exe" --host=localhost --port=3306 --default-character-set=utf8 --user=root --password=!KimForest --protocol=tcp --column-statistics=FALSE --routines --events --triggers "stock" > %FILENAME%

echo Backup completed: %FILENAME%


REM 获取当前日期，格式为 YYYYMMDD
for /f "tokens=1-4 delims=/-. " %%i in ("%date%") do (
    set yyyy=%%i
    set mm=%%j
    set dd=%%k
)

REM 拼接日期字符串
set TIMESTAMP=%yyyy%%mm%%dd%
cd /d %BACKUP_PATH%
git add .
git commit -m "備份%TIMESTAMP%"
git push