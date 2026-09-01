@echo off
title Publish ShareTrader

REM Load the Visual Studio build environment.
call "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"

cd /d "G:\ShareTraderSimMAUI\Windows"

echo Publishing ShareTrader...
echo.

dotnet publish ShareTrader.csproj ^
  -c Release ^
  -o "G:\Publish\ShareTrader"

if errorlevel 1 (
    echo.
    echo *** Publish FAILED ***
    pause
    exit /b 1
)

echo.
echo *** Publish SUCCESSFUL ***
start "" "G:\Publish\ShareTrader"
pause