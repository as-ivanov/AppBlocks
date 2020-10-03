set local_source_path="%~dp0\LocalNuget"

del /s /q %local_source_path%
rmdir /s /q %local_source_path%

for %%f in (%~dp0\Packages\Debug\*.nupkg) do (
	nuget add %%f -source %local_source_path%
)

rd /s /q "%userprofile%\.nuget\packages\appblocks.codegeneration.attributes.common"
rd /s /q "%userprofile%\.nuget\packages\appblocks.logging.codegeneration.attributes"
rd /s /q "%userprofile%\.nuget\packages\appblocks.logging.codegeneration.roslyn"
rd /s /q "%userprofile%\.nuget\packages\appblocks.logging.sdk"
rd /s /q "%userprofile%\.nuget\packages\appblocks.monitoring.abstractions"
rd /s /q "%userprofile%\.nuget\packages\appblocks.monitoring.appmetrics"
rd /s /q "%userprofile%\.nuget\packages\appblocks.monitoring.codegeneration.attributes"
rd /s /q "%userprofile%\.nuget\packages\appblocks.monitoring.codegeneration.roslyn"
rd /s /q "%userprofile%\.nuget\packages\appblocks.monitoring.sdk"


