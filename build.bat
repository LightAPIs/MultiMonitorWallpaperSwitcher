@echo off
echo ********** Build Multi-Monitor Wallpaper Switcher **********

REM var
set configuration=Release
set os=win

echo ********** Build Release_x86 **********
dotnet publish --self-contained false --configuration %configuration% --os %os% --arch x86 -p:PublishDir=..\publish\x86
echo ********** Build Release_x86 Result **********
tree /f .\publish\x86

echo ********** Build Release_x64 **********
dotnet publish --self-contained false --configuration %configuration% --os %os% --arch x64 -p:PublishDir=..\publish\x64
echo ********** Build Release_x64 Result **********
tree /f .\publish\x64

echo ********** Build Release_arm64 **********
dotnet publish --self-contained false --configuration %configuration% --os %os% --arch arm64 -p:PublishDir=..\publish\arm64
echo ********** Build Release_arm64 Result **********
tree /f .\publish\arm64

echo ********** Build Done. **********
REM pause>NUL
exit
