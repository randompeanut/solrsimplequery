import { Component, Input } from '@angular/core';
import { IMultiSelectOption, IMultiSelectTexts, IMultiSelectSettings } from 'angular-2-dropdown-multiselect';
import { SessionStateService } from '../shared//http/sessionstate.service';

@Component({
  selector: 'fieldlistfieldlist',
  templateUrl: './fieldlistfieldlist.component.html',
})
export class FieldListFieldListComponent {
  @Input()
  showLabel: boolean;

	fieldList: IMultiSelectOption[];
	optionsModel: string[] = [];

	settings: IMultiSelectSettings;

	texts: IMultiSelectTexts;

	constructor(private sessionStateService: SessionStateService) {
		sessionStateService.allAvailableFieldsChanged.subscribe(r => {
			this.fieldList = [];
			sessionStateService.persistenceModel.allAvailableFields.forEach(element => {
				this.fieldList.push({
					id: element,
					name: element
				});
			});

      this.settings = this.sessionStateService.getDropdownSettings();
      this.settings.showCheckAll = true;

      this.texts = this.sessionStateService.getDropdownTexts();

			let fieldNames: string[] = [];

			this.fieldList.forEach(r => fieldNames.push(r.name));

			this.optionsModel = this.sessionStateService.persistenceModel.selectedAvailableFieldListFields;
		});
	}

	selectionChanged(force: boolean = true) {
		this.sessionStateService.persistenceModel.selectedAvailableFieldListFields = this.optionsModel;
		if (force) {
			setTimeout(() => {
				this.selectionChanged(false);
			}, 500); //sometimes the collection doesn't update immediately
		}
	}

	refreshFieldList() {
		this.sessionStateService.persistenceModel.selectedAvailableFieldListFields = this.optionsModel;
	}
}
