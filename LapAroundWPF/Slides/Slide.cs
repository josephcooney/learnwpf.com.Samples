using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace LapAroundWPFTechEd2007.Slides
{
    public class Slide: System.Windows.Controls.Page
    {
        private string subtitle;

        public Slide()
        {
            base.ShowsNavigationUI = false;
        }

        static Slide()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Slide), new FrameworkPropertyMetadata(typeof(Slide)));
        }

        public string SubTitle
        {
            get { return subtitle; }
            set { subtitle = value; }
        }
    }
}
