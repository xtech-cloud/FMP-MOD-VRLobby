
@echo off

REM !!! Generated by the fmp-cli 1.31.0.  DO NOT EDIT!

md VRLobby\Assets\3rd\fmp-xtc-vrlobby

cd ..\vs2022
dotnet build -c Release

copy fmp-xtc-vrlobby-lib-mvcs\bin\Release\netstandard2.1\*.dll ..\unity2021\VRLobby\Assets\3rd\fmp-xtc-vrlobby\