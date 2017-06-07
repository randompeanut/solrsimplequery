import { Component } from '@angular/core';
import { MetaService } from '../shared/http/meta.service';
import { SessionStateService } from '../shared//http/sessionstate.service';

@Component({
  selector: 'connectionsettings',
  templateUrl: './connectionsettings.component.html'
})
export class ConnectionSettingsComponent {
	indexerEndPoints: string[];
	indexerChannels: string[];

	constructor(private sessionStateService: SessionStateService, private metaService: MetaService) {
		this.getAllIndexerEndPoints();
		this.getAllIndexerChannels();
	}

	getAllIndexerEndPoints(): void {
		this.metaService.getAllIndexerEndPoints()
		.then(result => {
			this.indexerEndPoints = [];
			result.forEach(r => this.indexerEndPoints.push(r));
		})
		.then(r => this.sessionStateService.onEndHttpBusy.emit());
	}

	getAllIndexerChannels(): void {
		this.metaService.getAllIndexerChannels()
		.then(result => {
			this.indexerChannels = [];
			result.forEach(r => this.indexerChannels.push(r));
		})
		.then(r => this.sessionStateService.onEndHttpBusy.emit());
	}

	emitSettingsChangedEvent(): void {
		this.sessionStateService.settingsUpdated.emit();
	}

	indexerEndPointUpdated(e): void {
		this.sessionStateService.selectedIndexerEndPoint = e.currentTarget.value;
		this.emitSettingsChangedEvent();
	}

	indexerChannelUpdated(e): void {
		this.sessionStateService.selectedIndexerChannel = e.currentTarget.value;
		this.emitSettingsChangedEvent();
	}
}
