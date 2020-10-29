using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for Controls.xaml
    /// </summary>
    public partial class Controls : Slide
    {
        public Controls()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Controls_Loaded);
        }

        void Controls_Loaded(object sender, RoutedEventArgs e)
        {
            retheme.Click += new RoutedEventHandler(retheme_Click);
        }

        void retheme_Click(object sender, RoutedEventArgs e)
        {
            ResourceDictionary themeDictionary = Application.LoadComponent(new Uri("Resources/SimpleStyles.xaml", UriKind.Relative)) as ResourceDictionary;
            if (themeDictionary != null)
            {
                this.Resources.MergedDictionaries.Add(themeDictionary);
            }
        }
    }
}
