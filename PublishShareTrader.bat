@echo off
title Publish Project

echo ==========================================
echo        Publishing Current Project
echo ==========================================
echo.

if exist "G:\Publish" (
    echo Cleaning G:\Publish...
    rmdir /S /Q "G:\Publish"
)

mkdir "G:\Publish"

echo Publishing...
dotnet publish "ShareTrader.csproj" -c Release -o "G:\Publish"
echo.
echo ==========================================
echo        Publish Complete
echo ==========================================
echo.
pause