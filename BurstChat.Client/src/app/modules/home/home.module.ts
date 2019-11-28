import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeRoutingModule } from 'src/app/modules/home/home.routing';
import { HomeComponent } from 'src/app/modules/home/components/home/home.component';

@NgModule({
    imports: [
        CommonModule,
        HomeRoutingModule
    ],
    declarations: [
        HomeComponent
    ],
})
export class HomeModule { }
