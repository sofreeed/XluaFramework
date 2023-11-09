set WORKSPACE=..\luban

set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\MiniTemplate

dotnet %LUBAN_DLL% ^
    -t all ^
    -d text-list  ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=%CONF_ROOT%\l10nOutput ^
	-x l10n.textListFile=keylist.txt ^
    -x l10n.textProviderFile=*@%CONF_ROOT%\l10n-texts.json

pause