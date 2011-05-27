#!/bin/sh -e

bike relative_path_test.bk

echo "****************"
mkdir ~/lib_for_bike
export BIKE_HOME=$HOME/lib_for_bike
bike addon_library_path_test.bk
rm -rf ~/lib_for_bike

echo "****************"
bike home_installation_path_test.bk

read        
 