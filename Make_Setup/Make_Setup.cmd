REM @echo off
REM Following two could be parameters of the batch file
set Make_Setup="D:\Werk\devteam\vapus\Make_Setup"
set Build="D:\Werk\devteam\vapus\Build"

set CurrentDir="%CD%"
set InnoSetupCompiler="C:\Program Files (x86)\Inno Setup 5\Compil32.exe"
REM Set the sort order
set dircmd=/o:gne

cd %Build%

echo Adding the header
type %Make_Setup%\SetupHeader.txt>%Make_Setup%\Make_Setup.iss
echo.>>%Make_Setup%\Make_Setup.iss

echo Hiding the files and folder that are excluded in the build...
for /f "tokens=*" %%a in ('dir /A-H /S /B Logs SlaveSideResults *vshost* *Commit* Lock* vApus.UpdateTool.exe.config vApus.Report.exe.config vApus.LogFixer.exe.config *.pdb FastColoredTextBox.xml unsortedlupusproxytemplog*') do (
attrib +h "%%a"
)

echo Adding the folders that are not hidden...
echo [Dirs]>>%Make_Setup%\Make_Setup.iss

for /f "tokens=*" %%a in ('dir /AD-H /S /B') do (
set "folder=%%a"
call echo Name: {app}%%folder:%CD%=%%>>%Make_Setup%\Make_Setup.iss
)

echo Adding the files that are not hidden...
echo [Files]>>%Make_Setup%\Make_Setup.iss

for /f "tokens=*" %%a in ('dir /A-D-H /S /B') do (
set "folder=%%~dpa"
call echo Source: %%a; DestDir:{app}%%folder:%CD%=%%>>%Make_Setup%\Make_Setup.iss
)

echo Unhiding the files and folder in the build...
for /f "tokens=*" %%a in ('dir /AH /S /B') do (
attrib -h "%%a"
)

echo Build the setup using the Inno Setup 5 compiler
%InnoSetupCompiler% /cc %Make_Setup%\Make_Setup.iss

cd %CurrentDir%
echo Done!