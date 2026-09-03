using Microsoft.UI.Xaml.Controls;
using OSRTracker.ViewModels;
using OSRTracker.ViewModels.Pages.GamePlay;
using OSRTracker.Views.Pages.GamePlay;

namespace OSRTracker.Services;

internal interface IDialog<T, V>
   where T : ContentDialog, IDialog<T, V>
   where V : DialogViewModel
{
   static abstract T Create(V vm);
}
internal interface IViewRegistry
{
   void Register<TViewModel, TView>()
      where TViewModel : DialogViewModel
      where TView : ContentDialog, IDialog<TView, TViewModel>;

   ContentDialog CreateDialog(DialogViewModel vm);
}

internal class ViewRegistry : IViewRegistry
{
   private readonly Dictionary<Type, Func<DialogViewModel, ContentDialog>> factories = [];

   public ViewRegistry() { }

   public void Register<TViewModel, TView>()
      where TViewModel : DialogViewModel
      where TView : ContentDialog, IDialog<TView, TViewModel>
   {
      factories.Add(typeof(TViewModel), vm => TView.Create((TViewModel)vm));
   }

   public ContentDialog CreateDialog(DialogViewModel vm)
   {
      if (factories.TryGetValue(vm.GetType(), out var factory))
      {
         return factory(vm);
      }

      throw new ArgumentException($"No dialog for ViewModel {vm.GetType()}");
   }
}
