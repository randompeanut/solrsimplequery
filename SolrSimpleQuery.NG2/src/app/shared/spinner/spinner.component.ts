import {Component, OnDestroy} from '@angular/core';
import { SessionStateService } from '../http/sessionstate.service';

@Component({
    selector: 'ajax-spinner',
    templateUrl: './spinner.component.html',
    styleUrls: ['./spinner.component.css']
})
export class SpinnerComponent implements OnDestroy {
    private isDelayedRunning: boolean = false;

    constructor(private sessionStateService: SessionStateService) {
        sessionStateService.onStartHttpBusy.subscribe(() => this.isDelayedRunning = true);
        sessionStateService.onEndHttpBusy.subscribe(() => this.isDelayedRunning = false);
    }

    ngOnDestroy(): any {
    }
}