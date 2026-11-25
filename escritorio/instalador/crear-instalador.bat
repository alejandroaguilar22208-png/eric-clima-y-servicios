@echo off
cd /d "%~dp0"
echo Compilando ClimaGestion...
set MSBUILD=
for /f "usebackq delims=" %%i in (`"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set "MSBUILD=%%i"
if "%MSBUILD%"=="" goto error
"%MSBUILD%" ..\ClimaGestion\ClimaGestion.csproj -restore -p:Configuration=Release -p:DebugType=none -v:m
if errorlevel 1 goto error
if exist publicado rmdir /s /q publicado
mkdir publicado
copy /y ..\ClimaGestion\bin\Release\net48\*.* publicado >nul
echo Armando el instalador...
"%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe" ClimaGestion.iss
if errorlevel 1 goto error
echo.
echo Listo: el instalador esta en la carpeta "salida".
pause
exit /b 0
:error
echo Hubo un error. Revisa los mensajes de arriba.
pause
exit /b 1
