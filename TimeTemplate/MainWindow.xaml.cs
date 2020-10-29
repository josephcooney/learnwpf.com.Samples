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

namespace TimeTemplate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<ChatEntry> conversation = new List<ChatEntry>();
            conversation.Add(new ChatEntry("hello, are you there Bill?", System.DateTime.Now - new TimeSpan(0, 25, 0), "Steve"));
            conversation.Add(new ChatEntry("Yes Steve, what is it?", System.DateTime.Now - new TimeSpan(0, 15, 20), "Bill"));
            conversation.Add(new ChatEntry("I want to double the salaries of all the developers  on the windows client team.", System.DateTime.Now - new TimeSpan(0, 12, 0), "Steve"));
            conversation.Add(new ChatEntry("Sure, those guys are doing a great job.", System.DateTime.Now - new TimeSpan(0, 6, 0), "Bill"));
            conversation.Add(new ChatEntry("That WPF thing they've built is totally awesome", System.DateTime.Now - new TimeSpan(0, 5, 0), "Bill"));
            this.DataContext = conversation;
        }
    }


    public class ChatEntry
    {
        public ChatEntry(string msg, DateTime chatTime, string messageSender)
        {
            Message = msg;
            DateTime = chatTime;
            Sender = messageSender;
        }

        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public string Sender { get; set; }
    }
}
