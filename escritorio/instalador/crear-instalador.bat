@echo off
cd /d "%~dp0"
echo Compilando ClimaGestion...
dotnet publish ..\EricClima.Escritorio\EricClima.Escritorio.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=none -o publicado
if errorlevel 1 goto error
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
