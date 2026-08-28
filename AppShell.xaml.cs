namespace ShareTrader
{
    public partial class AppShell : Shell
    {
        public MainPage MainPage => (MainPage)CurrentPage;

        public AppShell()
        {
            InitializeComponent();
        }
    }
}
