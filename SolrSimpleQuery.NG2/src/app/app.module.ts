import { NgModule } from '@angular/core'
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { ApiHttp } from './shared/http/api-http.service';
import { MetaService } from './shared/http/meta.service';
import { QueryService } from './shared/http/query.service';
import { SessionStateService } from './shared/http/sessionstate.service';
import { FilterFieldListComponent } from './filterfieldlist/filterfieldlist.component';
import { FieldListFieldListComponent } from './fieldlistfieldlist/fieldlistfieldlist.component';
import { ConnectionSettingsComponent } from './connectionsettings/connectionsettings.component';
import { FilterComponent } from './filter/filter.component';
import { FilterEditComponent } from './filter/filteredit/filteredit.component';
import { MultiselectDropdownModule } from 'angular-2-dropdown-multiselect'
import { DatePickerModule } from 'ng2-datepicker';
import { JsonFormatterModule } from './shared/formatter/json.formatter.module';
import { PrettyJsonModule } from 'angular2-prettyjson';
import { SpinnerComponent } from './shared/spinner/spinner.component';
import { CookieService } from 'ng2-cookies';

@NgModule({
  declarations: [
    AppComponent,
    FilterFieldListComponent,
    FieldListFieldListComponent,
    ConnectionSettingsComponent,
    FilterComponent,
    FilterEditComponent,
    SpinnerComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    ReactiveFormsModule,
    MultiselectDropdownModule,
    DatePickerModule,
    JsonFormatterModule,
    PrettyJsonModule
  ],
  providers: [
    ApiHttp,
    MetaService,
    QueryService,
    CookieService,
    SessionStateService
  ],
  bootstrap: [ AppComponent ]
})
export class AppModule {

}
