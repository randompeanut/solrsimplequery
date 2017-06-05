using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SolrSimpleQuery.Models;

namespace SolrSimpleQuery.Utility.Extensions
{
    public static class QueryExt
    {
        public static dynamic GetFieldValueByIdentifier<TIdentifierField>(
            this SolrResponse<Dictionary<TIdentifierField, List<DynamicResult>>> resultList,
            TIdentifierField fieldName,
            string identifierFieldName)
        {
            return resultList.Response.Docs[fieldName].FirstOrDefault(r => r.Name == identifierFieldName);
        }

        public static dynamic GetFieldValueByIdentifier<TIdentifierField, TResult>(
            this SolrResponse<Dictionary<TIdentifierField, TResult>> resultList,
            TIdentifierField identifierFieldValue,
            string fieldName)
        {
            var result = resultList.Response.Docs[identifierFieldValue];

            if (result.GetType().Name == "List`1")
            {
                foreach (var item in (IList) result)
                {
                    var properties = item.GetType().GetProperties();

                    var fieldNameProperty = properties.FirstOrDefault(r => r.Name == "FieldName");

                    if (fieldNameProperty == null || fieldNameProperty.GetValue(item).ToString() != fieldName) continue;

                    var valueProperty = properties.FirstOrDefault(r => r.Name == "Value");

                    if (valueProperty != null)
                        return valueProperty.GetValue(item);
                }

                return null;
            }
            var fieldProperty = result.GetType().GetProperties().FirstOrDefault(r => r.Name == fieldName);

            return fieldProperty == null ? null : fieldProperty.GetValue(result);
        }

        public static TResult GetObjectByIdentifier<TIdentifierField, TResult>(
            this SolrResponse<Dictionary<TIdentifierField, TResult>> resultList,
            TIdentifierField identifierFieldValue)
        {
            return resultList.Response.Docs[identifierFieldValue];
        }

        public static object[] GetFacetValues(this FacetResponse facetResponse)
        {
            var returnCollection = new List<object>();
            var jarray = (JArray) ((JContainer) ((JContainer) facetResponse.Facet_Fields).First).First;

            var counter = 0;
            var currentCountElement = new object[2];
            foreach (var jToken in jarray)
            {
                currentCountElement[counter] = ((JValue) jToken).Value;
                counter++;

                if (counter == 2)
                {
                    returnCollection.Add(currentCountElement);
                    currentCountElement = new object[2];
                    counter = 0;
                }
            }

            return returnCollection.ToArray();
        }
    }
}