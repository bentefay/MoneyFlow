
# q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]
$configuration = "release"

function Log {
    Write-Host
    Write-Host $("=" * $args[0].Length)  -ForegroundColor DarkCyan
    Write-Host $args[0] -ForegroundColor DarkCyan
    Write-Host $("=" * $args[0].Length)  -ForegroundColor DarkCyan
}   

Log "Building in configuration '$configuration'"

Log "Dotnet SDK version: $(& dotnet --version)"

Log "Restore"
& dotnet restore --verbosity m

Log "Build"
& dotnet build --configuration $configuration --verbosity m

Log "Publish"
& dotnet publish --configuration $configuration --output publish --verbosity m

Log "Zip"
Compress-Archive -Force -Path publish -DestinationPath publish.zip

