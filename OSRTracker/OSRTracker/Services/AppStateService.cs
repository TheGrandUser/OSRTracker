using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Core.Models;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;
using OSRTracker.Models;

namespace OSRTracker.Services;

internal class AppStateService : IAppStateService
{
   private readonly IServiceProvider serviceProvider;
   private readonly IAppDbContextFactory dbContextFactory;

   public AppStateService(IServiceProvider serviceProvider, IAppDbContextFactory appDbContextFactory)
   {
      this.serviceProvider = serviceProvider;
      dbContextFactory = appDbContextFactory;
   }

   public CampaignSettings? Campaign {
      get; private set;
   }
   public string CampaignDbPath { get; private set; } = string.Empty;

   public async Task CreateCampaignAsync(string path, string campaignName, SystemDto? rpgSystem)
   {
      this.CampaignDbPath = path;

      var parentDir = Path.GetDirectoryName(path)!;

      if (!Directory.Exists(parentDir))
      {
         Directory.CreateDirectory(parentDir);
      }

      dbContextFactory.SetDbPath(path);

      using var dbContext = dbContextFactory.CreateDbContext();

      await dbContext.Database.MigrateAsync();

      var campaign = new CampaignSettings()
      {
         Name = campaignName,
         SystemName = rpgSystem?.SystemName ?? "Empty System"
      };

      if (rpgSystem is null)
      {
         var attributes = AttributeDefinition.Defaults.Select(d => new AttributeDefinition() { Name = d.Name, Ordinal = d.Ordinal }).ToArray();

         dbContext.AttributeDefinitions.AddRange(attributes);
      }
      else
      {
         Dictionary<string, AttributeDefinition> attributeDefinitions = [];
         for (var i = 0; i < rpgSystem.Attributes.Count; i++)
         {
            var attribute = new AttributeDefinition()
            {
               Id = i + 1,
               Name = rpgSystem.Attributes[i],
               Ordinal = i
            };
            attributeDefinitions[attribute.Name] = attribute;
            dbContext.AttributeDefinitions.Add(attribute);
         }

         foreach (var classdef in rpgSystem.Classes)
         {
            ClassDefinition classEntity = classdef.ToClassDefinition(attributeDefinitions);

            dbContext.ClassDefinitions.Add(classEntity);
         }
      }

      await dbContext.CampaignSettings.AddAsync(campaign);

      await dbContext.SaveChangesAsync();

      Campaign = campaign;

      WeakReferenceMessenger.Default.Send<CampaignOpened>();
   }

   public async Task OpenCampaignAsync(string path)
   {
      Stopwatch watch = new Stopwatch();
      watch.Start();
      this.CampaignDbPath = path;

      var startTime = watch.Elapsed;
      
      dbContextFactory.SetDbPath(path);

      using var dbContext = await dbContextFactory.CreateDbContextAsync();

      var gotDbTime = watch.Elapsed;

      await dbContext.Database.MigrateAsync();

      var afterMigrateCheckTime = watch.Elapsed;

      var campaign = await dbContext.CampaignSettings.FindAsync(1);

      Campaign = campaign;

      var loadedCampaignTime = watch.Elapsed;

      WeakReferenceMessenger.Default.Send<CampaignOpened>();
      watch.Stop();

      var finalTime = watch.Elapsed;

      Debug.WriteLine(
         $"""
         Start Time: - - - - {startTime}
         Got DB Time:        {gotDbTime}
         After Migrate Time: {afterMigrateCheckTime}
         Loaded Campn Time:  {loadedCampaignTime}
         Final Time:         {finalTime}
         """);
   }


   public async Task RollbackCampaignAsync(string path)
   {
      Stopwatch watch = new Stopwatch();
      watch.Start();
      this.CampaignDbPath = path;

      var startTime = watch.Elapsed;
      var dbContextFactory = serviceProvider.GetRequiredService<IAppDbContextFactory>();

      var gotFactoryTime = watch.Elapsed;

      dbContextFactory.SetDbPath(path);

      using var dbContext = dbContextFactory.CreateDbContext();

      var gotDbTime = watch.Elapsed;

      await dbContext.Database.MigrateAsync("20260604054841_LevelXP Property");

      var afterMigrateCheckTime = watch.Elapsed;

      var campaign = await dbContext.CampaignSettings.FindAsync(1);
      //.GetCampaignAsync();

      Campaign = campaign;

      var loadedCampaignTime = watch.Elapsed;

      WeakReferenceMessenger.Default.Send<CampaignOpened>();
      watch.Stop();

      var finalTime = watch.Elapsed;

      Debug.WriteLine(
         $"""
         Start Time: - - - - {startTime}
         Got Factory Time:   {gotFactoryTime}
         Got DB Time:        {gotDbTime}
         After Migrate Time: {afterMigrateCheckTime}
         Loaded Campn Time:  {loadedCampaignTime}
         Final Time:         {finalTime}
         """);
   }
}
