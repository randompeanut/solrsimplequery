import { Injectable } from '@angular/core';
import { ApiHttp } from './api-http.service';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';
import { FilterCriteriaModel } from '../models/filtercriteria.model';
import { DynamicResultModel } from '../models/dynamicresult.model';
import { SessionStateService } from './sessionstate.service';
@Injectable()
export class QueryService {

	constructor(private apiHttp: ApiHttp, private sessionStateService: SessionStateService) {}

	queryDynamic(filterCriteria: FilterCriteriaModel): Promise <any> {
		return this.apiHttp.post(this.sessionStateService.queryEndPoint + '/queryDynamic', JSON.stringify(filterCriteria))
		.map(res => res.json())
		.toPromise();
	}

	queryGroupedDynamic(filterCriteria: FilterCriteriaModel): Promise <any> {
		return this.apiHttp.post(this.sessionStateService.queryEndPoint + '/queryGroupedDynamic', JSON.stringify(filterCriteria))
		.map(res => res.json())
		.toPromise();
	}
}
