@echo off

bike relative_path_test.bk

echo ****************
md c:\lib_for_bike
@set BIKE_LIB=c:\lib_for_bike
bike addon_library_path_test.bk
rmdir c:\lib_for_bike

echo ****************
bike home_installation_path_test.bk

pause
@echo on