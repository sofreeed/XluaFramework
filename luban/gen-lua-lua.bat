set WORKSPACE=.\

set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\MiniTemplate

dotnet %LUBAN_DLL% ^
    -t all ^
    -c lua-lua ^
    -d lua  ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=..\Client\Assets\LuaScripts\Luban\Gen ^
    -x outputDataDir=..\Client\Assets\LuaScripts\Luban\bytes ^
    -x pathValidator.rootDir=%WORKSPACE%\Projects\Csharp_Unity_bin ^
    -x l10n.textProviderFile=*@%CONF_ROOT%\l10n-texts.json

pause