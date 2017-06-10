export class FilterCriteriaModel {
    identifierFieldName: string;
    identifierFieldValue: string;
    urlFilters: string[];
    fields: string[];
    sortFieldName: string;
    sortBy: string;
    start: number = 0;
    rows: number = 1;
    overrideQuery: string;
    baseUrl: string;
    channel: string;
    facetQuery: boolean = false;
    facetFieldName: string;
}