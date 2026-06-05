dotnet ef migrations add %1
dotnet ef dbcontext optimize -c AppDbContext --output-dir CompiledModels