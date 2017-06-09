import { Component } from '@angular/core';
import { MetaService } from '../shared/http/meta.service';
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
    
		this.getFieldList(false);
		
		this.sessionStateService.settingsUpdated.subscribe(r => {
			this.getFieldList(true);
		});
	}

	getFieldList(resetOptions: boolean): void {
		let filterCriteria = this.sessionStateService.persistenceModel.getSeededFilterCriteria();
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

			this.sessionStateService.persistenceModel.allAvailableFields = fieldNames;
			this.sessionStateService.allAvailableFieldsChanged.emit();
		}).
		then(r => {
			if (resetOptions) {
				if (this.optionsModel !== undefined) {
					let tmpOptionsModel = [];
					this.optionsModel.forEach(r => {
						let foundObject = this.fieldList.find(l => l.id === r);
						if (foundObject !== undefined) {
							tmpOptionsModel.push(r);
						}
					});

					this.optionsModel = tmpOptionsModel;
					this.sessionStateService.persistenceModel.selectedAvailableFilterFields = this.optionsModel;
				}
			} else {
				this.optionsModel = this.sessionStateService.persistenceModel.selectedAvailableFilterFields;
			}
		})
		.then(r => this.sessionStateService.onEndHttpBusy.emit())
		.catch (r => {
			this.fieldList = [];
			this.optionsModel = [];
		});
	}

	selectionChanged(force: boolean = true) {
		this.sessionStateService.persistenceModel.selectedAvailableFilterFields = this.optionsModel;
		if (force) {
			setTimeout(() => {
				this.selectionChanged(false);
			}, 500); //sometimes the collection doesn't update immediately
		}
	}

	refreshFilters(force: boolean = true) {
		this.sessionStateService.persistenceModel.selectedAvailableFilterFields = this.optionsModel;

		if (force) {
			setTimeout(() => {
				this.refreshFilters(false);
			}, 500); //sometimes the collection doesn't update immediately
		}
		this.sessionStateService.filterFieldSelectionChanged.emit();
	}
}
