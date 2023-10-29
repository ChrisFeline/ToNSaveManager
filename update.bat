@echo off

taskkill /im ToNSaveManager.exe

set "github_repo=ChrisFeline/ToNSaveManager"
set "release_tag=ToNSaveManager"

set "zip_file=%release_tag%.zip"
set "download_url=https://github.com/%github_repo%/releases/latest/download/%zip_file%"

cls
echo [1/2] Downloading %zip_file%
curl -L -o "%zip_file%" "%download_url%" --progress-bar

cls
echo [2/2] Extracting %zip_file%
powershell -NoLogo -NonInteractive -command "Expand-Archive -Path '%zip_file%' -DestinationPath './' -Force"
del "%zip_file%"

cls
echo Download Completed

pause