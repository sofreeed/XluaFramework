@echo off
set "WORK_DIR=%cd%\Proto"
set "CS_OUT_PATH=%cd%\ProtoCS"

for /f "delims=" %%i in ('dir /b Proto "Proto/*.proto"') do (
	echo gen %%i
	protoc.exe --proto_path="%WORK_DIR%" --csharp_out="%CS_OUT_PATH%" "%WORK_DIR%\%%i")
)

echo finish
pause