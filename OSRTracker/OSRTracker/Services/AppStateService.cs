using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OSRTracker.Contracts.Messages;
using OSRTracker.Contracts.Services;
using OSRTracker.Core.Models;
using OSRTracker.Data;
using OSRTracker.Data.Contracts.Services;

namespace OSRTracker.Services;

internal class AppStateService : IAppStateService
{
   private readonly IServiceProvider serviceProvider;

   public AppStateService(IServiceProvider serviceProvider)
   {
      this.serviceProvider = serviceProvider;
   }

   public CampaignSettings? Campaign
   {
      get; private set;
   }
   public string CampaignDbPath { get; private set; } = string.Empty;

   public async Task CreateCampaignAsync(string path)
   {
      this.CampaignDbPath = path;

      var parentDir = Path.GetDirectoryName(path);

      if (!Directory.Exists(parentDir))
      {
         Directory.CreateDirectory(parentDir);
      }

      var dbContextFactory = serviceProvider.GetRequiredService<IAppDbContextFactory>();

      dbContextFactory.SetDbPath(path);

      using var dbContext = dbContextFactory.CreateDbContext();

      await dbContext.Database.MigrateAsync();

      var campaign = new CampaignSettings()
      {
         Name = "New Campaign",
         AttributeNames = ["Str", "Int", "Wil", "Dex", "Con", "Cha"],
         SystemName = "ACKS"
      };

      await dbContext.CampaignSettings.AddAsync(campaign);

      await dbContext.SaveChangesAsync();

      Campaign = campaign;

      WeakReferenceMessenger.Default.Send<CampaignOpened>();
   }

   public async Task OpenCampaignAsync(string path)
   {
      this.CampaignDbPath = path;

      var dbContextFactory = serviceProvider.GetRequiredService<IAppDbContextFactory>();

      dbContextFactory.SetDbPath(path);

      using var dbContext = dbContextFactory.CreateDbContext();

      var campaign = await dbContext.CampaignSettings.FirstAsync();

      Campaign = campaign;

      WeakReferenceMessenger.Default.Send<CampaignOpened>();

   }
}
