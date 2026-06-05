using System;
using System.Collections.Generic;
using System.Text;
using OSRTracker.Contracts.Services;
using OSRTracker.Core.Helpers;
using OSRTracker.Models;

namespace OSRTracker.Services;

internal class RpgSystemFileService : IRpgSystemFileService
{
   public async Task ExportAsync(string path, SystemDto data)
   {
      var str = await Json.StringifyAsync(data);

      await File.WriteAllTextAsync(path, str);
   }

   public async Task<SystemDto> ImportAsync(string path)
   {
      var str = await File.ReadAllTextAsync(path);

      var obj = await Json.ToObjectAsync<SystemDto>(str);

      return obj!;
   }
}
