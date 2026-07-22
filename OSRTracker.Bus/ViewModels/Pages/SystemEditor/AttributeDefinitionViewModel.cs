using OSRTracker.Models;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;

namespace OSRTracker.ViewModels.Pages.SystemEditor;

public partial class AttributeDefinitionViewModel(AttributeDefinition attributeDefinition, IAppDbContextFactory appDbContext) : UpdateableElementViewModel(appDbContext)
{
   private string name = attributeDefinition.Name;

   protected override void UpdateImpl(AppDbContext dbContext)
   {
      var attribute = dbContext.AttributeDefinitions.Find(Id);

      if (attribute is null)
      {
         // Report error?

         return;
      }

      attribute.Name = Name;
   }

   public AttributeDefinitionId Id { get; } = attributeDefinition.Id;
   public int Ordinal { get; } = attributeDefinition.Ordinal;

   public string Name { get => name; set => SetUpdatableProperty(ref name, value); }
}