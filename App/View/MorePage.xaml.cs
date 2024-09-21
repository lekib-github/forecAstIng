namespace forecAstIng.View
{
    public partial class MorePage : ContentPage
    {
        public MorePage(MorePageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
