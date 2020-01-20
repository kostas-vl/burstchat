import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { library } from '@fortawesome/fontawesome-svg-core';
import { faDragon } from '@fortawesome/free-solid-svg-icons';
import { HomeRoutingModule } from 'src/app/modules/home/home.routing';
import { HomeComponent } from 'src/app/modules/home/components/home/home.component';

@NgModule({
    imports: [
        CommonModule,
        FontAwesomeModule,
        HomeRoutingModule
    ],
    declarations: [
        HomeComponent
    ],
})
export class HomeModule {
    /**
     * Creates an instance of UserModule.
     * @memberof UserModule
     */
    constructor() {
        library.add(faDragon);
    }
}
