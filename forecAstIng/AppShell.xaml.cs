using forecAstIng.View;

namespace forecAstIng
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute(nameof(MorePage), typeof(MorePage));
        }
    }
}
