param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$MigrationName
)

if ([string]::IsNullOrWhiteSpace($MigrationName)) {
    Write-Host "Error: Migration name cannot be empty." -ForegroundColor Red
    exit 1
}

Write-Host "Creating migration: $MigrationName" -ForegroundColor Cyan

dotnet ef migrations add $MigrationName `
   --project ../../OSRTracker.Data `
   --startup-project ../OSRTracker

if ($LASTEXITCODE -ne 0) {
    Write-Host "Migration creation failed." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "Migration '$MigrationName' created successfully." -ForegroundColor Green

# 2. Update Compiled Models
Write-Host "Updating Compiled Models..." -ForegroundColor Cyan

dotnet ef dbcontext optimize -c AppDbContext `
   --output-dir CompiledModels `
   --project ../../OSRTracker.Data `
   --startup-project ../OSRTracker

if ($LASTEXITCODE -ne 0) {
    Write-Host "Warning: Failed to optimize compiled models." -ForegroundColor Yellow
} else {
    Write-Host "Compiled models updated successfully." -ForegroundColor Green
}