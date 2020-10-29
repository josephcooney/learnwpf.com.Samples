using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace ClickOnceFileAssociation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null && AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null && AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Any())
            {
                string[] activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
                var uri = new Uri(activationData[0]);
                fullText.Text = File.ReadAllText(uri.LocalPath);
            }
        }
    }
}
