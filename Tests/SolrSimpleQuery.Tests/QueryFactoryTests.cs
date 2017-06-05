using System;
using System.Globalization;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Omu.ValueInjecter;
using SolrSimpleQuery.Models;
using SolrSimpleQuery.Utility.Extensions;

namespace SolrSimpleQuery.Tests
{
    [TestClass]
    public class QueryFactoryTests
    {
        private const string SolrSimpleQueryBaseUrl = "http://www.int.dev.funda.nl:8080/indexer";
        private const string SolrSimpleQueryChannel = "fib";
        private const long GlobalId = 3522621;

        private readonly FilterCriteria _filterCriteria = new FilterCriteria
        {
            BaseUrl = SolrSimpleQueryBaseUrl,
            Channel = SolrSimpleQueryChannel,
            IdentifierFieldName = "GlobalId",
            FieldList = new[]
            {
                "GlobalId",
                "BronCode",
                "ZoekType",
                "IsKoop",
                "PublicatieDatum"
            },
            //FilterList = new IFilter[]
            //{
            //    new SimpleFilter<string>().Create("BronCode", "NVM"),
            //    new SimpleFilter<bool>().Create("IsKoop", true),
            //    new RangeFilter<int>().CreateFrom("KoopPrijs", 125000),
            //    new RangeFilter<DateTime>().CreateTo("PublicatieDatum", TypeExt.CreateDateTime(2017, 2, 1))
            //},
            UrlFilterList = new[]
            {
                "BronCode-NVM",
                "IsKoop-true",
                "KoopPrijs-125000+",
                "PublicatieDatum-+" + TypeExt.CreateDateTime(2017, 2, 1).ToString(CultureInfo.InvariantCulture)
            },
            Rows = 5,
            Start = 0,
            SortFieldName = "GlobalId",
            SortBy = "desc"
        };

        [TestMethod]
        public void QueryStaticTest()
        {
            var result = QueryFactory.Instance.Query<QueryResultObject>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);
        }

        [TestMethod]
        public void QueryStaticDefaultBaseUrlTest_success()
        {
            QueryFactory.SetBaseUrl(SolrSimpleQueryBaseUrl);

            var result = new QueryFactory().Query<QueryResultObject>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);
        }

        [TestMethod]
        public void QueryStaticDefaultChannelTest_success()
        {
            QueryFactory.SetChannel(SolrSimpleQueryChannel);

            var result = new QueryFactory().Query<QueryResultObject>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);
        }

        /// <summary>
        ///     This should fail because no baseurl is provided and the static instance is not set
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void QueryStaticDefaultBaseUrlTest_fail()
        {
            QueryFactory.ResetInstance();

            var noBaseUrlCriteria = new FilterCriteria();
            noBaseUrlCriteria.InjectFrom(_filterCriteria);
            noBaseUrlCriteria.BaseUrl = null;

            new QueryFactory().Query<QueryResultObject>(noBaseUrlCriteria);
        }

        /// <summary>
        ///     This should fail because no channel is provided and the static instance is not set
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(WebException))]
        public void QueryStaticDefaultChannelTest_fail()
        {
            QueryFactory.ResetInstance();

            var noChannelCriteria = new FilterCriteria();
            noChannelCriteria.InjectFrom(_filterCriteria);
            noChannelCriteria.Channel = null;

            new QueryFactory().Query<QueryResultObject>(noChannelCriteria);
        }

        [TestMethod]
        public void QueryGroupedStaticTest()
        {
            var result = QueryFactory.Instance.QueryGrouped<long, QueryResultObject>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);

            var fieldValueString = result.GetFieldValueByIdentifier(GlobalId, "BronCode");
            var fieldValueBool = result.GetFieldValueByIdentifier(GlobalId, "IsKoop");
            var fieldValueDate = result.GetFieldValueByIdentifier(GlobalId, "PublicatieDatum");

            Assert.IsTrue(fieldValueString == "NVM");
            Assert.IsTrue(fieldValueBool);

            var resultObject = result.GetObjectByIdentifier(GlobalId);

            Assert.IsTrue(resultObject != null);
        }

        [TestMethod]
        public void QueryGroupedDynamicStaticTest()
        {
            var result = QueryFactory.Instance.QueryGroupedDynamic<long>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);

            var fieldValueString = result.GetFieldValueByIdentifier(GlobalId, "BronCode");
            var fieldValueBool = result.GetFieldValueByIdentifier(GlobalId, "IsKoop");
            var fieldValueDate = result.GetFieldValueByIdentifier(GlobalId, "PublicatieDatum");

            Assert.IsTrue(fieldValueString.Value == "NVM");
            Assert.IsTrue(fieldValueBool.Value);

            var resultObject = result.GetObjectByIdentifier(GlobalId);

            Assert.IsTrue(resultObject != null);
        }

        [TestMethod]
        public void QueryTest()
        {
            var result = new QueryFactory().Query<QueryResultObject>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);
        }

        [TestMethod]
        public void QueryGroupedTest()
        {
            var result = new QueryFactory().QueryGrouped<long, QueryResultObject>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);

            var fieldValueString = result.GetFieldValueByIdentifier(GlobalId, "BronCode");
            var fieldValueBool = result.GetFieldValueByIdentifier(GlobalId, "IsKoop");
            var fieldValueDate = result.GetFieldValueByIdentifier(GlobalId, "PublicatieDatum");

            Assert.IsTrue(fieldValueString == "NVM");
            Assert.IsTrue(fieldValueBool);

            var resultObject = result.GetObjectByIdentifier(GlobalId);

            Assert.IsTrue(resultObject != null);
        }

        [TestMethod]
        public void QueryGroupedDynamicTest()
        {
            var result = new QueryFactory().QueryGroupedDynamic<long>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);

            var fieldValueString = result.GetFieldValueByIdentifier(GlobalId, "BronCode");
            var fieldValueBool = result.GetFieldValueByIdentifier(GlobalId, "IsKoop");
            var fieldValueDate = result.GetFieldValueByIdentifier(GlobalId, "PublicatieDatum");

            Assert.IsTrue(fieldValueString.Value == "NVM");
            Assert.IsTrue(fieldValueBool.Value);

            var resultObject = result.GetObjectByIdentifier(GlobalId);

            Assert.IsTrue(resultObject != null);
        }
    }
}