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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Kaleidoscope
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatcherTimer pulseTimer;
        private Random rnd = new Random(System.DateTime.Now.Millisecond);

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var rotatingVisual = this.FindResource("rotatingVisual") as Border;
            var brush = rotatingVisual.Background as VisualBrush;
            var transGroup = brush.RelativeTransform as TransformGroup;
            var patternRotate = transGroup.Children[0];
            patternRotate.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation(0, 270, new Duration(new TimeSpan(0, 0, 30))) { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever, AccelerationRatio = 0.25, DecelerationRatio = 0.25 });
            patternRotate.BeginAnimation(RotateTransform.CenterXProperty, new DoubleAnimation(0.4, 0.6, new Duration(new TimeSpan(0, 0, 22))) { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever, AccelerationRatio = 0.25, DecelerationRatio = 0.25});
            patternRotate.BeginAnimation(RotateTransform.CenterYProperty, new DoubleAnimation(0.4, 0.6, new Duration(new TimeSpan(0, 0, 12))) { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever, AccelerationRatio = 0.25, DecelerationRatio = 0.25 });

            var pen = this.FindResource("penColor") as SolidColorBrush;
            pen.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(Colors.White, Colors.DarkGray, new Duration(new TimeSpan(0, 0, 30))) { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever });
            var foreground = this.FindResource("foregroundColor") as SolidColorBrush;
            foreground.BeginAnimation(SolidColorBrush.ColorProperty, new ColorAnimation(Colors.White, Color.FromArgb(12,2,2,2), new Duration(new TimeSpan(0, 0, 21))) { DecelerationRatio = 0.3, AccelerationRatio = 0.3, AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever });

            var patternVisual = this.FindResource("patternVisual") as Border;
            var sizeAnimation = new DoubleAnimation(400, 1000, new Duration(new TimeSpan(0, 1, 32))) { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever, AccelerationRatio = 0.25, DecelerationRatio = 0.25 };
            patternVisual.BeginAnimation(Border.WidthProperty, sizeAnimation);
            patternVisual.BeginAnimation(Border.HeightProperty, sizeAnimation);

            var bg = this.FindResource("damaskImage") as Image;
            if (bg != null)
            {
                bg.BeginAnimation(Image.OpacityProperty, new DoubleAnimation(0, 0.5, new Duration(new TimeSpan(0, 1, 0))) { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever });
            }

            pulseTimer = new DispatcherTimer(DispatcherPriority.Normal) {Interval = new TimeSpan(0, 0, 5)};
            pulseTimer.Tick += new EventHandler(pulseTimer_Tick);
            pulseTimer.IsEnabled = true;
        }

        private bool pulseWasGenerated = false;
        void pulseTimer_Tick(object sender, EventArgs e)
        {
            var chance = rnd.Next(0, 15);
            if (pulseWasGenerated)
            {
                chance --;
            }
            if (chance < 3)
            {
                pulseWasGenerated = true;
                pulseEllipse.StrokeThickness = rnd.Next(5, 50);

                var duration = new Duration(new TimeSpan(TimeSpan.TicksPerSecond * rnd.Next(0,5) + rnd.Next(500, 100000)));
                pulseEllipse.BeginAnimation(Ellipse.WidthProperty, new DoubleAnimation(0,3000, duration){AccelerationRatio = 0.30});
                pulseEllipse.BeginAnimation(Ellipse.HeightProperty, new DoubleAnimation(0, 3000, duration) { AccelerationRatio = 0.30 });
            }
        }
    }
}
