# build release packages


Function Build($dir, $arch)
{
	# clean dir
	if (Test-Path $dir)
	{
		Remove-Item $dir -Recurse -Force
	}
	dotnet publish -c Release /p:PublishSingleFile=true --runtime $arch --framework $framework WebAPI
}

Function Pack($dir, $arch)
{

	mkdir -p $dir\CharacterEditor\Resources
	# copy resources
	xcopy /y /s /e CharacterEditor\Resources $dir\CharacterEditor\Resources
	# create zip archive in the root (in new window, separate thread)
	# mx=9 // max compression
	# mmt=8 // use multithreading
	start $zip_exe "a -mx=9 -mmt=on api.pvpgn.pro_${version}_$arch.zip .\$dir\*"
}


$version = Select-String -Path .\WebAPI\WebAPI.csproj -Pattern '<Version>(.*)</Version>' -AllMatches | %{ $_.Matches.Groups[1] }  | %{ $_.Value }

$framework = "netcoreapp3.1"
$rdir = "WebAPI\bin\Release\$framework"
$zip_exe = "C:\Program Files\7-Zip\7z.exe"


echo "Framework: $framework"
echo "App Version: $version"
echo " "

# build for every architecture
$arch = "win-x64"
$publish = "$rdir\$arch\publish"
Build $publish $arch
Pack $publish $arch

$arch = "linux-x64"
$publish = "$rdir\$arch\publish"
Build $publish $arch
Pack $publish $arch

$arch = "osx-x64"
$publish = "$rdir\$arch\publish"
Build $publish $arch
Pack $publish $arch

