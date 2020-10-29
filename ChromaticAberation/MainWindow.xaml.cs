using System;
using System.Collections.Generic;
using System.Linq;
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

namespace LearnWPF.RgbSeparation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MouseMove += new MouseEventHandler(MainWindow_MouseMove);
        }

        void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(this);
            rgbFx.RedOffset = new Point((point.X - this.Width/2)/this.Width, (point.Y - this.Height /2) /this.Height);
            rgbFx.BlueOffset = new Point((this.Width/2 - point.X) / this.Width, (this.Height / 2 - point.Y) / this.Height);

        }
    }
}
