@echo off
title Publish ShareTrader

REM Always run from the project folder.
cd /d "%~dp0"

echo.
echo Publishing ShareTrader...
echo.

"C:\Program Files\dotnet\dotnet.exe" publish ShareTrader.csproj ^
  -f net9.0-windows10.0.19041.0 ^
  -c Release ^
  -r win-x64 ^
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