import { Component } from '@angular/core';
import { SessionStateService } from '../shared/http/sessionstate.service';
import { FilterEditComponent } from './filteredit/filteredit.component';
import { IMultiSelectOption, IMultiSelectTexts, IMultiSelectSettings } from 'angular-2-dropdown-multiselect';
import { QueryService } from '../shared/http/query.service';

@Component({
  selector: 'filter',
  templateUrl: './filter.component.html'
})
export class FilterComponent {
	availableFields: IMultiSelectOption[] = [];
	optionsModel: string[] = [];
  sortByOptionsmodel: string[] = [];
	facetOptionsmodel: string[] = [];
	facetResultsmodel: string[] = [];
	selectedField: string;
	selectedFieldSelected: boolean = false;
	settings: IMultiSelectSettings;
	texts: IMultiSelectTexts;
  sortBy: string = "Asc";
	facetResults: IMultiSelectOption[] = [];

	constructor(private sessionStateService: SessionStateService, private queryService: QueryService) {
		sessionStateService.settingsUpdated.subscribe(r => {
			this.getAvailableFilterFields();
		});
		sessionStateService.allAvailableFieldsChanged.subscribe(r => {
			this.getAvailableFilterFields();
		});

		this.settings = this.sessionStateService.getDropdownSettings();
		this.texts = this.sessionStateService.getDropdownTexts();

		this.settings.selectionLimit = 1;
		this.settings.closeOnSelect = true;
		this.settings.autoUnselect = true;
		this.settings.showUncheckAll = false;
	}

	addFilter() {
    this.sessionStateService.persistenceModel.currentFilterId++;
		new FilterEditComponent(this.sessionStateService);
	}

	filterRemoved(e: number) {
		let filter = this.sessionStateService.persistenceModel.filters.find(r => r.id === e);
		let index = this.sessionStateService.persistenceModel.filters.indexOf(filter);
		this.sessionStateService.persistenceModel.filters.splice(index, 1);
    let counter = -1;
    this.sessionStateService.persistenceModel.filters.forEach(f => {
      counter++;
      f.id = counter;
    });

    this.sessionStateService.persistenceModel.currentFilterId = counter;
	}

	getAvailableFilterFields() {
		this.availableFields = [];
		this.sessionStateService.persistenceModel.getAvailableFilterFields().forEach(r => {
			this.availableFields.push({
				id: r,
				name: r
			});
		});
	}

	fieldUpdatedAdded() {
		this.selectedFieldSelected = true;
    if (this.optionsModel && this.optionsModel.length > 0) {
        this.selectedField = this.optionsModel[this.optionsModel.length - 1];
      }
	}

	fieldUpdatedRemoved() {
		if (this.optionsModel.length === 1) {
			this.fieldUpdatedAdded();
		} else {
			this.selectedFieldSelected = false;
		}
	}

	doQuery(facet: boolean = false) {	
		let formattedFilters = this.sessionStateService.persistenceModel.filters.filter(r => r.toString && r.toString !== '').map(r => r.toString);
		let filterCriteria = this.sessionStateService.persistenceModel.getSeededFilterCriteria();

		filterCriteria.urlFilterList = this.sessionStateService.persistenceModel.filters.map(r => r.toString);
		filterCriteria.fieldList = this.sessionStateService.persistenceModel.getAvailableFFieldListFields();

    filterCriteria.sortFieldName = this.sortByOptionsmodel && this.sortByOptionsmodel.length === 1 ? this.sortByOptionsmodel[0] : '';
    filterCriteria.sortBy = this.sortBy;

		if (facet && this.facetOptionsmodel && this.facetOptionsmodel.length === 1) {
			filterCriteria.facetQuery = true;
			filterCriteria.facetFieldName = this.facetOptionsmodel[0];
		}

		if (this.selectedFieldSelected) {
			if (!this.selectedField && this.optionsModel && this.optionsModel.length === 1) {
				this.selectedField = this.optionsModel[0];
			}

			filterCriteria.identifierFieldName = this.selectedField;

			this.queryService.queryGroupedDynamic(filterCriteria)
			.then(r => {
				this.sessionStateService.persistenceModel.queryResultJson = r;

				if (filterCriteria.facetQuery) {
					this.getFacetResults(r);
				}

        this.sessionStateService.queryExecuted.emit();
			})
			.then(r => this.sessionStateService.onEndHttpBusy.emit());
		} else {
			this.queryService.queryDynamic(filterCriteria)
			.then(r => {
				this.sessionStateService.persistenceModel.queryResultJson = r;

				if (filterCriteria.facetQuery) {
					this.getFacetResults(r);
				}

        this.sessionStateService.queryExecuted.emit();
			})
			.then(r => this.sessionStateService.onEndHttpBusy.emit());
		}
	}

	getFacetResults(r){
			this.facetResults = [];
					r.Facet_Counts.Facet_Fields.forEach(element => {
						this.facetResults.push({id: element[0], name: element[0] + ' (' + element[1] + ')'});
					});
	}
}