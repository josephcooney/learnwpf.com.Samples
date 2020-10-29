using System;
using System.Windows;
// some extra XPS namespaces

namespace LapAroundWPF
{
    /// <summary>
    /// Interaction logic for Popout.xaml
    /// </summary>

    public partial class Popout : Window
    {
        private Uri pageUri;

        public Popout()
        {
            InitializeComponent();
        }

        public Uri PageUri
        {
            get { return pageUri; }
            set { 
                pageUri = value;
                popoutFrame.Navigate(pageUri);
            }
        }
    }
}
