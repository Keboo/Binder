using System;
using System.Diagnostics;

namespace Binder.Core.Tests
{
    public abstract class TimingTestBase
    {
        protected static T ShowTiming<T>(string title, Func<T> method)
        {
            TimeSpan timing;
            var rv = GetTiming(method, out timing);
            Debug.WriteLine("{0}, {1}", title, timing);
            return rv;
        }

        protected static T GetTiming<T>(Func<T> method, out TimeSpan timing)
        {
            var sw = Stopwatch.StartNew();
            var rv = method();
            sw.Stop();
            timing = sw.Elapsed;
            return rv;
        }
    }
}