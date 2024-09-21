namespace forecAstIng.ViewModel
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isWorking;

        [ObservableProperty]
        string title;
    }
}
