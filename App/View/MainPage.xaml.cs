namespace forecAstIng.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage(ForecastsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
