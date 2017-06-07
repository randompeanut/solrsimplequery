import { Injectable, EventEmitter } from '@angular/core';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';
import { FilterCriteriaModel } from '../models/filtercriteria.model';
import { FilterEditComponent } from '../../filter/filteredit/filteredit.component';
import { IMultiSelectTexts, IMultiSelectSettings } from 'angular-2-dropdown-multiselect';

@Injectable()
export class SessionStateService {

	settingsUpdated: EventEmitter < any >  = new EventEmitter < any > ();
	allAvailableFieldsChanged: EventEmitter < any >  = new EventEmitter < any > ();
	filterFieldSelectionChanged: EventEmitter < any >  = new EventEmitter < any > ();
	fieldListFieldSelectionChanged: EventEmitter < any >  = new EventEmitter < any > ();
    queryExecuted: EventEmitter < any > = new EventEmitter < any > ();
	onStartHttpBusy: EventEmitter<any> = new EventEmitter<any>();
	onEndHttpBusy: EventEmitter<any> = new EventEmitter<any>();

    currentFilterId: number = -1;

	defaultApiEndPoint = "http://localhost:1177/api";
	defaultIndexerEndPoint = "http://www.int.dev.funda.nl:8080/indexer";
	defaultIndexerChannel = "fib";
	defaultRows = 10;
	defaultStart = 0;
	queryEndPoint = "query";
	metaEndPoint = "meta";

	selectedApiEndPoint: string;
	selectedIndexerEndPoint: string;
	selectedIndexerChannel: string;
	selectedRows: number;
	selectedStart: number;

	allAvailableFields: string[];
	selectedAvailableFilterFields: string[];
	selectedAvailableFieldListFields: string[];

	filterStrings: string[] = [];
    filters: FilterEditComponent[] = [];

    dropDownSettings: IMultiSelectSettings = {
		enableSearch: true,
		showUncheckAll: true,
		buttonClasses: 'btn btn-default btn-block',
		containerClasses: 'width-override dropdown-inline',
		dynamicTitleMaxItems: 5,
		selectionLimit: 0,
	};

	dropDownTexts: IMultiSelectTexts = {
		checkAll: 'Select all',
		uncheckAll: 'Unselect all',
		checked: 'item selected',
		checkedPlural: 'items selected',
		searchPlaceholder: 'Find',
		defaultTitle: 'Select',
		allSelected: 'All selected',
	};

    queryResultJson: any;

	constructor() {
		this.selectedApiEndPoint = this.defaultApiEndPoint;
		this.selectedIndexerEndPoint = this.defaultIndexerEndPoint;
		this.selectedIndexerChannel = this.defaultIndexerChannel;
		this.selectedRows = this.defaultRows;
		this.selectedStart = this.defaultStart;
	}

	getSeededFilterCriteria(): FilterCriteriaModel {
		let filterCriteriaModel = new FilterCriteriaModel();

		filterCriteriaModel.baseUrl = this.selectedIndexerEndPoint;
		filterCriteriaModel.channel = this.selectedIndexerChannel;
		filterCriteriaModel.rows = this.selectedRows;
		filterCriteriaModel.start = this.selectedStart;

		return filterCriteriaModel;
	}

	getAvailableFilterFields() {
		if (!this.selectedAvailableFilterFields
			 || this.selectedAvailableFilterFields.length === 0) {
			return this.allAvailableFields;
		}

		return this.selectedAvailableFilterFields;
	}

	getAvailableFFieldListFields() {
		if (!this.selectedAvailableFieldListFields
			 || this.selectedAvailableFieldListFields.length === 0) {

			return [];
		}

		return this.selectedAvailableFieldListFields;
	}

    getDropdownSettings() : IMultiSelectSettings {
        let settings: IMultiSelectSettings = this.simpleClone(this.dropDownSettings);

        return settings;
    }

    getDropdownTexts() : IMultiSelectTexts {
        let texts: IMultiSelectTexts = this.simpleClone(this.dropDownTexts);

        return texts;
    }

    simpleClone(obj: any) {
        return Object.assign({}, obj);
    }
}
