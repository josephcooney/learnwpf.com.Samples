using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace LapAroundWPFTechEd2007.Slides
{
	public partial class WhatIsWPF : Slide
	{
        private const string startingApp = @"
//references /r:""C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0\PresentationCore.dll"" /r:""C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0\PresentationFramework.dll"" /r:""C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0\WindowsBase.dll""
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

public class HelloWindow
{
    [STAThreadAttribute()]
	static void Main(string[] args)
	{
		
	}
}";

        public WhatIsWPF()
		{
			this.InitializeComponent();
            this.MouseUp += new System.Windows.Input.MouseButtonEventHandler(WhatIsWPF_MouseUp);
            wpfBlock.MouseUp += new System.Windows.Input.MouseButtonEventHandler(wpfBlock_MouseUp);
		}

        void wpfBlock_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                System.Diagnostics.Process.Start(@"C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0\");

                string helloPath = @"c:\temp\hello.txt";
                string exePath = @"d:\temp\hello.exe";

                if (File.Exists(helloPath))
                {
                    File.Delete(helloPath);
                }

                if (File.Exists(exePath))
                {
                    File.Delete(exePath);
                }

                using (StreamWriter writer = new StreamWriter(helloPath))
                {
                    writer.Write(startingApp);
                }

                System.Diagnostics.Process.Start(helloPath);

                e.Handled = true;
            }
        }

        void WhatIsWPF_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                DoubleAnimation fadeIn = new DoubleAnimation(1, new Duration(new TimeSpan(0, 0, 1)));
                clrBlock.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            }
        }
	}
}