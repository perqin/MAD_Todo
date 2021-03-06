﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MAD_Todo.Utils
{
    /// <summary>
    /// Boolean to Visibility converter
    /// </summary>
    /// <seealso cref="https://github.com/jamesmcroft/WinUX-UWP-Toolkit/blob/master/Croft.Core/Croft.Core.UWP/Xaml/Converters/BooleanToVisibilityConverter.cs"/>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a bool value to a Visibility value.
        /// </summary>
        /// <returns>
        /// Returns Visibility.Visible if true, else Visibility.Collapsed.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var b = value as bool?;
            return b == null ? Visibility.Collapsed : (b.Value ? Visibility.Visible : Visibility.Collapsed);
        }

        /// <summary>
        /// Converts a Visibility value to a bool value.
        /// </summary>
        /// <returns>
        /// Returns true if Visiblility.Visible, else false.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var v = value as Visibility?;
            return v == null ? (object)null : v.Value == Visibility.Visible;
        }
    }

    public class DateTimeToOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //TODO: FIXME: Convert make crash
            DateTime dt = (value as DateTime?) ?? DateTime.Today;
            DateTimeOffset dto = new DateTimeOffset(dt.Year, dt.Month, dt.Day, 0, 0, 0, new TimeSpan(8, 0, 0));
            return dto;
            //DateTime dt = (value as DateTime?) ?? DateTime.Today;
            //return (DateTimeOffset)dt;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //TODO: FIXME: Convert make crash
            DateTimeOffset dto = (value as DateTimeOffset?) ?? DateTimeOffset.Now;
            DateTime dt = dto.Date;
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            //return DateTime.Today;
            //DateTimeOffset dto = ((value as DateTimeOffset?) ?? DateTimeOffset.Now);
            //return dto.DateTime;
        }
    }

    //public class SelectedItemConverter : IValueConverter {
    //    public object Convert(object value, Type targetType, object parameter, string language) {
    //        int r = value as int? ?? -1;
    //        if (r == -1) return 0 else return 
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, string language) {
    //        return ((value as DateTimeOffset?) ?? DateTimeOffset.Now).DateTime;
    //    }
    //}
}
