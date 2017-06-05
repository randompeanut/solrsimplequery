using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrSimpleQuery.Filters;
using SolrSimpleQuery.Utility.Extensions;

namespace SolrSimpleQuery.Tests
{
    [TestClass]
    public class RangeFilterTests
    {
        [TestMethod]
        public void CreateFromTest_int()
        {
            var rangeFilter = new RangeFilter<int>().CreateFrom("test", 100);

            Assert.IsTrue(rangeFilter.FieldName == "test");
            Assert.IsTrue(rangeFilter.FromIsSpecified);
            Assert.IsTrue(rangeFilter.FromValue == 100);

            Assert.IsTrue(rangeFilter.ToString == "test%3a[100 TO *]");
        }

        [TestMethod]
        public void CreateFromTest_DateTime()
        {
            var rangeFilter = new RangeFilter<DateTime>().CreateFrom("test", TypeExt.CreateDateTime(2017, 1, 1));

            Assert.IsTrue(rangeFilter.FieldName == "test");
            Assert.IsTrue(rangeFilter.FromIsSpecified);
            Assert.IsTrue(rangeFilter.FromValue == new DateTime(2017, 1, 1));

            Assert.IsTrue(rangeFilter.ToString == "test%3a[2017-01-01T12:00:00Z TO *]");
        }

        [TestMethod]
        public void CreateToTest_int()
        {
            var rangeFilter = new RangeFilter<int>().CreateTo("test", 100);

            Assert.IsTrue(rangeFilter.FieldName == "test");
            Assert.IsTrue(rangeFilter.ToIsSpecified);
            Assert.IsTrue(rangeFilter.ToValue == 100);

            Assert.IsTrue(rangeFilter.ToString == "test%3a[* TO 100]");
        }

        [TestMethod]
        public void CreateToTest_DateTime()
        {
            var rangeFilter = new RangeFilter<DateTime>().CreateTo("test", TypeExt.CreateDateTime(2017, 1, 1));

            Assert.IsTrue(rangeFilter.FieldName == "test");
            Assert.IsTrue(rangeFilter.ToIsSpecified);
            Assert.IsTrue(rangeFilter.ToValue == new DateTime(2017, 1, 1));

            Assert.IsTrue(rangeFilter.ToString == "test%3a[* TO 2017-01-01T12:00:00Z]");
        }

        [TestMethod]
        public void CreateTest_int()
        {
            var rangeFilter = new RangeFilter<int>().Create("test", 100, 200);

            Assert.IsTrue(rangeFilter.FieldName == "test");
            Assert.IsTrue(rangeFilter.FromIsSpecified);
            Assert.IsTrue(rangeFilter.FromValue == 100);
            Assert.IsTrue(rangeFilter.ToIsSpecified);
            Assert.IsTrue(rangeFilter.ToValue == 200);

            Assert.IsTrue(rangeFilter.ToString == "test%3a[100 TO 200]");
        }

        [TestMethod]
        public void CreateTest_DateTime()
        {
            var rangeFilter = new RangeFilter<DateTime>().Create("test", TypeExt.CreateDateTime(2017, 1, 1),
                TypeExt.CreateDateTime(2017, 12, 1));

            Assert.IsTrue(rangeFilter.FieldName == "test");
            Assert.IsTrue(rangeFilter.FromIsSpecified);
            Assert.IsTrue(rangeFilter.FromValue == new DateTime(2017, 1, 1));
            Assert.IsTrue(rangeFilter.ToIsSpecified);
            Assert.IsTrue(rangeFilter.ToValue == new DateTime(2017, 12, 1));

            Assert.IsTrue(rangeFilter.ToString == "test%3a[2017-01-01T12:00:00Z TO 2017-12-01T12:00:00Z]");
        }

        [TestMethod]
        public void SetFieldNameTest()
        {
            var rangeFilter = new RangeFilter<int>().Create("test", 100, 200);
            rangeFilter.SetFieldName("test2");

            Assert.IsTrue(rangeFilter.FieldName == "test2");
        }

        [TestMethod]
        public void SetFromValueTest()
        {
            var rangeFilter = new RangeFilter<int>().Create("test", 100, 200);
            rangeFilter.SetFromValue(105);

            Assert.IsTrue(rangeFilter.FromValue == 105);
        }

        [TestMethod]
        public void SetToValueTest()
        {
            var rangeFilter = new RangeFilter<int>().Create("test", 100, 200);
            rangeFilter.SetToValue(205);

            Assert.IsTrue(rangeFilter.ToValue == 205);
        }
    }
}