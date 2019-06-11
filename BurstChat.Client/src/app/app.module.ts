import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faSmile, faDragon } from '@fortawesome/free-solid-svg-icons';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { RootComponent } from 'src/app/components/root/root.component';
import { SidebarComponent } from 'src/app/components/sidebar/sidebar.component';

@NgModule({
    imports: [
        BrowserModule,
        FontAwesomeModule,
        AppRoutingModule
    ],
    declarations: [
        RootComponent,
        SidebarComponent
    ],
    providers: [],
    bootstrap: [RootComponent]
})
export class AppModule {

    /**
     * Creates an instance of AppModule.
     * @memberof AppModule
     */
    constructor() {
        library.add(faSmile, faDragon);
    }

}
