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
    /// Interaction logic for Style.xaml
    /// </summary>
    public partial class StyleSlide : Slide
    {
        public StyleSlide()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Style_Loaded);
        }

        void Style_Loaded(object sender, RoutedEventArgs e)
        {
            defaultButton.Click += new RoutedEventHandler(defaultButton_Click);
        }

        void defaultButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("I was clicked");
        }
    }
}
