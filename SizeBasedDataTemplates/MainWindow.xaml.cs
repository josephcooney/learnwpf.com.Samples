using System.Windows;

namespace SizeBasedDataTemplates
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<DateTime> dateTimes;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            resetButton.Click += (o, s) => PopulateRandomData();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dateTimes = new ObservableCollection<DateTime>();
            dates.DataContext = dateTimes;
            PopulateRandomData();
        }

        private void PopulateRandomData()
        {
            var rnd = new Random(System.DateTime.Now.Millisecond);
            PopulateRandomDateTimes();
            var person = new Person { DateOfBirth = RandomDateTime(rnd), GivenName = RandomName(rnd), FamilyName = RandomName(rnd) + "son"};
            personContent.DataContext = person;
        }

        private void PopulateRandomDateTimes()
        {
            dateTimes.Clear();
            var rnd = new Random(System.DateTime.Now.Millisecond);
            int target = rnd.Next(0, 20);
            for (int counter = 0; counter < target; counter++)
            {
                dateTimes.Add(RandomDateTime(rnd));
            }
        }

        public string RandomName(Random rnd)
        {
            var index = rnd.Next(0, names.Length - 1);
            return names[index];
        }

        public DateTime RandomDateTime(Random rnd)
        {
            return new DateTime(rnd.Next(1800, 2100), rnd.Next(1,12), rnd.Next(1,28), rnd.Next(0,23), rnd.Next(0,59), rnd.Next(0,59));
        }

        private string[] names = new string[]{ "John", "Sally", "Mark", "David", "Andrew", "Paul", "Darren" };
    }

    public class Person
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
