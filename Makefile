
run-linux-opengl: prepare
	cd "Source/GridDominance.OpenGL" && dotnet run

build-linux-opengl: prepare
	cd "Source/GridDominance.OpenGL" && dotnet publish -c Release -r linux-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained

prepare:
	cd "Source/GridDominance.Content.Pipeline" && dotnet build

sign:
	jarsigner -verbose \
	          -sigalg SHA1withRSA \
			  -digestalg SHA1 \
			  -keystore BFB_identity.keystore  \
			  -signedjar "Publish/Android.IAB/com.blackforestbytes.griddominance.iab.signed.aab" \
			  "Publish/Android.IAB/com.blackforestbytes.griddominance.iab.aab" bfb_identity
	jarsigner -verbose \
	          -sigalg SHA1withRSA \
			  -digestalg SHA1 \
			  -keystore BFB_identity.keystore  \
			  -signedjar "Publish/Android.Full/com.blackforestbytes.griddominance.full.signed.aab" \
			  "Publish/Android.Full/com.blackforestbytes.griddominance.full.aab" bfb_identity