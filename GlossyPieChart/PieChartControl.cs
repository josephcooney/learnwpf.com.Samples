using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LearnWPF.GlossyPieChart
{
    public class PieChartControl : Control
    {
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(ObservableCollection<PieValue>), typeof(PieChartControl), new PropertyMetadata(default(ObservableCollection<PieValue>)));

        public PieChartControl()
        {
            Values = new ObservableCollection<PieValue>();
        }

        public ObservableCollection<PieValue> Values
        {
            get { return (ObservableCollection<PieValue>)GetValue(ValuesProperty); }
            set
            {
                var oldValue = this.Values;
                if (oldValue != null)
                {
                    oldValue.CollectionChanged -= value_CollectionChanged;
                }
                SetValue(ValuesProperty, value); 
                if (value != null)
                {
                    value.CollectionChanged += value_CollectionChanged; 
                }
                
            }
        }

        void value_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    var pieItem = (PieValue)item;
                    pieItem.PropertyChanged -= PieItemChanged;
                }                
            }

            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (var item in e.NewItems)
                {
                    var pieItem = (PieValue) item;
                    pieItem.PropertyChanged += PieItemChanged;
                }
            }
        }

        void PieItemChanged(object sender, PropertyChangedEventArgs e)
        {
            InvalidateVisual();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var min = Math.Min(constraint.Width, constraint.Height);
            return new Size(min, min);
        }

        protected override void OnRender(DrawingContext dc)
        {
            // calculate the size of each PieValue
            var total = Values.Sum(a => a.Value);
            var radius = this.DesiredSize.Width/2;
            var center = new Point(radius, radius);
            var currentPoint = PointFromAngle(0, radius);
            var arcSize = new Size(radius, radius);
            var pen = new Pen(Brushes.Black, 2);
            float totalDegrees = 0;
            foreach (var pieValue in Values)
            {
                float percentage = (float) pieValue.Value/(float) total;
                var degrees = 360*percentage;
                totalDegrees += degrees;

                var nextPoint = PointFromAngle(totalDegrees, radius);

                var isLarge = percentage > 0.5;
                var geom = new PathGeometry();
                var startPoint = center;
                var fig = new PathFigure() {StartPoint = startPoint};
                fig.Segments.Add(new LineSegment(currentPoint, false));
                fig.Segments.Add(new ArcSegment(nextPoint, arcSize, degrees, isLarge, SweepDirection.Clockwise, true));
                fig.Segments.Add(new LineSegment(startPoint, false));
                geom.Figures.Add(fig);
                dc.PushClip(geom);
                dc.DrawEllipse(pieValue.Fill, pen, center, radius, radius);
                dc.Pop();
                currentPoint = nextPoint;
            }

        }

        private double ToRadians(float degrees)
        {
            return (degrees*Math.PI)/180;
        }

        private Point PointFromAngle(float angleInDegrees, double radius)
        {
            var radians = ToRadians(angleInDegrees);
            var result = new Point(radius*Math.Cos(radians) + radius, radius*Math.Sin(radians) + radius);
            return result;
        }
    }

    public class PieValue : DependencyObject, INotifyPropertyChanged    
    {
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof (string), typeof (PieValue), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("fill", typeof (Brush), typeof (PieValue), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof (int), typeof (PieValue), new PropertyMetadata(default(int)));

        public int Value
        {
            get { return (int) GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
                OnPropertyChanged("Value");
            }
        }   
        
        public string Key
        {
            get { return (string) GetValue(KeyProperty); }
            set { 
                SetValue(KeyProperty, value);
                OnPropertyChanged("Key");
            }
        }   

        public Brush Fill
        {
            get { return (Brush) GetValue(FillProperty); }
            set { 
                SetValue(FillProperty, value); 
                OnPropertyChanged("Fill");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
