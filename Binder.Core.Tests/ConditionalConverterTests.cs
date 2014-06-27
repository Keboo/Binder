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
            result = converter.Convert(new object[] { 4 }, null, conditionFormat, null);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void SimpleInequalityComparisionWithTypeConversion()
        {
            const string conditionFormat = "{0} > 3.0";
            var converter = new ConditionalConverter();
            object result = converter.Convert(new object[] { 5 }, null, conditionFormat, null);
            Assert.AreEqual(true, result);
            result = converter.Convert(new object[] { 0 }, null, conditionFormat, null);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ObjectComparisionWithNull()
        {
            const string conditionFormat = "{0} != null";
            var converter = new ConditionalConverter();
            object result = converter.Convert(new[] { new object() }, null, conditionFormat, null);
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void MultipleBooleanParameters()
        {
            const string conditionFormat = "({0} && {1}) || {2}";
            var converter = new ConditionalConverter();
            object result = converter.Convert(new object[] { true, true, false }, null, conditionFormat, null);
            Assert.AreEqual(true, result);
            result = converter.Convert(new object[] { true, false, true }, null, conditionFormat, null);
            Assert.AreEqual(true, result);
            result = converter.Convert(new object[] { true, false, false }, null, conditionFormat, null);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ObjectComparision()
        {
            const string conditionFormat = "{0} == {1}";
            var converter = new ConditionalConverter();
            object result = converter.Convert(new[] { new object(), new object() }, null, conditionFormat, null);
            Assert.AreEqual(false, result);
            object obj = new object();
            result = converter.Convert(new[] { obj, obj }, null, conditionFormat, null);
            Assert.AreEqual(true, result);
        }
    }
}
