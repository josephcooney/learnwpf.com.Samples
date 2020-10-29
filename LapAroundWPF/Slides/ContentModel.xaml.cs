using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LapAroundWPFTechEd2007.Slides
{
    /// <summary>
    /// Interaction logic for ContentModel.xaml
    /// </summary>

    public partial class ContentModel : Slide
    {
        public ContentModel()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ContentModel_Loaded);
        }

        void ContentModel_Loaded(object sender, RoutedEventArgs e)
        {
            List<String> items = new List<string>();
            items.Add("Steve Ballmer");
            items.Add("Bill Gates");
            myCombo.ItemsSource = items;
        }

    }
}