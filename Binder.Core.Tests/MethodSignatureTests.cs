using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Binder.Core.Tests
{
    [TestClass]
    public class MethodSignatureTests
    {
        [TestMethod]
        public void SimpleMethodsAreEquals()
        {
            var first = new MethodSignature("{0}", new[] { typeof(int) });
            var second = new MethodSignature("{0}", new[] { typeof(int) });
            Assert.IsTrue(first.Equals(second));
            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }

        [TestMethod]
        public void CoplexMethodsWithMultipleParametersAreEqual()
        {
            var first = new MethodSignature("{0} > 3 && {1} < 5", new[] { typeof(long), typeof(int) });
            var second = new MethodSignature("{0} > 3 && {1} < 5", new[] { typeof(long), typeof(int) });
            Assert.IsTrue(first.Equals(second));
            Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }

        [TestMethod]
        public void DifferentParamterTypesResultsInUniqueMethodSignature()
        {
            var first = new MethodSignature("{0}", new[] { typeof(long) });
            var second = new MethodSignature("{0}", new[] { typeof(int) });
            Assert.IsFalse(first.Equals(second));
            Assert.AreNotEqual(first.GetHashCode(), second.GetHashCode());
        }

        [TestMethod]
        public void OrderOfParamtersResultsInUniqueMethodSignature()
        {
            var first = new MethodSignature("{0} > {1}", new[] { typeof(long), typeof(int) });
            var second = new MethodSignature("{0} > {1}", new[] {typeof(int), typeof(long) });
            Assert.IsFalse(first.Equals(second));
            Assert.AreNotEqual(first.GetHashCode(), second.GetHashCode());
        }
    }
}
