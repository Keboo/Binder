using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Binder.Core.Tests
{
    [TestClass]
    public class MethodManagerTests
    {
        [TestMethod]
        public void SimpleInequalityComparision()
        {
            const string conditionFormat = "{0} > 3";
            object result = ShowTiming("Inequality comparision", () => MethodManager.RunMethod(conditionFormat, new object[] { 1 }));
            Assert.AreEqual(false, result);
            result = ShowTiming("Inequality comparison, cached", () => MethodManager.RunMethod(conditionFormat, new object[] { 4 }));
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void SimpleInequalityComparisionWithTypeConversion()
        {
            const string conditionFormat = "{0} > 3.0";
            object result = ShowTiming("Inequality, type conversion", () => MethodManager.RunMethod(conditionFormat, new object[] { 5 }));
            Assert.AreEqual(true, result);
            result = ShowTiming("Inequality, type conversion cached", () => MethodManager.RunMethod(conditionFormat, new object[] { 0 }));
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void ObjectComparisionWithNull()
        {
            const string conditionFormat = "{0} != null";
            object result = ShowTiming("Object comparison", () => MethodManager.RunMethod(conditionFormat, new[] { new object() }));
            Assert.AreEqual(true, result);
            result = ShowTiming("Object comparison, cached", () => MethodManager.RunMethod(conditionFormat, new object[] { null }));
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void MultipleBooleanParameters()
        {
            const string conditionFormat = "({0} && {1}) || {2}";
            object result = ShowTiming("MultipleBooleanParameters", () => MethodManager.RunMethod(conditionFormat, new object[] { true, true, false }));
            Assert.AreEqual(true, result);
            result = ShowTiming("MultipleBooleanParameters, cached", () => MethodManager.RunMethod(conditionFormat, new object[] { true, false, true }));
            Assert.AreEqual(true, result);
            result = ShowTiming("MultipleBooleanParameters, cached", () => MethodManager.RunMethod(conditionFormat, new object[] { true, false, false }));
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void AlternateBooleanOperatorsAllowed()
        {
            const string conditionFormat = "({0} and {1}) or {2}";
            object result = ShowTiming("AlternateBooleanOperatorsAllowed", () => MethodManager.RunMethod(conditionFormat, new object[] { true, true, false }));
            Assert.AreEqual(true, result);
            result = ShowTiming("AlternateBooleanOperatorsAllowed, cached", () => MethodManager.RunMethod(conditionFormat, new object[] { true, false, true }));
            Assert.AreEqual(true, result);
            result = ShowTiming("AlternateBooleanOperatorsAllowed, cached", () => MethodManager.RunMethod(conditionFormat, new object[] { true, false, false }));
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void CanConvertStringToInt32()
        {
            const string conditionFormat = "Convert.ToInt32({0})";
            object result = ShowTiming("TypeConversionsAllowed", () => MethodManager.RunMethod(conditionFormat, new object[] { "1" }));
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void CanConvertStringToInt16()
        {
            const string conditionFormat = "Convert.ToInt16({0})";
            object result = ShowTiming("TypeConversionsAllowed", () => MethodManager.RunMethod(conditionFormat, new object[] { "1" }));
            Assert.AreEqual((short)1, result);
        }

        [TestMethod]
        public void ObjectComparision()
        {
            const string conditionFormat = "{0} == {1}";
            object result = ShowTiming("Object comparison", () => MethodManager.RunMethod(conditionFormat, new[] { new object(), new object() }));
            Assert.AreEqual(false, result);
            var obj = new object();
            result = ShowTiming("Object comparision, cached", () => MethodManager.RunMethod(conditionFormat, new[] { obj, obj }));
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void DifferentSignaturesWork()
        {
            const string conditionFormat = "0 < {0}";
            object result1 = MethodManager.RunMethod(conditionFormat, new object[] { 1 });
            Assert.AreEqual(true, result1);
            object result2 = MethodManager.RunMethod(conditionFormat, new object[] { 1L });
            Assert.AreEqual(true, result2);
        }

        [TestMethod]
        public void CanCallMethodsOnParameterObjects()
        {
            const string conditionFormat = "{0}.StartsWith(\"a\")";
            object result1 = MethodManager.RunMethod(conditionFormat, new object[] { "apple" });
            Assert.AreEqual(true, result1);
            object result2 = MethodManager.RunMethod(conditionFormat, new object[] { "fruit" });
            Assert.AreEqual(false, result2);
        }

        [TestMethod]
        public void RunningMethodCachesMethod()
        {
            const string conditionFormat = "{0} > 0";

            Assert.IsFalse(MethodManager.IsCached(conditionFormat, new[] { typeof(int) }));
            object result1 = MethodManager.RunMethod(conditionFormat, new object[] { 1 });
            Assert.AreEqual(true, result1);
            Assert.IsTrue(MethodManager.IsCached(conditionFormat, new[] { typeof(int) }));
        }

        [TestMethod]
        public void Test()
        {
            var result = MethodManager.RunMethod("new (@1 = 1)", new object[0]);
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