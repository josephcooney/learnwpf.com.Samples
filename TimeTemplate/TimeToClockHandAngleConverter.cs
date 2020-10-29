using System;
using System.Windows.Data;

namespace TimeTemplate
{
    [ValueConversion(typeof(DateTime), typeof(double))]
    public class TimeToClockHandAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            HandType hand = (HandType)Enum.Parse(typeof(HandType), parameter as string);
            if (value is DateTime)
            {
                DateTime timeValue = (DateTime)value;
                if (hand == HandType.HourHand)
                {
                    // convert from hours to degrees
                    return timeValue.Hour * 30 + timeValue.Minute * 0.5;
                }
                if (hand == HandType.MinuteHand)
                {
                    // convert from minutes to degrees
                    return timeValue.Minute * 6;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            // we don't intend this to ever be called
            return null;
        }

        private enum HandType
        {
            HourHand,
            MinuteHand
        }
    }
}
