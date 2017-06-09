import { Injectable, EventEmitter } from '@angular/core';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';
import { IMultiSelectTexts, IMultiSelectSettings } from 'angular-2-dropdown-multiselect';
import { CookieService } from 'ng2-cookies';
import { PersistenceModel } from '../models/persistence.model';

@Injectable()
export class SessionStateService {

	settingsUpdated: EventEmitter<any>  = new EventEmitter<any>();
	allAvailableFieldsChanged: EventEmitter<any>  = new EventEmitter<any>();
	filterFieldSelectionChanged: EventEmitter<any>  = new EventEmitter<any>();
	fieldListFieldSelectionChanged: EventEmitter<any>  = new EventEmitter<any>();
    queryExecuted: EventEmitter<any> = new EventEmitter<any>();
	onStartHttpBusy: EventEmitter<any> = new EventEmitter<any>();
	onEndHttpBusy: EventEmitter<any> = new EventEmitter<any>();

	persistenceModel: PersistenceModel;

	queryEndPoint = "query";
	metaEndPoint = "meta";

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

	constructor(cookieService: CookieService) {
		this.persistenceModel = new PersistenceModel(cookieService);
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
