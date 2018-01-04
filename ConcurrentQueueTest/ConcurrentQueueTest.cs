using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Sixt4.TestAssignment.Test
{
    [TestClass]
    public class ConcurrentQueueTest
    {
        [TestMethod]
        public void TestEnqueue()
        {
            TestEnqueueAsync();
        }

        private async void TestEnqueueAsync()
        {
            Sixt4.TestAssignment.ConcurrentQueue<int> list = new Sixt4.TestAssignment.ConcurrentQueue<int>();
            var tasks = new List<Task>();
            var range = Enumerable.Range(0, 10000);

            foreach (var number in range)
            {
                tasks.Add(Task.Factory.StartNew(() => list.Enqueue(number), TaskCreationOptions.PreferFairness));
            }

            await Task.WhenAll(tasks);

            Assert.IsTrue(range.Count<int>() == list.Count());
        }

        [TestMethod]
        public void TestEnqueueWithCollection()
        {
            TestEnqueueWithCollectionAsync();
        }

        private async void TestEnqueueWithCollectionAsync()
        {
            Sixt4.TestAssignment.ConcurrentQueue<int> list = new Sixt4.TestAssignment.ConcurrentQueue<int>();
            var tasks = new List<Task>();
            var range = Enumerable.Range(0, 10000);
            List<int> numbers = new List<int>();

            foreach (var number in range)
            {
                numbers.Add(number);
            }
            tasks.Add(Task.Factory.StartNew(() => list.Enqueue(numbers), TaskCreationOptions.PreferFairness));

            await Task.WhenAll(tasks);

            Assert.IsTrue(range.Count<int>() == list.Count());
        }

        [TestMethod]
        public void TestDequeue()
        {
            Sixt4.TestAssignment.ConcurrentQueue<int> list = new Sixt4.TestAssignment.ConcurrentQueue<int>();

            Task<int> taskDequeue = Task<int>.Run(() => list.Dequeue(1000));
            Task taskEnqueue = Task.Delay(500).ContinueWith(_ => list.Enqueue(2));
            taskDequeue.Wait();
            taskEnqueue.Wait();
            Assert.IsTrue(taskDequeue.Result == 2);
        }

        [TestMethod]
        public void TestDequeueTimeoutException()
        {
            Sixt4.TestAssignment.ConcurrentQueue<int> list = new Sixt4.TestAssignment.ConcurrentQueue<int>();
            bool returnValue = false;
            try
            {
                Task<int> taskDequeue = Task<int>.Run(() => list.Dequeue(2000));
                taskDequeue.Wait();
            }
            catch (Exception)
            {
                returnValue = true;
            }
            Assert.IsTrue(returnValue);
        }

        [TestMethod]
        public void TestDequeueReturn()
        {
            Sixt4.TestAssignment.ConcurrentQueue<int> list = new Sixt4.TestAssignment.ConcurrentQueue<int>();

            Task<int> taskDequeue = Task<int>.Run(() => list.Dequeue(5));
            Task taskEnqueue = Task<int>.Run(() => list.Enqueue(2));
            taskDequeue.Wait();
            taskEnqueue.Wait();
            Assert.IsTrue(taskDequeue.Result == 2);
        }

        [TestMethod]
        public void TestDequeueWithCollection()
        {
            TestDequeueWithCollectionAsync();
        }

        private async void TestDequeueWithCollectionAsync()
        {
            Sixt4.TestAssignment.ConcurrentQueue<int> list = new Sixt4.TestAssignment.ConcurrentQueue<int>();
            var tasks = new List<Task>();
            var range = Enumerable.Range(0, 10000);
            List<int> numbers = new List<int>();

            foreach (var number in range)
            {
                numbers.Add(number);
            }
            tasks.Add(Task.Factory.StartNew(() => list.Enqueue(numbers), TaskCreationOptions.PreferFairness));
            tasks.Add(Task<List<int>>.Factory.StartNew(() => list.DequeueAll(1000), TaskCreationOptions.PreferFairness));
            await Task.WhenAll(tasks);

            Assert.IsTrue(range.Count<int>() == (tasks[1] as Task<List<int>>).Result.Count());
        }
    }
}
