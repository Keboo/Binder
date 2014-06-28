using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
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
            object result = ShowTiming("Inequality comparision", () => converter.Convert(new object[] {1}, null, conditionFormat, null));
            Assert.AreEqual(false, result);
            result = ShowTiming("Inequality comparison, cached", () => converter.Convert(new object[] { 4 }, null, conditionFormat, null));
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void SimpleInequalityComparisionWithTypeConversion()
        {
            const string conditionFormat = "{0} > 3.0";
            var converter = new ConditionalConverter();
            object result = ShowTiming("Inequality, type conversion", () => converter.Convert(new object[] { 5 }, null, conditionFormat, null));
            Assert.AreEqual(true, result);
            result = ShowTiming("Inequality, type conversion cached", () => converter.Convert(new object[] { 0 }, null, conditionFormat, null));
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ObjectComparisionWithNull()
        {
            const string conditionFormat = "{0} != null";
            var converter = new ConditionalConverter();
            object result = ShowTiming("Object comparison", () => converter.Convert(new[] { new object() }, null, conditionFormat, null));
            Assert.AreEqual(true, result);
            result = ShowTiming("Object comparison, cached", () => converter.Convert(new object[] { null }, null, conditionFormat, null));
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void MultipleBooleanParameters()
        {
            const string conditionFormat = "({0} && {1}) || {2}";
            var converter = new ConditionalConverter();
            object result = ShowTiming("MultipleBooleanParameters", () => converter.Convert(new object[] { true, true, false }, null, conditionFormat, null));
            Assert.AreEqual(true, result);
            result = ShowTiming("MultipleBooleanParameters, cached", () => converter.Convert(new object[] { true, false, true }, null, conditionFormat, null));
            Assert.AreEqual(true, result);
            result = ShowTiming("MultipleBooleanParameters, cached", () => converter.Convert(new object[] { true, false, false }, null, conditionFormat, null));
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void AlternateBooleanOperatorsAllowed()
        {
            const string conditionFormat = "({0} and {1}) or {2}";
            var converter = new ConditionalConverter();
            object result = ShowTiming("AlternateBooleanOperatorsAllowed", () => converter.Convert(new object[] { true, true, false }, null, conditionFormat, null));
            Assert.AreEqual(true, result);
            result = ShowTiming("AlternateBooleanOperatorsAllowed, cached", () => converter.Convert(new object[] { true, false, true }, null, conditionFormat, null));
            Assert.AreEqual(true, result);
            result = ShowTiming("AlternateBooleanOperatorsAllowed, cached", () => converter.Convert(new object[] { true, false, false }, null, conditionFormat, null));
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void CanConvertStringToInt32()
        {
            const string conditionFormat = "Convert.ToInt32({0})";
            var converter = new ConditionalConverter();
            object result = ShowTiming("TypeConversionsAllowed", () => converter.Convert(new object[] { "1" }, null, conditionFormat, null));
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void CanConvertStringToInt16()
        {
            const string conditionFormat = "Convert.ToInt16({0})";
            var converter = new ConditionalConverter();
            object result = ShowTiming("TypeConversionsAllowed", () => converter.Convert(new object[] { "1" }, null, conditionFormat, null));
            Assert.AreEqual((short)1, result);
        }

        [TestMethod]
        public void ObjectComparision()
        {
            const string conditionFormat = "{0} == {1}";
            var converter = new ConditionalConverter();
            object result = ShowTiming("Object comparison", () => converter.Convert(new[] { new object(), new object() }, null, conditionFormat, null));
            Assert.AreEqual(false, result);
            var obj = new object();
            result = ShowTiming("Object comparision, cached", () => converter.Convert(new[] { obj, obj }, null, conditionFormat, null));
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void DifferentSignaturesWork()
        {
            const string conditionFormat = "0 < {0}";
            var converter = new ConditionalConverter();
            object result1 = converter.Convert(new object[] {1}, null, conditionFormat, null);
            Assert.AreEqual(true, result1);
            object result2 = converter.Convert(new object[] {1L}, null, conditionFormat, null);
            Assert.AreEqual(true, result2);
        }

        [TestMethod]
        public void CanCallMethodsOnParameterObjects()
        {
            const string conditionFormat = "{0}.StartsWith(\"a\")";
            var converter = new ConditionalConverter();
            object result1 = converter.Convert(new object[] { "apple" }, null, conditionFormat, null);
            Assert.AreEqual(true, result1);
            object result2 = converter.Convert(new object[] { "fruit" }, null, conditionFormat, null);
            Assert.AreEqual(false, result2);
        }

        [TestMethod]
        public void CanConvertResultingType()
        {
            const string conditionFormat = "{0} > 5 ? \"Red\" : \"Blue\"";
            var converter = new ConditionalConverter();
            object result1 = converter.Convert(new object[] { 10 }, typeof(Brush), conditionFormat, null);
            Assert.AreEqual(new SolidColorBrush(Colors.Red), result1);
            object result2 = converter.Convert(new object[] { 0 }, typeof(Brush), conditionFormat, null);
            Assert.AreEqual(new SolidColorBrush(Colors.Blue), result2);
        }


        private T ShowTiming<T>(string title, Func<T> method)
        {
            var sw = Stopwatch.StartNew();
            var rv = method();
            sw.Stop();
            Debug.WriteLine("{0}, {1}", title, sw.Elapsed);
            return rv;
        }
    }
}
