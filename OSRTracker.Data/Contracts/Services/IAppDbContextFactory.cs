using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace OSRTracker.Data.Contracts.Services;

public interface IAppDbContextFactory : IDbContextFactory<AppDbContext>
{
   void SetDbPath(string filePath);
}
