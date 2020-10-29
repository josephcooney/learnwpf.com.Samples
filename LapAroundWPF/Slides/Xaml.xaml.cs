using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace LapAroundWPFTechEd2007.Slides
{
    /// <summary>
    /// Interaction logic for Xaml.xaml
    /// </summary>
    public partial class Xaml : Slide
    {
        string startDocument = @"<Grid 
  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <Button Content=""Hello, from XAML"" Width=""100"" Height=""25"" />
</Grid>";

        string linearGradientBrush = @"
    <Button.Background>        
       <LinearGradientBrush StartPoint=""0,0"" EndPoint=""0,1"">
           <GradientStop Color=""White"" />
           <GradientStop Color=""Blue"" Offset=""1"" />
       </LinearGradientBrush>
    </Button.Background>
    ";

        public Xaml()
        {
            InitializeComponent();
            editor.Text = startDocument;
            try
            {
                Clipboard.SetData(DataFormats.Text, linearGradientBrush);
            }
            catch (Exception ex)
            {
                // TODO - sometimes calling clipboard.setdata fails with a COM exception
            }

            InputGestureCollection updateUIGestures = new InputGestureCollection();
            updateUIGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Alt));
            RoutedUICommand updateUICommand = new RoutedUICommand("Update UI", "Update UI", GetType(), updateUIGestures);
            CommandBindings.Add(new CommandBinding(updateUICommand, new ExecutedRoutedEventHandler(this.UpdateFromXamlCode)));

        }

        private void UpdateFromXamlCode(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(editor.Text);
                object content = XamlReader.Load(new XmlNodeReader(doc));
                container.Content = content;
            }
            catch (XamlParseException xamEx)
            {

            }
            catch (Exception ex)
            {

            }
        }
    }
}
