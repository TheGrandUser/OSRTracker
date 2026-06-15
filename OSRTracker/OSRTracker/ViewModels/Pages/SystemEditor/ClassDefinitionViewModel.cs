using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OSRTracker.Models;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.ViewModels.Pages.SystemEditor;

namespace OSRTracker.ViewModels;

public partial class ClassDefinitionViewModel : UpdateableElementViewModel
{
   private readonly ClassDefinition classDefinition;
   private readonly ObservableCollection<ClassLevel> levels = [];

   private string name;

   public ClassDefinitionViewModel(ClassDefinition classDefinition, AppDbContext dbContext, ObservableCollection<AttributeDefinitionViewModel> attributes)
      : base(dbContext)
   {
      this.classDefinition = classDefinition;
      Attributes = attributes;
      name = classDefinition.Name;

      for (var i = 0; i < classDefinition.LevelXP.Count; i++)
      {
         var l = classDefinition.LevelXP[i];
         levels.Add(new ClassLevel(this, i + 1, l.XP));
      }

      foreach (var keyAttribute in classDefinition.KeyAttributes)
      {
         var vm = attributes.First(vm => vm.Attribute.Id == keyAttribute.Id);
         KeyAttributes.Add(vm);
      }

      KeyAttributes.CollectionChanged += KeyAttributes_CollectionChanged;
   }

   private void KeyAttributes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
   {
      Update();
   }

   public string Name { get => name; set => SetUpdatableProperty(ref name, value); }

   public ClassDefinition ClassDefinition => classDefinition;

   public ObservableCollection<ClassLevel> Levels => levels;
   public ObservableCollection<AttributeDefinitionViewModel> KeyAttributes { get; set; } = [];

   public ObservableCollection<AttributeDefinitionViewModel> Attributes { get; }

   [RelayCommand]
   private void AddKeyAttribute(AttributeDefinitionViewModel attribute)
   {
      KeyAttributes.Add(attribute);
      Update();
   }

   [RelayCommand]
   private void RemoveKeyAttribute(AttributeDefinitionViewModel attribute)
   {
      KeyAttributes.Remove(attribute);
      Update();
   }

   [RelayCommand]
   private void AddLevel()
   {
      var newLevel = new ClassLevel(this, levels.Count + 1, levels.LastOrDefault()?.XPRequired + 1 ?? 0);
      levels.Add(newLevel);
      Update();
   }

   [RelayCommand]
   private void RemoveLevel()
   {
      if (levels.Count > 1)
      {
         levels.RemoveAt(levels.Count - 1);
         Update();
      }
   }


   protected override void UpdateImpl(AppDbContext dbContext)
   {
      classDefinition.Name = Name;
      classDefinition.KeyAttributes.Clear();
      foreach (var keyAttribute in KeyAttributes)
      {
         classDefinition.KeyAttributes.Add(keyAttribute.Attribute);
      }
      classDefinition.LevelXP.Clear();
      foreach (var level in levels)
      {
         classDefinition.LevelXP.Add(new LevelXPRequirement(level.XPRequired));
      }

      //dbContext.ClassDefinitions.Update(classDefinition);
   }
}

public partial class ClassLevel(ClassDefinitionViewModel owner, int level, int xpRequired) : ObservableObject
{
   [ObservableProperty]
   public partial int Level { get; set; } = level;
   public int XPRequired {
      get => xpRequired;
      set {
         if (SetProperty(ref xpRequired, value))
         {
            owner.Update();
         }
      }
   }
}
