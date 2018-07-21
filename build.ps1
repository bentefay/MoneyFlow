
# q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]
$configuration = "release"
$directory = Get-Location
function LogStart {
    Write-Host
    Write-Host $("+" * $args[0].Length)  -ForegroundColor DarkGray
    Write-Host $args[0] -ForegroundColor DarkGray
    Write-Host $("+" * $args[0].Length)  -ForegroundColor DarkGray
    Write-Host
}

function LogSection {
    Write-Host
    Write-Host $("=" * $args[0].Length)  -ForegroundColor DarkCyan
    Write-Host $args[0] -ForegroundColor DarkCyan
    Write-Host $("=" * $args[0].Length)  -ForegroundColor DarkCyan
    Write-Host
}

function Log {
    Write-Host $args[0]
}

LogStart "Pipeline for MoneyFlow"

Log "Building in configuration '$configuration'"

Log "Working directory: $directory"

Log "Dotnet SDK version: $(& dotnet --version)"

LogSection "Clean"
& Remove-Item -Recurse -Force $directory/artifacts

LogSection "Restore"
& dotnet restore src/Web --verbosity m

LogSection "Build"
& dotnet build src/Web --configuration $configuration --output $directory/artifacts/build --verbosity m

LogSection "Publish"
& dotnet publish src/Web --configuration $configuration --output $directory/artifacts/publish --verbosity m

LogSection "Zip"
Compress-Archive -Force -Path $directory/artifacts/publish -DestinationPath publish.zip