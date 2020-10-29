using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using LapAroundWPF.Properties;
using Microsoft.Samples.KMoore.WPFSamples.Transition;
using System.Windows.Markup;
using System.Windows.Documents;


namespace LapAroundWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Frame f1;
        private Frame f2;

        bool frameFlag = true;
        Transition[] trans;
        Random rnd = new Random(System.DateTime.Now.Millisecond);
        Mutex m = new Mutex();

        List<Uri> slides = new List<Uri>();

        public MainWindow()
        {
            InitializeComponent();

            f1 = new Frame();
            f2 = new Frame();

            mainGrid.MouseUp += new MouseButtonEventHandler(mainGrid_MouseUp);
            mainGrid.MouseRightButtonUp += new MouseButtonEventHandler(mainGrid_MouseRightButtonUp);
            slideList.SelectionChanged += new SelectionChangedEventHandler(slideList_SelectionChanged);
            printButton.Click += new RoutedEventHandler(printButton_Click);
            popoutButton.Click += new RoutedEventHandler(popoutButton_Click);
            BuildSlideList();
            InitializeFrames();
            trans = this.FindResource("transitions") as Transition[];
            slideList.ItemsSource = slides;

            try
            {
                ShowSlide(slides[Settings.Default.CurrentSlideId]);
            }
            catch (Exception)
            {
                Settings.Default.CurrentSlideId = 0;
                Settings.Default.Save();
            }

            //InitializeWiiMote();
        }

        void popoutButton_Click(object sender, RoutedEventArgs e)
        {
            Popout pop = new Popout();
            pop.PageUri = GetCurrentFrame().Source;

            pop.Show();
        }

        void printButton_Click(object sender, RoutedEventArgs e)
        {
            Frame itemToPrint = GetCurrentFrame();

            // do printing here...
            string path = string.Format(@"c:\temp\{0}.xps", itemToPrint.Source.ToString().Replace("/", ".")); ;
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (XpsDocument doc = new XpsDocument(path, System.IO.FileAccess.ReadWrite))
            {
                XpsPackagingPolicy packagePolicy = new XpsPackagingPolicy(doc);
                XpsSerializationManager serializationMgr = new XpsSerializationManager(packagePolicy, false);

                serializationMgr.SaveAsXaml(itemToPrint); // this can be any UI element
            }

            System.Diagnostics.Process.Start(path);

        }

        private Frame GetCurrentFrame()
        {
            Frame itemToPrint = null;

            if (frameFlag)
            {
                itemToPrint = f2;
            }
            else
            {
                itemToPrint = f1;
            }
            return itemToPrint;
        }


        void slideList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = slideList.SelectedIndex;
            if (index != Settings.Default.CurrentSlideId)
            {
                ShowSlide(slides[index]);
                Settings.Default.CurrentSlideId = index;
                Settings.Default.Save();
            }
        }

        void mainGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                ShowNextSlide();
            }
        }

        private void InitializeFrames()
        {
            f1.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            f2.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        }

        void mainGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowPreviousSlide();
        }

        private void BuildSlideList()
        {
            slides.Add(new Uri("/slides/Logo.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/Intro.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/ContentFamilies.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/ThreeD.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/WhatIsWPF.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/timeline.xaml", UriKind.Relative));

            //slides.Add(new Uri("/slides/WPFSize.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/xamlintro.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/xaml.xaml", UriKind.Relative));

            slides.Add(new Uri("/slides/ContentModelIntro.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/ContentModel.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/ContentModel2.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/Layout/LayoutIntro.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/Layout/StackPanel.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/Layout/WrapPanel.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/Layout/Canvas.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/Layout/DockPanel.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/Layout/Grid.xaml", UriKind.Relative));

            slides.Add(new Uri("/slides/Controls.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/StyleIntro.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/Style.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/Typography.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/XPS.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/DataBinding.xaml", UriKind.Relative));
            slides.Add(new Uri("/slides/End.xaml", UriKind.Relative));
        }

        void ShowNextSlide()
        {
            Settings.Default.CurrentSlideId++;
            int index = Settings.Default.CurrentSlideId;

            if (index + 1 > slides.Count)
            {
                index = 0;
                Settings.Default.CurrentSlideId = 0;
            }

            ShowSlide(slides[index]);
            slideList.SelectedIndex = index;
            Settings.Default.Save();
        }

        void ShowPreviousSlide()
        {
            Settings.Default.CurrentSlideId--;
            int index = Settings.Default.CurrentSlideId;

            if (index < 0)
            {
                index = 0;
                Settings.Default.CurrentSlideId = 0;
                return;
            }

            ShowSlide(slides[index]);
            slideList.SelectedIndex = index;
            Settings.Default.Save();
        }

        private void ShowSlide(Uri slideUri)
        {
            mainPresenter.Transition = GetRandomTransition();
            if (frameFlag)
            {
                f1.Source = slideUri;
                mainPresenter.Content = f1;
            }
            else
            {
                f2.Source = slideUri;
                mainPresenter.Content = f2;
            }

            frameFlag = !frameFlag;
        }

        private Transition GetRandomTransition()
        {
            int index = rnd.Next(0, trans.Length);
            return trans[index];
        }
    }
}
