import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { RootComponent } from 'src/app/components/root/root.component';

@NgModule({
    imports: [
        BrowserModule,
        AppRoutingModule
    ],
    declarations: [
        RootComponent,
    ],
    providers: [],
    bootstrap: [RootComponent]
})
export class AppModule { }
