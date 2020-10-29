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

namespace ErrorTemplate
{
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new Person();
        }
    }

    public class Person : IDataErrorInfo, INotifyPropertyChanged
    {
        private string firstName;
        private string lastName;

        public string FirstName
        {
            get { return firstName; }
            set 
            { 
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        public string LastName
        {
            get { return lastName; }
            set 
            { 
                lastName = value; 
                OnPropertyChanged("LastName");
            }
        }

        public string this[string columnName]
        {
            get { return "Test Error on field " + columnName ; }
        }

        public string Error
        {
            get { return null; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    } 
}
