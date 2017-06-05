import { Component, EventEmitter, Input, Output } from '@angular/core';
import { SessionStateService } from '../../shared//http/sessionstate.service';
import { IMultiSelectOption, IMultiSelectTexts, IMultiSelectSettings } from 'angular-2-dropdown-multiselect';
import { DatePickerOptions, DateModel } from 'ng2-datepicker';

 @Component({
	selector: 'filteredit',
	templateUrl: './filteredit.component.html',
  styleUrls: ['./filteredit.component.css'],
})
export class FilterEditComponent {

	availableFields: IMultiSelectOption[] = [];
	selectedField: string;
	selectedFieldSelected: boolean = false;
	selectedFieldFromValue: string = '';
	selectedFieldToValue: string = '';
	selectedFilterType: string = 'Simple';
	selectedRangeFilterType: string = 'To';
	showSimple: boolean = true;
	showSimpleRange: boolean = false;
	showDateRange: boolean = false;
	showToValue: boolean = false;

	@Input()
  id: number;

  @Input()
  filter: FilterEditComponent;

  @Output()
  optionsModel: string[] = [];

	@Output()
	filterRemoved: EventEmitter < number >  = new EventEmitter < number > ();

	dateFrom: DateModel;
	dateTo: DateModel;
	options: DatePickerOptions = {
		autoApply: true,
		style: 'normal',
		locale: 'nl',
		initialDate: new Date(),
		firstWeekdaySunday: false,
		format: 'DD/MM/YYYY',
		selectYearText: 'select year',
		todayText: 'today',
		clearText: 'clear',
	}

  settings: IMultiSelectSettings;

	texts: IMultiSelectTexts;

	constructor(private sessionStateService: SessionStateService) {
    let counter = this.sessionStateService.filters.length;

    let sessionFilter = this.sessionStateService.filters[this.sessionStateService.currentFilterId];
    if (sessionFilter === undefined) {
      this.id = counter;
      this.sessionStateService.filters.push(this);
    } else {
      sessionFilter.filter = this;
    }
	}

  ngOnChanges() {
    this.settings = this.sessionStateService.getDropdownSettings();
    this.texts = this.sessionStateService.getDropdownTexts();

    this.settings.selectionLimit = 1;
    this.settings.closeOnSelect = true;
    this.settings.autoUnselect = true;
    this.settings.showUncheckAll = false;

    this.getAvailableFilterFields();
    this.sessionStateService.settingsUpdated.subscribe(r => {
      this.getAvailableFilterFields();
    });
    this.sessionStateService.filterFieldSelectionChanged.subscribe(r => {
      this.getAvailableFilterFields();
    });
  }

  @Output()
  get toString(): string {
    if (!this.filter.selectedField || this.filter.selectedField === '') {
      this.filter.selectedField = this.filter.optionsModel && this.filter.optionsModel.length === 1 ? this.filter.optionsModel[0] : '';
    }

    if (!this.filter.selectedFieldSelected) {
      return '';
    }

    if (this.filter.showSimple) {
      if (!this.checkNullOrEmpty(this.filter.selectedFieldFromValue)) {
        return '';
      }
      return this.filter.selectedField + '-' + this.filter.selectedFieldFromValue;
    } else {
      switch (this.filter.selectedFilterType.toLowerCase()) { 
        case 'simple range':
          if (!this.checkNullOrEmpty(this.filter.selectedFieldFromValue)) {
            return '';
          }

          switch (this.filter.selectedRangeFilterType.toLowerCase()) {
            case 'from':
              return this.filter.selectedField + '-' + this.filter.selectedFieldFromValue + '+';
            case 'to':
              return this.filter.selectedField + '-+' + this.filter.selectedFieldFromValue;
            case 'range':
              if (!this.checkNullOrEmpty(this.filter.selectedFieldToValue)) {
                return '';
              }
              return this.filter.selectedField + '-' + this.filter.selectedFieldFromValue + '+' + this.filter.selectedFieldToValue;
          }
          break;
        case 'date range':
          if (!this.checkNullOrEmpty(this.filter.dateFrom.formatted)) {
            return '';
          }

          switch (this.filter.selectedRangeFilterType.toLowerCase()) {
            case 'from':
              return this.filter.selectedField + '-' + this.filter.dateFrom.formatted + '+';
            case 'to':
              return this.filter.selectedField + '-+' + this.filter.dateFrom.formatted;
            case 'range':
              if (!this.checkNullOrEmpty(this.filter.dateTo.formatted)) {
                return '';
              }
              return this.filter.selectedField + '-' + this.filter.dateFrom.formatted + '+' + this.filter.dateTo.formatted ;
          }
          break;
      }
    }

    return '';
  }

	getAvailableFilterFields() {
		this.availableFields = [];
		this.sessionStateService.getAvailableFilterFields().forEach(r => {
			this.availableFields.push({
				id: r,
				name: r
			});
		});
	}

	filterTypeUpdated() {
		this.showSimple = false;
		this.showSimpleRange = false;
		this.showDateRange = false;

		switch (this.selectedFilterType.toLowerCase()) {
		case "simple":
			this.showSimple = true;
			this.showToValue = false;
			break;
		case "simple range":
			this.showSimpleRange = true;
			break;
		case "date range":
			this.showDateRange = true;
			break;
		}
	}

	rangeFilterTypeUpdated() {
		this.showToValue = false;

		switch (this.selectedRangeFilterType.toLowerCase()) {
		case "range":
			this.showToValue = true;
			break;
		}
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

	removeFilter() {
		this.filterRemoved.emit(this.id);
	}

	checkNullOrEmpty(value: string): boolean {
		return value && value.length > 0;
	}
}