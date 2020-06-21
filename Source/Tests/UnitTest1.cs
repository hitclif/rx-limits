using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ConsoleApp1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        private Subject<Data> _stream = new Subject<Data>();

        private readonly Data[] _data =
        {
            new Data("session-1", 1),

            new Data("session-2", 1),
            new Data("session-2", 2),

            new Data("session-1", 2),

            new Data("session-2", 3),
            new Data("session-2", 4),
            new Data("session-3", 1),
            new Data("session-2", 5),
            new Data("session-2", 6),
            new Data("session-2", 7),
            new Data("session-2", 8),
            new Data("session-2", 9),
        };

        [TestMethod]
        public void TestMethod1()
        {
            var lastValue = -1;
            var wasCompleted = false;

            var subscription = _stream
                .CompleteAfter("session-1", 3)
                .Subscribe(
                    d => { lastValue = d.Value; },
                    e => { },
                    () => {
                        wasCompleted = true;
                    });

            this.ExecuteStream();

            Assert.IsTrue(wasCompleted);
            Assert.AreEqual(2, lastValue);
        }

        [TestMethod]
        public void TestMethod2()
        {
            // session 1
            var s1_LastValue = -1;
            var s1_WasCompleted = false;
            _stream
                .CompleteAfter("session-1", 3)
                .Subscribe(
                    d => s1_LastValue = d.Value,
                    e => { },
                    () => s1_WasCompleted = true);

            // session 2
            var s2_LastValue = -1;
            var s2_WasCompleted = false;
            _stream
                .CompleteAfter("session-2", 2)
                .Subscribe(
                    d => s2_LastValue = d.Value,
                    e => { },
                    () => s2_WasCompleted = true);

            // session 3
            var s3_LastValue = -1;
            var s3_WasCompleted = false;
            _stream
                .CompleteAfter("session-3", 3)
                .Subscribe(
                    d => s3_LastValue = d.Value,
                    e => { },
                    () => s3_WasCompleted = true);

            this.ExecuteStream();

            Assert.IsTrue(s1_WasCompleted);
            Assert.AreEqual(2, s1_LastValue);
            Assert.IsFalse(s2_WasCompleted);
            Assert.AreEqual(9, s2_LastValue);
            Assert.IsTrue(s3_WasCompleted);
            Assert.AreEqual(1, s3_LastValue);
        }

        private void ExecuteStream()
        {
            foreach(var d in _data)
            {
                _stream.OnNext(d);
            }
        }
    }
}
