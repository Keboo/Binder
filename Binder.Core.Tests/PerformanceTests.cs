using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Binder.Core.Tests
{
    [TestClass]
    public class PerformanceTests : TimingTestBase
    {
        [TestMethod]
        public void PerformanceComparisonWithIntConverter()
        {
            object[] values = { 1, 2 };

            var intConverter = new IntComparisionValueConverter();
            TimeSpan intTiming;
            object intResult = GetTiming(() => intConverter.Convert(values, typeof(bool), null, null), out intTiming);

            var conditionalConverter = new ConditionalMultiValueConverter();
            TimeSpan conditionalTiming1;
            TimeSpan conditionalTiming2;
            object conditionalResult1 = GetTiming(() => conditionalConverter.Convert(values, typeof(bool), "{0} > {1}", null), out conditionalTiming1);
            object conditionalResult2 = GetTiming(() => conditionalConverter.Convert(values, typeof(bool), "{0} > {1}", null), out conditionalTiming2);

            Assert.AreEqual(intResult, conditionalResult1);
            Assert.AreEqual(intResult, conditionalResult2);
            Assert.IsTrue(conditionalTiming1 < TimeSpan.FromTicks(intTiming.Ticks * 120));
            Assert.IsTrue(conditionalTiming2 < TimeSpan.FromTicks(intTiming.Ticks * 12));
        }

        private class IntComparisionValueConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (values != null && values.Length >= 2)
                    return System.Convert.ToInt32(values[0]) > System.Convert.ToInt32(values[1]);
                return DependencyProperty.UnsetValue;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
