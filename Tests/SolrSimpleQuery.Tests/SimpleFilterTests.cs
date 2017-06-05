using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrSimpleQuery.Filters;
using SolrSimpleQuery.Utility.Extensions;

namespace SolrSimpleQuery.Tests
{
    [TestClass]
    public class SimpleFilterTests
    {
        [TestMethod]
        public void SimpleFilterTest_string()
        {
            var filter = new SimpleFilter<string>().Create("test", "value");
            Assert.IsTrue(filter.FieldName == "test");
            Assert.IsTrue(filter.Value == "value");

            Assert.IsTrue(filter.ToString == "test%3avalue");
        }

        [TestMethod]
        public void SimpleFilterTest_Datetime()
        {
            var filter = new SimpleFilter<DateTime>().Create("test", TypeExt.CreateDateTime(2017, 1, 1));
            Assert.IsTrue(filter.FieldName == "test");
            Assert.IsTrue(filter.Value == new DateTime(2017, 1, 1));

            Assert.IsTrue(filter.ToString == "test%3a2017-01-01T12:00:00Z");
        }

        [TestMethod]
        public void SimpleFilterTest_bool()
        {
            var filter = new SimpleFilter<bool>().Create("test", true);
            Assert.IsTrue(filter.FieldName == "test");
            Assert.IsTrue(filter.Value);

            Assert.IsTrue(filter.ToString == "test%3aTrue");
        }

        [TestMethod]
        public void SetFieldNameTest()
        {
            var filter = new SimpleFilter<bool>().Create("test", true);
            filter.SetFieldName("test2");

            Assert.IsTrue(filter.FieldName == "test2");
        }

        [TestMethod]
        public void SetValueTest()
        {
            var filter = new SimpleFilter<bool>().Create("test", true);
            filter.SetValue(false);

            Assert.IsTrue(filter.Value == false);
        }
    }
}