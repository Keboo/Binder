using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Binder.Core.Tests
{
    [TestClass]
    public class ConditionalConverterTests
    {
        [TestMethod]
        public void SimpleInequalityComparision()
        {
            const string conditionFormat = "{0} > 3";
            var converter = new ConditionalConverter();
            object result = converter.Convert(new object[] {1}, null, conditionFormat, null);
            Assert.AreEqual(false, result);
        }
    }
}
