using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Models;

namespace OSRTracker.Contracts.Services;

public interface IRpgSystemFileService
{
   Task ExportAsync(string path, SystemDto data);
   Task<SystemDto> ImportAsync(string path);
}
