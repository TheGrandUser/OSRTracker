using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Data;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using Microsoft.Windows.Storage.Pickers;
using OSRTracker.Contracts.Services;
using OSRTracker.Contracts.ViewModels;
using OSRTracker.Models;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Services;
using ThrottleDebounce;

namespace OSRTracker.ViewModels.Pages.SystemEditor;

public partial class SystemEditorPageViewModel : ObservableObject, INavigationAware
{
   private readonly IAppDbContextFactory contextFactory;
   private readonly IRpgSystemFileService rpgSystemFileService;
   private readonly IDialogService dialogService;

   private CampaignSettings? campaignSettings = null;
   private string name;
   private string systemName;

   private readonly ObservableCollection<ClassDefinitionViewModel> classes = [];
   private readonly ObservableCollection<AttributeDefinitionViewModel> attributes = [];
   private readonly ObservableCollection<AttributeDefinition> attributeDefinitions = [];

   public SystemEditorPageViewModel(IAppDbContextFactory contextFactory, IRpgSystemFileService rpgSystemFileService, IDialogService dialogService)
   {
      this.contextFactory = contextFactory;
      this.rpgSystemFileService = rpgSystemFileService;
      this.dialogService = dialogService;
      name = string.Empty;
      systemName = string.Empty;

      ClassesView = new AdvancedCollectionView(classes, true);
      ClassesView.SortDescriptions.Add(new SortDescription(nameof(ClassDefinitionViewModel.Name), SortDirection.Ascending));

      AttributesView = new AdvancedCollectionView(attributes, false);
      AttributesView.SortDescriptions.Add(new SortDescription(nameof(AttributeDefinitionViewModel.Ordinal), SortDirection.Ascending));
   }

   [ObservableProperty]
   public partial bool NoDatabase { get; private set; }

   public string Name {
      get => name;
      set {
         if (SetProperty(ref name, value))
         {
            if (campaignSettings is not null)
            {
               campaignSettings.Name = value;
               UpdateCampaign();
            }
         }
      }
   }

   public string SystemName {
      get => systemName;
      set {
         if (SetProperty(ref systemName, value))
         {
            if (campaignSettings is not null)
            {
               campaignSettings.SystemName = value;
               UpdateCampaign();
            }
         }
      }
   }

   public AdvancedCollectionView ClassesView { get; }

   [ObservableProperty]
   public partial ClassDefinitionViewModel? SelectedClass { get; set; }

   public ObservableCollection<AttributeDefinitionViewModel> Attributes => attributes;
   public AdvancedCollectionView AttributesView { get; }

   public void OnNavigatedFrom()
   {

      using var dbContext = contextFactory.CreateDbContext();


      foreach (var classDef in classes)
      {
         classDef.ForceUpdate(dbContext);
      }

      dbContext.SaveChanges();

      dbContext.Dispose();
   }
   public void OnNavigatedTo(object parameter)
   {
      PopulateScreen();
   }

   private async void PopulateScreen()
   {
      if (!contextFactory.HasPath)
      {
         NoDatabase = true;

         return;
      }

      using var dbContext = contextFactory.CreateDbContext();

      var soleCampaign = new CampaignId(1);
      campaignSettings = await dbContext.CampaignSettings.AsNoTracking().FirstAsync(x => x.Id == soleCampaign);

      name = campaignSettings.Name;
      systemName = campaignSettings.SystemName;
      OnPropertyChanged(nameof(Name));
      OnPropertyChanged(nameof(SystemName));

      await dbContext.Database.OpenConnectionAsync();

      var connection = dbContext.Database.GetDbConnection();
      

      await foreach (var attributeDefinition in dbContext.AttributeDefinitions.OrderBy(a => a.Ordinal).AsAsyncEnumerable())
      {
         var attributeDefinitionViewModel = new AttributeDefinitionViewModel(attributeDefinition, contextFactory);

         attributes.Add(attributeDefinitionViewModel);
         attributeDefinitions.Add(attributeDefinition);
      }

      if (attributes.Count < 6)
      {
         var ordinals = attributes.Select(a => a.Ordinal).ToHashSet();
         foreach (var defaultAttribute in AttributeDefinition.Defaults.Where(a => !ordinals.Contains(a.Ordinal)))
         {
            var attributeDefinition = new AttributeDefinition()
            {
               Name = defaultAttribute.Name,
               Ordinal = defaultAttribute.Ordinal
            };
            var attributeDefinitionViewModel = new AttributeDefinitionViewModel(attributeDefinition, contextFactory);
            attributes.Add(attributeDefinitionViewModel);
            attributeDefinitions.Add(attributeDefinition);
            await dbContext.AttributeDefinitions.AddAsync(attributeDefinition);
            await dbContext.SaveChangesAsync();
         }
      }

      await foreach (var classDefinition in dbContext.ClassDefinitions.Include(c => c.KeyAttributes).AsAsyncEnumerable())
      {
         var classDefinitionViewModel = new ClassDefinitionViewModel(classDefinition, contextFactory, attributes);

         classes.Add(classDefinitionViewModel);
      }

   }

   private void UpdateCampaign()
   {
      if (campaignSettings is null)
      {
         return;
      }

      using var dbContext = contextFactory.CreateDbContext();

      dbContext.CampaignSettings.Update(campaignSettings);
      dbContext.SaveChanges();
   }

   [RelayCommand]
   private async Task AddClass()
   {
      var classDef = new ClassDefinition()
      {
         Id = ClassDefinitionId.Empty,
         Name = "New Class",
         KeyAttributes = [],
         LevelXP = [new LevelXPRequirement(0)]
      };

      {
         using var dbContext = contextFactory.CreateDbContext();

         var result = await dbContext.ClassDefinitions.AddAsync(classDef);

         await dbContext.SaveChangesAsync();
      }

      var classDefViewModel = new ClassDefinitionViewModel(classDef, contextFactory, attributes);

      classes.Add(classDefViewModel);

      SelectedClass = classDefViewModel;
   }

   [RelayCommand]
   private async Task ExportSystem()
   {
      if (campaignSettings is null)
      {
         return;
      }

      var result = await dialogService.PickSaveFileAsync(fileTypeChoices: [("JSON File", [".json"])]);

      if (string.IsNullOrEmpty(result.Path))
      {
         return;
      }

      var exportData = new SystemDto
      {
         SystemName = campaignSettings.SystemName,
         Attributes = [.. attributes.OrderBy(a => a.Ordinal).Select(a => a.Name)],
         Classes = [.. classes.Select(c => c.CreateExportDto())]
      };

      await rpgSystemFileService.ExportAsync(result.Path, exportData);
   }

   [RelayCommand]
   private async Task ImportSystem()
    {
      if (campaignSettings is null)
      {
         return;
      }

      using var dbContext = contextFactory.CreateDbContext();

      if (dbContext.Characters.Any())
      {
         // Show warning dialog about overwriting data

         var result = await dialogService.ShowContentDialog(
            "Import System",
            "Importing this system will overwrite existing character data. Continue?",
            primaryButtonText: "Continue",
            closeButtonText: "Cancel");
            
         if (result != Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
         {
            return;
         }
      }

      {
         var result = await dialogService.PickSingleFileAsync([("JSON File", [".json"])]);

         if (string.IsNullOrEmpty(result.Path))
         {
            return;
         }

         var importData = await rpgSystemFileService.ImportAsync(result.Path);


         campaignSettings.SystemName = importData.SystemName;

         Dictionary<string, AttributeDefinition> attributeMap = [];

         for (var i = 0; i < importData.Attributes.Count; i++)
         {
            var attribute = importData.Attributes[i];
            var attributeDefinition = new AttributeDefinition
            {
               Id = new AttributeDefinitionId(i + 1),
               Name = attribute,
               Ordinal = i,
            };
            dbContext.AttributeDefinitions.Update(attributeDefinition);
            attributeMap[attribute] = attributeDefinition;
         }


         foreach (var classDto in importData.Classes)
         {
            var classDefinition = classDto.ToClassDefinition(attributeMap);
            dbContext.ClassDefinitions.Add(classDefinition);
         }


      }
   }
}
