namespace OSRTracker.Contracts.ViewModels;

public interface INavigationAware
{
    void OnNavigatedTo(object parameter);

    void OnNavigatedFrom();
}


//public interface IAsyncNavigationAware
//{
//   Task OnNavigatedToAsync(object parameter);

//   Task OnNavigatedFromAsync();
//}