using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using OSRTracker.Contracts.ViewModels;
using OSRTracker.Data.Contracts.Services;

namespace OSRTracker.ViewModels;

public partial class CharacterRosterViewModel : ObservableRecipient, INavigationAware
{
   private readonly IAppDbContextFactory appDbContextFactory;

   public CharacterRosterViewModel(IAppDbContextFactory appDbContextFactory)
   {
      this.appDbContextFactory = appDbContextFactory;
   }

   public void OnNavigatedFrom()
   {
   }
   public void OnNavigatedTo(object parameter)
   {
      PopulateCharacters();
   }

   private void PopulateCharacters()
   {
      if(!this.appDbContextFactory.HasPath)
      {
         // Handle the case where the database path is not available
      }
   }
}
