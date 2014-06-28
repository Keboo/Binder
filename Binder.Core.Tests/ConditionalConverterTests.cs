using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;

namespace Binder.Core.Tests
{
    [TestClass]
    public class ConditionalConverterTests
    {
        [TestMethod]
        public void CanConvertResultingType()
        {
            const string conditionFormat = "{0} > 5 ? \"Red\" : \"Blue\"";
            var converter = new ConditionalConverter();
            object result1 = converter.Convert(new object[] { 10 }, typeof(Brush), conditionFormat, null);
            var redBrush = result1 as SolidColorBrush;
            Assert.IsNotNull(redBrush);
            Assert.AreEqual(Colors.Red, redBrush.Color);
            object result2 = converter.Convert(new object[] { 0 }, typeof(Brush), conditionFormat, null);
            var blueBrush = result2 as SolidColorBrush;
            Assert.IsNotNull(blueBrush);
            Assert.AreEqual(Colors.Blue, blueBrush.Color);
        }
    }
}
