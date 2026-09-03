
Write-Host "Removing migration" -ForegroundColor Cyan

dotnet ef migrations remove `
   --project ../OSRTracker.Data `
   --startup-project ../OSRTracker.EfHost `
   --context AppDbContext

if ($LASTEXITCODE -ne 0) {
    Write-Host "Migration removal failed." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "Migration removed successfully." -ForegroundColor Green

# 2. Update Compiled Models
Write-Host "Updating Compiled Models..." -ForegroundColor Cyan

dotnet ef dbcontext optimize -c AppDbContext `
   --output-dir CompiledModels `
   --project ../OSRTracker.Data `
   --startup-project ../OSRTracker.EfHost

if ($LASTEXITCODE -ne 0) {
    Write-Host "Warning: Failed to optimize compiled models." -ForegroundColor Yellow
} else {
    Write-Host "Compiled models updated successfully." -ForegroundColor Green
}