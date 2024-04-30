
run-linux-opengl: prepare
	cd "Source/GridDominance.OpenGL" && dotnet run

build-linux-opengl: prepare
	cd "Source/GridDominance.OpenGL" && dotnet publish -c Release -r linux-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained

prepare:
	cd "Source/GridDominance.Content.Pipeline" && dotnet build