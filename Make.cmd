@echo off
REM Set invironment variables
set IntelliLock="C:\Program Files (x86)\Eziriz\IntelliLock\Intellilock.exe"
set vApusBuild=Build
set vApusPrerequisites=Prerequisites
set lupusProxyBuild=..\lupusproxy\Build
set vApusSMTBuild=..\vapus-smt\Build

REM Make sure msysgit is installed with the cmd line option available during install
echo Getting the last commit of Lupus Proxy, this must be changed to pull using SSH, FTP or something from the vApusBuilder machine
REM cd ..\lupusproxy
REM Momenteel nog op de master
REM call git checkout development
REM call git pull
REM call git reset --hard
echo.

echo Getting the last commit of vApus SMT, this must be changed to pull using SSH, FTP or something from the vApusBuilder machine
REM cd ..\vapus-smt
REM call git checkout development
REM call git pull
REM call git reset --hard
echo.

REM Get back in the right folder
REM cd vapus

echo Copying Lupus Proxy to vApus Prerequisites
copy /y %lupusproxyBuild%\*.* "%vApusPrerequisites%\Lupus Proxy" > NUL
echo.

echo Copying needed vApus SMT Data to vApus Prerequisites 
copy /y %vApusSMTBuild%\vApusSMT.Base.dll "%vApusPrerequisites%\vApus SMT" > NUL
copy /y %vApusSMTBuild%\vApusSMT.Communication.dll "%vApusPrerequisites%\vApus SMT" > NUL
echo.

REM Rebuilding vApus
call "BuildvApus(vs10).bat"
echo.

echo Locking vApus assemblies
%IntelliLock% -project Lock_vApus.ilproj
copy /y %vApusBuild%\Locked %vApusBuild% > NUL
rmdir /s /q %vApusBuild%\Locked
echo.

echo Copying vApus SMT Data to the vApus Build Folder
echo -- Making and populating monitorsources sub dir
REM Switches at the end are for trimming the output 
robocopy /mir /mt %vApusSMTBuild%\monitorsources %vApusBuild%\monitorsources /njh /njs /ndl /nc /ns

echo -- Copying the other files
copy /y %vApusSMTBuild%\vApusSMT.exe %vApusBuild% > NUL
copy /y %vApusSMTBuild%\vApusSMT.exe.config %vApusBuild% > NUL
copy /y %vApusSMTBuild%\vApusSMT_GUI.exe %vApusBuild% > NUL
copy /y %vApusSMTBuild%\vApusSMT_GUI.exe.config %vApusBuild% > NUL
copy /y %vApusSMTBuild%\vApusSMT.Proxy.Local.dll %vApusBuild% > NUL

echo Putting the wmi communications dll in the build folder, this is a temp fix
copy /y %vApusSMTBuild%\vApusSMT.Windows.Communication.dll %vApusBuild% > NUL
echo.

echo Making the setup
call "Make_Setup\Make_Setup.cmd"

echo Finished, the setup can be found in the folder "Make_Setup"
pause