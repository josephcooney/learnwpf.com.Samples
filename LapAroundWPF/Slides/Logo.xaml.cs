using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace LapAroundWPFTechEd2007
{
	public partial class Logo
	{
        private bool loaded = false;

		public Logo()
		{
			this.InitializeComponent();

			// Insert code required on object creation below this point.
		}

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (!loaded)
            {
                Carousel.Add(@"c:\sandbox\silverlight.wmv");
                Carousel.Add(@"c:\sandbox\cocoigor.wmv");
                Carousel.Add(@"c:\sandbox\Walking With Dinosaurs 6.mp4");
                Carousel.Build();
                Carousel.Activate();
                loaded = true;
            }
        }
        private void OnList3DItemSelected(object o, SelectionChangedEventArgs e)
        {
        }

        //this intercepts the mouseclick on 3d -- will also raise
        //the on selected event
        private void CarouselMouseEventInterceptorClicked(object o, MouseButtonEventArgs e)
        {
            Carousel.OnPreviewLeftClick(o, e);
        }
	}
}