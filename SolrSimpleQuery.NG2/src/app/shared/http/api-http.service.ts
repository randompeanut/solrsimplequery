import { Injectable, Injector } from '@angular/core';
import { Http, Headers, BaseRequestOptions, Request, RequestOptions, RequestOptionsArgs, RequestMethod, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import { SessionStateService } from '../http/sessionstate.service';

 @ Injectable()
export class ApiHttp {
	constructor(private http: Http, private sessionStateService: SessionStateService) {}

	_request(request: Request, useToken: boolean): Observable < Response > {
		request.headers = new Headers();
		request.headers.append('Content-Type', 'application/json');
		request.headers.append('Accept', 'application/json');
		return this.http.request(request)
		.catch (this.handleErrorResponse);
	}

	private handleErrorResponse(errorResponse: Response) {
		try {
			var object = errorResponse.json();
			return Observable.throw (object.errors ? object.errors : []);
		} catch (ex) {
			return Observable.throw ([]);
		};
	}
	private requestHelper(requestArgs: RequestOptionsArgs, additionalOptions: RequestOptionsArgs, useToken ?  : boolean): Observable < Response > {
		requestArgs.url = this.sessionStateService.selectedApiEndPoint + '/' + requestArgs.url;
		let options = new RequestOptions(requestArgs);
		if (additionalOptions) {
			options = options.merge(additionalOptions)
		}
		return this._request(new Request(options), useToken)
	}

	get(url: string, useToken: boolean = true, options ?  : RequestOptionsArgs): Observable < Response > {
		return this.requestHelper({
			url: url,
			method: RequestMethod.Get
		}, options, useToken);
	}

	post(url: string, body: string, useToken: boolean = true, options ?  : RequestOptionsArgs): Observable < Response > {
		return this.requestHelper({
			url: url,
			body: body,
			method: RequestMethod.Post
		}, options, useToken);
	}

	put(url: string, body: string, useToken: boolean = true, options ?  : RequestOptionsArgs): Observable < Response > {
		return this.requestHelper({
			url: url,
			body: body,
			method: RequestMethod.Put
		}, options, useToken);
	}

	delete (url: string, useToken: boolean = true, options ?  : RequestOptionsArgs): Observable < Response > {
		return this.requestHelper({
			url: url,
			method: RequestMethod.Delete
		}, options, useToken);
	}

	patch(url: string, body: string, options ?  : RequestOptionsArgs): Observable < Response > {
		return this.requestHelper({
			url: url,
			body: body,
			method: RequestMethod.Patch
		}, options);
	}

	head(url: string, options ?  : RequestOptionsArgs): Observable < Response > {
		return this.requestHelper({
			url: url,
			method: RequestMethod.Head
		}, options);
	}

}
