using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using Dapper;

namespace OSRTracker.Models.Mappers;

public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
{
   private static readonly JsonSerializerOptions options = new()
   {
      PropertyNameCaseInsensitive = true
      // add any other options you need
   };

   public override T? Parse(object value)
   {
      if (value is null or DBNull)
      {
         return default!;
      }

      var json = value.ToString();
      return string.IsNullOrWhiteSpace(json)
         ? default!
         : JsonSerializer.Deserialize<T>(json, options);
   }

   public override void SetValue(IDbDataParameter parameter, T? value)
   {
      parameter.Value = value is null
         ? DBNull.Value
         : JsonSerializer.Serialize<T>(value, options);
   }
}
