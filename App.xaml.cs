using ShareTrader.Services;

namespace ShareTrader
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            window.Width = 1000;
            window.Height = 1000;
            window.Destroying += Window_Destroying;

            return window;
        }       

        private void Window_Destroying(object? sender, EventArgs e)
        {
            // Runs when the app is closing.
            FileManager.SaveConfig();
            FileManager.SaveBalances();
        }
    }

}