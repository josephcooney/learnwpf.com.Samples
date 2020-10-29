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
    /// Interaction logic for XPS.xaml
    /// </summary>
    public partial class XPS : Slide
    {
        private string xpsText = @"
// some extra XPS namespaces
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;

using (XpsDocument doc 
    = new XpsDocument(path, System.IO.FileAccess.ReadWrite))
{
    XpsPackagingPolicy packagePolicy = 
        new XpsPackagingPolicy(doc);
    XpsSerializationManager serializationMgr = 
        new XpsSerializationManager(packagePolicy, false);

    // itemToPrint can be any UI element
    serializationMgr.SaveAsXaml(itemToPrint); 
}
        ";

        public XPS()
        {
            InitializeComponent();
            editor.Text = xpsText;
        }
    }
}
