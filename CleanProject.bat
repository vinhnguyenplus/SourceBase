@echo OFF
setlocal enabledelayedexpansion

echo 正在清理项目文件...

REM Delete front-end node_modules (more efficient way)
if exist ".\Web\node_modules" (
    echo Deleting Web node_modules...
    rd /s /q ".\Web\node_modules" 2>nul
)

REM Clean the bin, obj, and public folders of the Admin.NET project
for /d /r ".\Admin.NET\" %%b in (bin obj public) do (
    if exist "%%b" (
        echo Deleting %%~b...
        rd /s /q "%%b" 2>nul
    )
)

echo 【Processing complete, press any key to exit】
pause >nul
exit /b 0