using System;
using System.Collections.Generic;
using System.Linq;
using SolrSimpleQuery.Filters;
using SolrSimpleQuery.Filters.Interfaces;
using SolrSimpleQuery.Utility.Enums;
using SolrSimpleQuery.Utility.Extensions;

namespace SolrSimpleQuery.Utility.Formatters
{
    public class StringFormatters
    {
        /// <summary>
        ///     Returns a url encoded list of fields for query
        /// </summary>
        /// <param name="fieldList"></param>
        /// <returns></returns>
        public static string GetFieldsString(List<string> fieldList)
        {
            return fieldList.Any()
                ? "&fl=" + string.Join(UrlExt.UrlCommma, fieldList.Select(f => f.UrlEncode()))
                : string.Empty;
        }

        /// <summary>
        ///     Returns a url encoded list of filters for query
        /// </summary>
        /// <param name="filterCollection"></param>
        /// <returns></returns>
        public static string GetFiltersString(List<IFilter> filterCollection)
        {
            return filterCollection.Any()
                ? filterCollection.Aggregate(string.Empty,
                    (current, filter) => current + "&fq=" + filter.ToString)
                : string.Empty;
        }


        /// <summary>
        ///     Returns a url encoded list of filters for query
        /// </summary>
        /// <param name="filterCollection"></param>
        /// <returns></returns>
        public static string GetFiltersString(List<string> filterCollection)
        {
            var filterString = string.Empty;

            foreach (var filter in filterCollection)
            {
                var filterElements = filter.Split('-');

                if (filter.Contains("+"))
                {
                    if (filterElements.Length != 2) continue;

                    var rangeFilterElements = filterElements[1]
                        .Split(new[] {'+'}, StringSplitOptions.RemoveEmptyEntries);
                    if (rangeFilterElements.Length > 2) continue;

                    var rangeFilterType = FilterType.RangeFilterFromTo;

                    if (rangeFilterElements.Length == 1)
                        rangeFilterType = filterElements[1].StartsWith("+")
                            ? FilterType.RangeFilterTo
                            : FilterType.RangeFilterFrom;

                    var checkFromDate = default(DateTime);
                    var checkToDate = default(DateTime);

                    var hasFromDate = false;
                    var hasToDate = false;

                    if (rangeFilterType == FilterType.RangeFilterFromTo ||
                        rangeFilterType == FilterType.RangeFilterFrom)
                        hasFromDate = DateTime.TryParse(rangeFilterElements[0], out checkFromDate);

                    if (rangeFilterType == FilterType.RangeFilterFromTo || rangeFilterType == FilterType.RangeFilterTo)
                        hasToDate = DateTime.TryParse(
                            rangeFilterType == FilterType.RangeFilterFromTo
                                ? rangeFilterElements[1]
                                : rangeFilterElements[0], out checkToDate);

                    if (hasFromDate || hasToDate)
                        switch (rangeFilterType)
                        {
                            case FilterType.RangeFilterFrom:
                                filterString += "&fq=" + new RangeFilter<DateTime>()
                                                    .CreateFrom(filterElements[0], checkFromDate)
                                                    .ToString;
                                break;
                            case FilterType.RangeFilterTo:
                                filterString += "&fq=" + new RangeFilter<DateTime>()
                                                    .CreateTo(filterElements[0], checkToDate)
                                                    .ToString;
                                break;
                            default:
                                filterString +=
                                    "&fq=" +
                                    new RangeFilter<DateTime>().Create(filterElements[0], checkFromDate, checkToDate)
                                        .ToString;
                                break;
                        }
                    else
                        filterString += "&fq=" + (
                                            rangeFilterType == FilterType.RangeFilterFrom
                                                ? new RangeFilter<string>()
                                                    .CreateFrom(filterElements[0], rangeFilterElements[0])
                                                    .ToString
                                                : rangeFilterType == FilterType.RangeFilterTo
                                                    ? new RangeFilter<string>()
                                                        .CreateTo(filterElements[0], rangeFilterElements[0])
                                                        .ToString
                                                    : new RangeFilter<string>()
                                                        .Create(filterElements[0], rangeFilterElements[0],
                                                            rangeFilterElements[1])
                                                        .ToString
                                        );
                }
                else
                {
                    filterString += filterElements.Length != 2
                        ? string.Empty
                        : "&fq=" + new SimpleFilter<string>().Create(filterElements[0], filterElements[1].Trim('*'),
                                  filter.Contains("*"))
                              .ToString;
                }
            }

            return filterString;
        }

        /// <summary>
        ///     Returns sort for query
        /// </summary>
        /// <param name="sortFieldName"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static string GetSortString(string sortFieldName, string sortBy)
        {
            if (string.IsNullOrEmpty(sortFieldName) || sortBy == "none") return string.Empty;

            return string.Concat("&sort=", sortFieldName, "+", sortBy);
        }
    }
}