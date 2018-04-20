SET Site=dnn710ce
set ver=debug
SET PROD=DNN.UrlRedirectProvider
SET PATH=DNN_UrlRedirectProvider
set website_path=website
set dnn_installs_path=..\..\..\sites
set UIOnly=All
if NOT x%1 == x""  set site=%1
if not x%2 == x""  set ver=%2
if not x%3 == x""  set UIOnly=%3
:copy
====module provider dll====
if %UIOnly% == All copy %PROD%\bin\%ver%\DNN.URLRedirect* %dnn_installs_path%\%site%\%website_path%\bin
========================
rem dir for file dates
dir %dnn_installs_path%\%site%\%website_path%\bin\%prod%.*

copy %prod%\UI\*.ascx %dnn_installs_path%\%site%\%website_path%\DesktopModules\%PATH%\
copy %prod%\UI\*.ashx %dnn_installs_path%\%site%\%website_path%\DesktopModules\%PATH%\
copy %prod%\UI\*.css %dnn_installs_path%\%site%\%website_path%\DesktopModules\%PATH%\
copy %prod%\UI\images\*.* %dnn_installs_path%\%site%\%website_path%\DesktopModules\%PATH%\images\
copy %prod%\UI\js\*.* %dnn_installs_path%\%site%\%website_path%\DesktopModules\%PATH%\js\
copy %prod%\UI\App_LocalResources\*.resx %dnn_installs_path%\%site%\%website_path%\DesktopModules\%PATH%\App_LocalResources\

ECHO OFF
echo The script was finished at:
date /t
time /t
