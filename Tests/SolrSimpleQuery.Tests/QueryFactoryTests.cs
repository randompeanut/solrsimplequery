using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
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
            FieldsList = new List<string>
            {
                "GlobalId",
                "BronCode",
                "ZoekType",
                "IsKoop",
                "PublicatieDatum"
            },
            //Filters = new IFilter[]
            //{
            //    new SimpleFilter<string>().Create("BronCode", "NVM"),
            //    new SimpleFilter<bool>().Create("IsKoop", true),
            //    new RangeFilter<int>().CreateFrom("KoopPrijs", 125000),
            //    new RangeFilter<DateTime>().CreateTo("PublicatieDatum", TypeExt.CreateDateTime(2017, 2, 1))
            //},
            UrlFiltersList = new List<string>
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
        public async Task QueryStaticTest()
        {
            var result = await QueryFactory.Instance.Query<QueryResultObject>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);
        }

        [TestMethod]
        public async Task QueryStaticDefaultBaseUrlTest_success()
        {
            QueryFactory.SetBaseUrl(SolrSimpleQueryBaseUrl);

            var result = await new QueryFactory().Query<QueryResultObject>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);
        }

        [TestMethod]
        public async Task QueryStaticDefaultChannelTest_success()
        {
            QueryFactory.SetChannel(SolrSimpleQueryChannel);

            var result = await new QueryFactory().Query<QueryResultObject>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);
        }

        /// <summary>
        ///     This should fail because no baseurl is provided and the static instance is not set
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task QueryStaticDefaultBaseUrlTest_fail()
        {
            QueryFactory.ResetInstance();

            var noBaseUrlCriteria = new FilterCriteria();
            noBaseUrlCriteria.InjectFrom(_filterCriteria);
            noBaseUrlCriteria.BaseUrl = null;

            await new QueryFactory().Query<QueryResultObject>(noBaseUrlCriteria);
        }

        /// <summary>
        ///     This should fail because no channel is provided and the static instance is not set
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(WebException))]
        public async Task QueryStaticDefaultChannelTest_fail()
        {
            QueryFactory.ResetInstance();

            var noChannelCriteria = new FilterCriteria();
            noChannelCriteria.InjectFrom(_filterCriteria);
            noChannelCriteria.Channel = null;

            await new QueryFactory().Query<QueryResultObject>(noChannelCriteria);
        }

        [TestMethod]
        public async Task QueryGroupedStaticTest()
        {
            var result = await QueryFactory.Instance.QueryGrouped<long, QueryResultObject>(_filterCriteria);

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
        public async Task QueryGroupedDynamicStaticTest()
        {
            var result = await QueryFactory.Instance.QueryGroupedDynamic<long>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);

            var fieldValueString = result.GetFieldValueByIdentifier(GlobalId, "BronCode");
            var fieldValueBool = result.GetFieldValueByIdentifier(GlobalId, "IsKoop");

            Assert.IsTrue(fieldValueString.Value == "NVM");
            Assert.IsTrue(fieldValueBool.Value);

            var resultObject = result.GetObjectByIdentifier(GlobalId);

            Assert.IsTrue(resultObject != null);
        }

        [TestMethod]
        public async Task QueryTest()
        {
            var result = await new QueryFactory().Query<QueryResultObject>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);
        }

        [TestMethod]
        public async Task QueryGroupedTest()
        {
            var result = await new QueryFactory().QueryGrouped<long, QueryResultObject>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);

            var fieldValueString = result.GetFieldValueByIdentifier(GlobalId, "BronCode");
            var fieldValueBool = result.GetFieldValueByIdentifier(GlobalId, "IsKoop");

            Assert.IsTrue(fieldValueString == "NVM");
            Assert.IsTrue(fieldValueBool);

            var resultObject = result.GetObjectByIdentifier(GlobalId);

            Assert.IsTrue(resultObject != null);
        }

        [TestMethod]
        public async Task QueryGroupedDynamicTest()
        {
            var result = await new QueryFactory().QueryGroupedDynamic<long>(_filterCriteria);

            Assert.IsTrue(result.Response.Docs.Count == 5);

            var fieldValueString = result.GetFieldValueByIdentifier(GlobalId, "BronCode");
            var fieldValueBool = result.GetFieldValueByIdentifier(GlobalId, "IsKoop");

            Assert.IsTrue(fieldValueString.Value == "NVM");
            Assert.IsTrue(fieldValueBool.Value);

            var resultObject = result.GetObjectByIdentifier(GlobalId);

            Assert.IsTrue(resultObject != null);
        }
    }
}