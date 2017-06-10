using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrSimpleQuery.Filters;
using SolrSimpleQuery.Filters.Interfaces;
using SolrSimpleQuery.Models;

namespace SolrSimpleQuery.Tests
{
    [TestClass]
    public class MetaFactoryTests
    {
        private const string SolrSimpleQueryBaseUrl = "http://www.int.dev.funda.nl:8080/indexer";
        private const string SolrSimpleQueryChannel = "fib";

        [TestMethod]
        public async Task GetAvailableFieldsTest_withResults()
        {
            var resultAgra =
                await MetaFactory.Instance.GetAvailableFields(new FilterCriteria
                {
                    BaseUrl = SolrSimpleQueryBaseUrl,
                    Channel = SolrSimpleQueryChannel,
                    FiltersList = new List<IFilter> {new SimpleFilter<string>().Create("ZoekType", "agra", true)},
                    Rows = 5
                });

            var resultBedrij =
                await MetaFactory.Instance.GetAvailableFields(new FilterCriteria
                {
                    BaseUrl = SolrSimpleQueryBaseUrl,
                    Channel = SolrSimpleQueryChannel,
                    FiltersList = new List<IFilter> { new SimpleFilter<string>().Create("ZoekType", "bedrij", true)},
                    Rows = 5
                });

            Assert.IsTrue(resultAgra.Any());
            Assert.IsTrue(resultBedrij.Any());
            Assert.IsTrue(resultAgra.Count != resultBedrij.Count);
        }

        [TestMethod]
        public async Task GetAvailableFieldsTest_noResults()
        {
            var resultAgra =
                await MetaFactory.Instance.GetAvailableFields(new FilterCriteria
                {
                    BaseUrl = SolrSimpleQueryBaseUrl,
                    Channel = SolrSimpleQueryChannel,
                    FiltersList = new List<IFilter> { new SimpleFilter<string>().Create("ZoekType", "agrat", true)},
                    Rows = 5
                });

            Assert.IsFalse(resultAgra.Any());
        }

        [TestMethod]
        public async Task CheckForFieldTest_exists()
        {
            var result = await MetaFactory.Instance.CheckForFields(new FilterCriteria
            {
                BaseUrl = SolrSimpleQueryBaseUrl,
                Channel = SolrSimpleQueryChannel,
                IdentifierFieldName = "OppervlakteHaTot",
                FiltersList = new List<IFilter> { new SimpleFilter<string>().Create("ZoekType", "agra", true)},
                Rows = 5
            });

            Assert.IsTrue(result == 1);
        }

        [TestMethod]
        public async Task CheckForFieldTest_notExists()
        {
            var result = await MetaFactory.Instance.CheckForFields(new FilterCriteria
            {
                BaseUrl = SolrSimpleQueryBaseUrl,
                Channel = SolrSimpleQueryChannel,
                IdentifierFieldName = "OppervlakteHaTot",
                FiltersList = new List<IFilter> { new SimpleFilter<string>().Create("ZoekType", "bedrij", true)},
                Rows = 5
            });

            Assert.IsFalse(result == 0);
        }
    }
}