import { FilterEditComponent } from '../../filter/filteredit/filteredit.component';
import { FilterCriteriaModel } from '../models/filtercriteria.model';
import { CookieService } from 'ng2-cookies';

export class PersistenceModel {

    defaultApiEndPoint = "http://localhost:1177/api";
	defaultIndexerEndPoint = "http://www.int.dev.funda.nl:8080/indexer";
	defaultIndexerChannel = "fib";
	defaultRows = 10;
	defaultStart = 0;
    allAvailableFields: string[];
    queryResultJson: any;
    currentFilterId: number = -1;
    filterStrings: string[] = [];
    filters: FilterEditComponent[] = [];

    restoreDefaults() {
        this.selectedApiEndPoint = this.defaultApiEndPoint;
        this.selectedIndexerEndPoint = this.defaultIndexerEndPoint;
	    this.selectedIndexerChannel = this.defaultIndexerChannel;;
	    this.selectedRows = this.defaultRows;
	    this.selectedStart = this.defaultStart;

        this.selectedAvailableFilterFields = [];
        this.selectedAvailableFieldListFields= [];
        this.filters = [];
    }

    get selectedApiEndPoint(): string {
        let value = this.getCookie("selectedApiEndPoint");
        if (value.length > 0) {
            return value;
        }

        return this.defaultApiEndPoint;
    }

    set selectedApiEndPoint(value: string) {
        this.setCookie("selectedApiEndPoint", value);
    }

	get selectedIndexerEndPoint(): string {
        let value = this.getCookie("selectedIndexerEndPoint");
        if (value.length > 0) {
            return value;
        }

        value = this.defaultIndexerEndPoint;
    }

    set selectedIndexerEndPoint(value: string) {
        this.setCookie("selectedIndexerEndPoint", value);
    }

    get selectedIndexerChannel(): string {
        let value = this.getCookie("selectedIndexerChannel");
        if (value.length > 0) {
            return value;
        }

        return this.defaultIndexerChannel;
    }

    set selectedIndexerChannel(value: string) {
        this.setCookie("selectedIndexerChannel", value);
    }

    get selectedRows() : number {
        let value = this.getCookie("selectedRows");
        if (value.length > 0) {
            return parseInt(value, 10);
        }

        return this.defaultRows;
    }

    set selectedRows(value: number) {
        this.setCookie("selectedRows", value.toString());
    }

    get selectedStart() : number {
        let value = this.getCookie("selectedStart");
        if (value.length > 0) {
            return parseInt(value, 10);
        }

        return this.defaultStart;
    }

    set selectedStart(value: number) {
        this.setCookie("selectedStart", value.toString());
    }

    get selectedAvailableFilterFields(): string[] {
        let value = this.getCookie("selectedAvailableFilterFields");
        if (value.length > 0) {
            return JSON.parse(value);
        }

        return [];
    }

    set selectedAvailableFilterFields(value: string[]) {
        this.setCookie("selectedAvailableFilterFields", JSON.stringify(value));
    }

    get selectedAvailableFieldListFields(): string[] {
        let value = this.getCookie("selectedAvailableFieldListFields");
        let valid = this.checkArrayStringEmpty(value)
        if (valid) {
            return JSON.parse(value);
        }

        return [];
    }

    set selectedAvailableFieldListFields(value: string[]) {
        this.setCookie("selectedAvailableFieldListFields", JSON.stringify(value));
    }

    constructor(private cookieService: CookieService) {
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

	setCookie(name: string, value: string) {
		this.cookieService.set(name, value);
	}

	getCookie(name: string): string {
		if (this.cookieService.check(name)) {
			return this.cookieService.get(name);
		}

		return '';
	}

	deleteCookie(name: string) {
		if (this.cookieService.check(name)) {
			this.cookieService.delete(name);
		}
	}

    checkArrayStringEmpty(value: string) {
        return value && value.length > 0 && value !== '[]';
    }
}