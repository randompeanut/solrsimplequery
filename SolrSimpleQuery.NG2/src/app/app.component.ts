import {Component} from '@angular/core';
import {SessionStateService} from './shared/http/sessionstate.service';
import {PrettyJsonComponent} from 'angular2-prettyjson';

@Component({
  entryComponents: [PrettyJsonComponent],
  selector: 'app',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(private sessionStateService: SessionStateService) {}
}
