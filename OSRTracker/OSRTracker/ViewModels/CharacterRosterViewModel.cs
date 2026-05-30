using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using OSRTracker.Contracts.ViewModels;

namespace OSRTracker.ViewModels;

public partial class CharacterRosterViewModel : ObservableRecipient, INavigationAware
{
   public CharacterRosterViewModel()
   {
      
   }

   public void OnNavigatedFrom()
   {
   }
   public void OnNavigatedTo(object parameter)
   {

   }
}
