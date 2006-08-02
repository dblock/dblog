@echo off
pushd "%~dp0"
setlocal

set PROJECTNAME=DBlog
set TARGET=%~dp0%PROJECTNAME%
echo Creating %TARGET% ...
if EXIST "%TARGET%" ( rd /s/q "%TARGET%" )

echo Copying Web
xcopy /S /I "%~dp0..\Web.Deploy\Release\*.*" "%TARGET%"
echo Copying Sql
mkdir "%TARGET%\sql"
copy "%~dp0..\Database\*.*" "%TARGET%\sql"

if EXIST %PROJECTNAME%.zip del %PROJECTNAME%.zip
bin\zip32 -r %PROJECTNAME%.zip %PROJECTNAME%\*.*

endlocal
popd
