#!/bin/bash
set -e

dotnet msbuild ".." -restore /t:Build /p:Configuration=Release;Platform=Any;RuntimeIdentifier="win" /p:TargetFrameworkVersion=v4.6.2  
"%LOCALAPPDATA%\Playnite\Toolbox.exe" pack "..\PlayNext\bin\Release" "..\PlayNext\bin\build"
