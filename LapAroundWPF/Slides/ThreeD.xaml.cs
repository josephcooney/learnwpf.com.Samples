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
    /// Interaction logic for ThreeD.xaml
    /// </summary>
    public partial class ThreeD : Slide
    {
        public ThreeD()
        {
            InitializeComponent();
            vectorBasedButton.Click += new RoutedEventHandler(vectorBasedButton_Click);
        }

        void vectorBasedButton_Click(object sender, RoutedEventArgs e)
        {
            ClothDemo cd = new ClothDemo();
            cd.Create3D(vp);
        }
    }
}
