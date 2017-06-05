import { Injectable } from '@angular/core';
import { ApiHttp } from './api-http.service';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';
import { FilterCriteriaModel } from '../models/filtercriteria.model';
import { SessionStateService } from './sessionstate.service';
@Injectable()
export class MetaService {

	constructor(private apiHttp: ApiHttp, private sessionStateService: SessionStateService) {}

	getAllIndexerEndPoints(): Promise <string[]> {
		return this.apiHttp.get(this.sessionStateService.metaEndPoint + '/getAllIndexerEndPoints')
		.map(res => res.json())
		.toPromise();
	}

	getAllIndexerChannels(): Promise <string[]> {
		return this.apiHttp.get(this.sessionStateService.metaEndPoint + '/getAllIndexerChannels')
		.map(res => res.json())
		.toPromise();
	}

	getAllFieldList(filterCriteria: FilterCriteriaModel): Promise <string[]> {
		return this.apiHttp.post(this.sessionStateService.metaEndPoint + '/getallavailablefields', JSON.stringify(filterCriteria))
		.map(res => res.json())
		.toPromise();
	}

	getFieldList(filterCriteria: FilterCriteriaModel): Promise <any[]> {
		return this.apiHttp.post(this.sessionStateService.metaEndPoint + '/getavailablefields', JSON.stringify(filterCriteria))
		.map(res => res.json())
		.toPromise();
	}
}
