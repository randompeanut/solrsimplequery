import {Component} from '@angular/core';
import {SessionStateService} from './shared/http/sessionstate.service';

@Component({
  selector: 'app',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(private sessionStateService: SessionStateService) {}
}
