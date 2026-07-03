@REM dotnet build  --source .\handover_api\ --output .\output\

dotnet publish -c Release --self-contained --runtime win-x64 -p:PublishReadyToRun=true -p:PublishTrimmed=false
