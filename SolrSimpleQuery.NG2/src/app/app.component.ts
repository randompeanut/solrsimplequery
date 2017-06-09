import {Component} from '@angular/core';
import {SessionStateService} from './shared/http/sessionstate.service';

@Component({
  selector: 'app',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  jsonFormat: string = "Raw";

  constructor(private sessionStateService: SessionStateService) { }

  restoreDefaults() {
    this.sessionStateService.persistenceModel.restoreDefaults();

    this.sessionStateService.settingsUpdated.emit();
    this.sessionStateService.allAvailableFieldsChanged.emit();
    this.sessionStateService.filterFieldSelectionChanged.emit();
    this.sessionStateService.fieldListFieldSelectionChanged.emit();
  }
}
