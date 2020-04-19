set local_source_path="%~dp0\LocalNuget"

del /s /q %local_source_path%
rmdir /s /q %local_source_path%

for %%f in (%~dp0\Packages\Release\*.nupkg) do (
	nuget add %%f -source %local_source_path%
)

