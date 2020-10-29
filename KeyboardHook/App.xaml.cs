namespace HookSample
{
    using System;
    using System.Windows;
    using System.Threading;
    using System.Windows.Threading;

    public partial class App : Application
    {
        private MainWindow window;

        public App(KeyboardHook keyboardHook)
        {
            if (keyboardHook == null) throw new ArgumentNullException("keyboardHook");
            keyboardHook.KeyCombinationPressed += KeyCombinationPressed;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            window = new MainWindow();
            
            window.Show();
        }

        void KeyCombinationPressed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(ShowMainWindow));
        }

        private void ShowMainWindow()
        {
            KeyboardHook.ActivateWindow(window);
        }
    }
}
