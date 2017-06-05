using System.Linq;
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
        public void GetAvailableFieldsTest_withResults()
        {
            var resultAgra =
                MetaFactory.Instance.GetAvailableFields(new FilterCriteria
                {
                    BaseUrl = SolrSimpleQueryBaseUrl,
                    Channel = SolrSimpleQueryChannel,
                    FilterList = new IFilter[] {new SimpleFilter<string>().Create("ZoekType", "agra", true)},
                    Rows = 5
                });

            var resultBedrij =
                MetaFactory.Instance.GetAvailableFields(new FilterCriteria
                {
                    BaseUrl = SolrSimpleQueryBaseUrl,
                    Channel = SolrSimpleQueryChannel,
                    FilterList = new IFilter[] {new SimpleFilter<string>().Create("ZoekType", "bedrij", true)},
                    Rows = 5
                });

            Assert.IsTrue(resultAgra.Any());
            Assert.IsTrue(resultBedrij.Any());
            Assert.IsTrue(resultAgra.Length != resultBedrij.Length);
        }

        [TestMethod]
        public void GetAvailableFieldsTest_noResults()
        {
            var resultAgra =
                MetaFactory.Instance.GetAvailableFields(new FilterCriteria
                {
                    BaseUrl = SolrSimpleQueryBaseUrl,
                    Channel = SolrSimpleQueryChannel,
                    FilterList = new IFilter[] {new SimpleFilter<string>().Create("ZoekType", "agrat", true)},
                    Rows = 5
                });

            Assert.IsFalse(resultAgra.Any());
        }

        [TestMethod]
        public void CheckForFieldTest_exists()
        {
            var result = MetaFactory.Instance.CheckForFields(new FilterCriteria
            {
                BaseUrl = SolrSimpleQueryBaseUrl,
                Channel = SolrSimpleQueryChannel,
                IdentifierFieldName = "OppervlakteHaTot",
                FilterList = new IFilter[] {new SimpleFilter<string>().Create("ZoekType", "agra", true)},
                Rows = 5
            });

            Assert.IsTrue(result == 1);
        }

        [TestMethod]
        public void CheckForFieldTest_notExists()
        {
            var result = MetaFactory.Instance.CheckForFields(new FilterCriteria
            {
                BaseUrl = SolrSimpleQueryBaseUrl,
                Channel = SolrSimpleQueryChannel,
                IdentifierFieldName = "OppervlakteHaTot",
                FilterList = new IFilter[] {new SimpleFilter<string>().Create("ZoekType", "bedrij", true)},
                Rows = 5
            });

            Assert.IsFalse(result == 0);
        }
    }
}