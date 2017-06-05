import { Component } from '@angular/core';
import {Observable} from 'rxjs/Observable';
import { MetaService } from '../shared/http/meta.service';
import { FilterCriteriaModel } from '../shared/models/filtercriteria.model';
import { IMultiSelectOption, IMultiSelectTexts, IMultiSelectSettings } from 'angular-2-dropdown-multiselect';
import { SessionStateService } from '../shared//http/sessionstate.service';

@Component({
  selector: 'filterfieldlist',
  templateUrl: './filterfieldlist.component.html'
})
export class FilterFieldListComponent {
	fieldList: IMultiSelectOption[];
	optionsModel: string[] = [];

	settings: IMultiSelectSettings;
	texts: IMultiSelectTexts;

	constructor(private metaService: MetaService, private sessionStateService: SessionStateService) {
      this.settings = this.sessionStateService.getDropdownSettings();
      this.texts = this.sessionStateService.getDropdownTexts();
    
		this.getFieldList();

		this.sessionStateService.settingsUpdated.subscribe(r => {
			this.getFieldList();
		});
	}

	getFieldList(): void {
		let filterCriteria = this.sessionStateService.getSeededFilterCriteria();
		this.metaService.getAllFieldList(filterCriteria)
		.then(result => {
			this.fieldList = [];
			result.forEach(element => {
				this.fieldList.push({
					id: element,
					name: element
				});
			});

			let fieldNames: string[] = [];

			this.fieldList.forEach(r => fieldNames.push(r.name));

			this.sessionStateService.allAvailableFields = fieldNames;
			this.sessionStateService.allAvailableFieldsChanged.emit();
		}).
		then(r => {
			if (this.optionsModel !== undefined) {
				let tmpOptionsModel = [];
				this.optionsModel.forEach(r => {
					let foundObject = this.fieldList.find(l => l.id === r);
					if (foundObject !== undefined) {
						tmpOptionsModel.push(r);
					}
				});

				this.optionsModel = tmpOptionsModel;
				this.sessionStateService.selectedAvailableFilterFields = this.optionsModel;
			}
		})
		.catch (r => {
			this.fieldList = [];
			this.optionsModel = [];
		});
	}

	refreshFilters(force: boolean = true) {
		this.sessionStateService.selectedAvailableFilterFields = this.optionsModel;

		if (force) {
			setTimeout(() => {
				this.refreshFilters(false);
			}, 500); //sometimes the collection doesn't update immediately
		}
		this.sessionStateService.filterFieldSelectionChanged.emit();
	}
}
