import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardComponent } from 'src/app/modules/shared/components/card/card.component';
import { CardHeaderComponent } from 'src/app/modules/shared/components/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/modules/shared/components/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/modules/shared/components/card-footer/card-footer.component';
import { DialogComponent } from 'src/app/modules/shared/components/dialog/dialog.component';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
        DialogComponent
    ],
    exports: [
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
        DialogComponent
    ]
})
export class SharedModule { }