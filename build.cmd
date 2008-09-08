@echo off

if "%~1"=="" ( 
 call :Usage
 goto :EOF
)

pushd "%~dp0"
setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION

set ProjectName=DBlog

set VisualStudioCmd=%ProgramFiles%\Microsoft Visual Studio 8\VC\vcvarsall.bat

if EXIST "%VisualStudioCmd%" ( 
 call "%VisualStudioCmd%"
)

set SvnDir=%ProgramFiles%\svn
if NOT EXIST "%SvnDir%" set SvnDir=%ProgramFiles%\Subversion
if NOT EXIST "%SvnDir%" (
 echo Missing SubVersion, expected in %SvnDir%
 exit /b -1
)

set NUnitDir=%ProgramFiles%\NUnit 2.4.7\bin
if NOT EXIST "%NUnitDir%" (
 echo Missing NUnit, expected in %NUnitDir%
 exit /b -1
)

set FrameworkVersion=v3.5
set FrameworkDir=%SystemRoot%\Microsoft.NET\Framework

PATH=%FrameworkDir%\%FrameworkVersion%;%NUnitDir%;%SvnDir%;%PATH%
msbuild.exe %ProjectName%.proj /t:%*
popd
endlocal
goto :EOF

:Usage
echo  Syntax:
echo.
echo   build [target] /p:Configuration=[Debug (default),Release]
echo.
echo  Target:
echo.
echo   all : build everything
echo.
echo  Examples:
echo.
echo   build all
echo   build all /p:Configuration=Release
goto :EOF