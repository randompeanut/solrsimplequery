import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { JsonFormatterComponent } from './json.formatter.component';

@NgModule({
  imports: [
    BrowserModule
  ],
  declarations: [
    JsonFormatterComponent
  ],
  exports: [ JsonFormatterComponent ]
})
export class JsonFormatterModule {

}
