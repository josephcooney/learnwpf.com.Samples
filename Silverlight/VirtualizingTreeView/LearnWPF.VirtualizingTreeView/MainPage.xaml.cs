using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LearnWPF.VirtualizingTreeView
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            // build an arbitaray tree
            var root = new Employee("Steven J Ballmer");
            root.DirectReports.Add(new Employee("Steven Sinofsky"));
            root.DirectReports.Add(new Employee("Bob Muglia"));
            root.DirectReports.Add(new Employee("Stephen Elop"));
            for (int counter = 0; counter < 1000; counter ++)
            {
                root.DirectReports[0].DirectReports.Add(new Employee("Generic Windows SDE " + counter.ToString()));
            }
            var theGu = new Employee("Scott Guthrie");
            root.DirectReports[1].DirectReports.Add(theGu);
            for (int counter = 0; counter < 100; counter ++)
            {
                theGu.DirectReports.Add(new Employee("Generic devdiv SDE " + counter.ToString()));
            }

            this.DataContext = new TreeViewModel<Employee>(root, (Employee e) => e.DirectReports);
        }
    }

    public class Employee
    {
        public Employee(string name)
        {
            Name = name; DirectReports = new List<Employee>();
        }
        public string Name { get; private set; }
        public List<Employee> DirectReports { get; private set; }
    }
}
