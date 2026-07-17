using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Models;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;

namespace OSRTracker.Services;

internal class AppStateService(IServiceProvider serviceProvider, IAppDbContextFactory appDbContextFactory) : IAppStateService
{
   private readonly IServiceProvider serviceProvider = serviceProvider;
   private readonly IAppDbContextFactory dbContextFactory = appDbContextFactory;

   public CampaignSettings? Campaign { get; private set; }
   public string CampaignDbPath { get; private set; } = string.Empty;
   public SessionTrackId? ActiveSessionTrackId { get; private set; }

   public async Task CreateCampaignAsync(string path, string campaignName, SystemDto? rpgSystem)
   {
      CampaignDbPath = path;

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
               Id = new AttributeDefinitionId(i + 1),
               Name = rpgSystem.Attributes[i],
               Ordinal = i
            };
            attributeDefinitions[attribute.Name] = attribute;
            dbContext.AttributeDefinitions.Add(attribute);
         }

         foreach (var classdef in rpgSystem.Classes)
         {
            var classEntity = classdef.ToClassDefinition(attributeDefinitions);

            dbContext.ClassDefinitions.Add(classEntity);
         }
      }

      await dbContext.CampaignSettings.AddAsync(campaign);

      await dbContext.SaveChangesAsync();

      Campaign = campaign;

      WeakReferenceMessenger.Default.Send<CampaignOpened>();
   }

   public async Task<CampaignSettings> OpenCampaignAsync(string path)
   {
      var watch = new Stopwatch();
      watch.Start();
      CampaignDbPath = path;

      var startTime = watch.Elapsed;
      
      dbContextFactory.SetDbPath(path);

      using var dbContext = await dbContextFactory.CreateDbContextAsync();

      var gotDbTime = watch.Elapsed;

      await dbContext.Database.MigrateAsync();

      var afterMigrateCheckTime = watch.Elapsed;

      var campaign = await dbContext.CampaignSettings.FindAsync(new CampaignId(1));

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

      return campaign!;
   }


   public async Task RollbackCampaignAsync(string path)
   {
      var watch = new Stopwatch();
      watch.Start();
      CampaignDbPath = path;

      var startTime = watch.Elapsed;
      var dbContextFactory = serviceProvider.GetRequiredService<IAppDbContextFactory>();

      var gotFactoryTime = watch.Elapsed;

      dbContextFactory.SetDbPath(path);

      using var dbContext = dbContextFactory.CreateDbContext();

      var gotDbTime = watch.Elapsed;

      await dbContext.Database.MigrateAsync("20260604054841_LevelXP Property");

      var afterMigrateCheckTime = watch.Elapsed;

      var campaign = await dbContext.GetCampaignAsync();

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

   public void SetActiveSessionTrackId(SessionTrackId? activeSessionId)
   {
      ActiveSessionTrackId = activeSessionId;

      WeakReferenceMessenger.Default.Send(new ActiveSessionTrack(activeSessionId));
   }
}
