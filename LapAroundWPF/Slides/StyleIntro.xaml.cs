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
    /// Interaction logic for StyleIntro.xaml
    /// </summary>
    public partial class StyleIntro : Slide
    {

        string startDocument = @"<StackPanel HorizontalAlignment=""Center"" VerticalAlignment=""Center""
  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" >
    <StackPanel.Resources>
        <Style TargetType=""{x:Type TextBlock}"">
            <Setter Property=""Foreground"" Value=""White"" />
            <Setter Property=""FontSize"" Value=""30"" />
        </Style>
    </StackPanel.Resources>
    <TextBlock Text=""Like CSS, but with angle brackets"" />
    <TextBlock Text=""Set properties on WPF elements"" />
    <TextBlock Text=""&lt;Style x:Key='some name' TargetType='{x:Type SomeType}'&gt;
        &lt;Setter Property='Property Name' Value='Value' /&gt;
    "" />
</StackPanel>";

        string newStyle = @"
        <Style x:Key=""myCode"" TargetType=""{x:Type TextBlock}"">
            <Setter Property=""FontFamily"" Value=""Consolas"" />
        </Style>";

        public StyleIntro()
        {
            InitializeComponent();
            editor.Text = startDocument;
            
            InputGestureCollection updateUIGestures = new InputGestureCollection();
            updateUIGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Alt));
            RoutedUICommand updateUICommand = new RoutedUICommand("Update UI", "Update UI", GetType(), updateUIGestures);
            CommandBindings.Add(new CommandBinding(updateUICommand, new ExecutedRoutedEventHandler(this.ParseText)));

            ParseText(null, null);
            Clipboard.SetText(newStyle);
        }

        private void ParseText(object sender, RoutedEventArgs e)
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
