cd ..
copy release\setup.exe sfx
copy release\Bike.Installer.msi sfx
cd sfx

"C:\Program Files\WinRAR\rar.exe" a -r -sfx -z"sfx.cfg" bike.exe setup.exe Bike.Installer.msi

del setup.exe
del Bike.Installer.msi