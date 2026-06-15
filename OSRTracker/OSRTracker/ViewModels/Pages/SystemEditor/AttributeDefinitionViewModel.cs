using OSRTracker.Models;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;

namespace OSRTracker.ViewModels.Pages.SystemEditor;

public class AttributeDefinitionViewModel : UpdateableElementViewModel
{
   private string name;

   public AttributeDefinition Attribute { get; }

   public AttributeDefinitionViewModel(AttributeDefinition attributeDefinition, AppDbContext appDbContext)
      : base(appDbContext)
   {
      Attribute = attributeDefinition;
      this.name = attributeDefinition.Name;
   }

   protected override void UpdateImpl(AppDbContext dbContext)
   {
      Attribute.Name = Name;
      //dbContext.AttributeDefinitions.Update(Attribute);
   }

   public int Ordinal => Attribute.Ordinal;

   public string Name { get => name; set => SetUpdatableProperty(ref name, value); }
}