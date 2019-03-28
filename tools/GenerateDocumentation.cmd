@echo off

set _roslynatorExe="..\src\CommandLine\bin\Debug\net461\roslynator"
set _msbuildPath="C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin"
set _msbuildProperties="Configuration=Release"
set _rootDirectoryUrl="../../docs/api/"

%_msbuildPath%\msbuild "..\src\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

%_roslynatorExe% generate-doc "..\src\Core.sln" ^
 --msbuild-path %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 -o "..\docs\api" ^
 -h "Roslynator API Reference"

%_roslynatorExe% list-symbols "..\src\Core.sln" ^
 --msbuild-path %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 --visibility public ^
 --depth member ^
 --ignored-parts containing-namespace ^
 --ignored-attributes System.Runtime.CompilerServices.InternalsVisibleToAttribute ^
 --output "..\docs\api.txt"

%_roslynatorExe% generate-doc-root "..\src\Core.sln" ^
 --msbuild-path %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 --projects Core ^
 -o "..\src\Core\README.md" ^
 -h "Roslynator.Core" ^
 --root-directory-url %_rootDirectoryUrl%

%_roslynatorExe% generate-doc-root "..\src\Core.sln" ^
 --msbuild-path %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 --projects CSharp ^
 -o "..\src\CSharp\README.md" ^
 -h "Roslynator.CSharp" ^
 --root-directory-url %_rootDirectoryUrl%

%_roslynatorExe% generate-doc-root "..\src\Core.sln" ^
 --msbuild-path %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 --projects Workspaces.Core ^
 -o "..\src\Workspaces.Core\README.md" ^
 -h "Roslynator.CSharp.Workspaces" ^
 --root-directory-url %_rootDirectoryUrl%

%_roslynatorExe% generate-doc-root "..\src\Core.sln" ^
 --msbuild-path %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 --projects CSharp.Workspaces ^
 -o "..\src\CSharp.Workspaces\README.md" ^
 -h "Roslynator.CSharp.Workspaces" ^
 --root-directory-url %_rootDirectoryUrl%

pause
