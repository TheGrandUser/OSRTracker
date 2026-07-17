using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Dapper;
using OSRTracker.Helpers;
using OSRTracker.Models;

namespace OSRTracker;

internal class TypeMappings
{
   internal static void AddTypeMappings()
   {
      Setup<AttributeDefinitionId>();
      Setup<CampaignId>();
      Setup<CharacterId>();
      Setup<ClassDefinitionId>();
      Setup<CurrencyDefinitionId>();
      Setup<DelveId>();
      Setup<GeneralXPAwardId>();
      Setup<MonsterEntryId>();
      Setup<SessionId>();
      Setup<SessionDelveId>();
      Setup<SessionTrackId>();
      Setup<TreasureEntryId>();

      SqlMapper.AddTypeHandler(new JsonTypeHandler<LevelXPRequirement[]>());
      SqlMapper.AddTypeHandler(new JsonTypeHandler<List<LevelXPRequirement>>());


      static void Setup<T>() where T : struct, IEntityId<T>
      {
         TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(EntityIdTypeConverter<T>)));
         SqlMapper.AddTypeHandler(new EntityIdTypeHandler<T>());
      }
   }

}

internal class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
{
   public override T? Parse(object value) => value is string json ? Json.ToObject<T>(json) : default;
   public override void SetValue(IDbDataParameter parameter, T? value) => parameter.Value = value is null ? null : Json.Stringify(value);
}

internal class EntityIdTypeConverter<T> : TypeConverter
   where T : struct, IEntityId<T>
{
   public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(int) || sourceType == typeof(string);

   public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
   {
      return value switch
      {
         int i => T.Create(i),
         string s when int.TryParse(s, out var i) => T.Create(i),
         _ => base.ConvertFrom(context, culture, value)
      };
   }

   public override bool CanConvertTo(ITypeDescriptorContext? context, [NotNullWhen(true)] Type? destinationType) =>
      destinationType == typeof(int) || destinationType == typeof(string);

   public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
   {
      if (value is T id)
      {
         if (destinationType == typeof(int))
         {
            return id.Value;
         }
         else if (destinationType == typeof(string))
         {
            return id.Value.ToString();
         }
      }

      return base.ConvertTo(context, culture, value, destinationType);
   }
}

internal class EntityIdTypeHandler<T> : SqlMapper.TypeHandler<T>
   where T : struct, IEntityId<T>
{
   public override T Parse(object value)
   {
      return value switch
      {
         int i => T.Create(i),
         null => T.Empty,
         _ => T.Create(Convert.ToInt32(value))
      };
   }

   public override void SetValue(IDbDataParameter parameter, T value)
   {
      parameter.DbType = DbType.Int32;
      parameter.Value = value.Value;
   }
}
