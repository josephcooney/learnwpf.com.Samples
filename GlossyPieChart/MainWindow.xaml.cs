using System.ComponentModel;
using System.Windows;

namespace LearnWPF.GlossyPieChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new TestViewModel() {Value = 10};
        }
    }

    public class TestViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int value;
        public  int Value
        {
            get { return value; }
            set { this.value = value; OnPropertyChanged("Value"); }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
