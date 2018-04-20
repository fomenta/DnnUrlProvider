ECHO OFF
REM !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
REM YOU NEED A COMMAND LINE VERSION OF 7Zip OR SIMILAR TO RUN THIS CODE
REM UPDATE 7zip path accordingly.
REM !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
ECHO ON
REM USAGE : packageForInstall.zip DEBUG or packageForInstall.zip RELEASE
ECHO OFF
REM !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

SET VER=01.01.02
SET COMPILE=RELEASE
ECHO OFF

if NOT x%1 == x""  set COMPILE=%1

REM delete the install directory for fresh files every time
del install /Q /F /S
echo Looking for any existing zip files - should be none
dir install\*.zip
md install
md install\resources
md install\resources\js
md install\resources\App_LocalResources
md install\resources\Images

SET PROD=DNN.UrlRedirectProvider
set PRODVER=%PROD%_%VER%
set SRC=%PROD%
rem no trailing \ on the site variable.  The site variable specifies the root site path for the source website
rem where the developed code sits
REM SET Site Path for Install copy
set Site=c:\inetpub\wwwroot\dotnetnuke

ECHO ================== Start Copy %PROD% ====================================================
ECHO ON
rem copy the eula and DNN package
copy %SRC%\package\*.dnn install\
copy %SRC%\package\*.html install\

REM Copy out of the bin directory, because all relevant dlls are in there
copy %SRC%\bin\%COMPILE%\DNN.UrlRedirectProvider*.dll Install\
if "%COMPILE%" == "DEBUG" copy %SRC%\bin\%COMPILE%\DNN.UrlRedirectProvider*.pdb Install\

REM Copy the relevant files out of the main directory
copy %SRC%\UI\*.ascx Install\Resources
copy %SRC%\UI\images\*.* Install\Resources\images
copy %SRC%\UI\js\*.* Install\Resources\js
copy %SRC%\UI\*.css Install\Resources
copy %SRC%\UI\App_LocalResources\*.resx Install\Resources\App_LocalResources

REM copy the sql data provider files out of the data directory
copy %SRC%\Data\SqlDataProvider\*.SqlDataProvider Install\

ECHO OFF
ECHO ================== Finish Copy and Zip %PROD% =======================

SET INSTALL_ZIP=%PROD%_%VER%_Install.zip
SET DEBUG_INSTALL_ZIP=%PROD%_%VER%_Debug_Install.zip
SET RESOURCES_ZIP=%PROD%_Resources.zip

ECHO OFF
ECHO ================== Start Zipping %PROD% ======================
ECHO ON
rem zip up the resources file
rem puts it into the install directory
ECHO OFF
ECHO ================ Resources Files ============================
ECHO ON
"c:\program files\7-zip\7za.exe"  a install\%RESOURCES_ZIP% .\install\resources\* -x!*.zip

rem zip up the install file
ECHO OFF
ECHO ================ Install Files ========================================================
ECHO ON
if "%COMPILE%" == "RELEASE" "c:\program files\7-zip\7za.exe"  a -tzip Install\%INSTALL_ZIP% .\install\*.* -x!%INSTALL_ZIP%
if "%COMPILE%" == "DEBUG" "c:\program files\7-zip\7za.exe"  a -tzip Install\%DEBUG_INSTALL_ZIP% .\install\*.* -x!%DEBUG_INSTALL_ZIP%
ECHO ON

copy install\%INSTALL_ZIP% %site%\install\module\
ECHO OFF



rem zip up the source code
SET SOURCE_ZIP=%PROD%_%VER%_Source.zip

ECHO Deleting old source version
DEL %SOURCE_ZIP%

ECHO ================== Start Copying Source for %PROD% ====================================================
ECHO ON
REM Copy out of the bin directory, because all relevant dlls are in there
REM "C:\program files\pkware\pkzipc.exe" -add %SOURCE_ZIP% install\%install_ZIP% 
"c:\program files\7-zip\7za.exe"  a %SOURCE_ZIP% -x!*.zip

ECHO Copy SOlution Files
"c:\program files\7-zip\7za.exe"  a %SOURCE_ZIP% .\*.sln -dir=specify
"c:\program files\7-zip\7za.exe"  a %SOURCE_ZIP% .\deployToTestSite.bat -r
"c:\program files\7-zip\7za.exe"  a %SOURCE_ZIP% .\*.testrunconfig -r
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% .\readme.txt -r
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% .\bin\*.dll -r

ECHO Copy main project
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\*.csproj -r
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\*.cs -r
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\Data\SqlDataProvider\*.SqlDataProvider -dir=specify
REM "c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\Data\*.cs -dir=specify
REM"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\Entities\*.cs -dir=specify
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\Package\* -r
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\UI\*.ascx -r
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\UI\*.ascx.cs -r
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\UI\*.dnn -r
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\UI\images\*.* -r
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\UI\js\*.* -r
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\UI\*.css -r
"c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\UI\App_LocalResources\*.resx -r
REM "c:\program files\7-zip\7za.exe" a %SOURCE_ZIP% %prod%\UI\App_LocalResources\*.cs -r

ECHO OFF