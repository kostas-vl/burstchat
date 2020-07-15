import { NgModule } from '@angular/core';
import { ImageCropperModule } from 'ngx-image-cropper';
import { CommonModule } from '@angular/common';
import { CardComponent } from 'src/app/modules/shared/components/card/card.component';
import { CardHeaderComponent } from 'src/app/modules/shared/components/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/modules/shared/components/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/modules/shared/components/card-footer/card-footer.component';
import { DialogComponent } from 'src/app/modules/shared/components/dialog/dialog.component';
import { AvatarComponent } from 'src/app/modules/shared/components/avatar/avatar.component';
import { ImageCropComponent } from 'src/app/modules/shared/components/image-crop/image-crop.component';

@NgModule({
    imports: [
        CommonModule,
        ImageCropperModule
    ],
    declarations: [
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
        DialogComponent,
        AvatarComponent,
        ImageCropComponent
    ],
    exports: [
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
        DialogComponent,
        AvatarComponent,
        ImageCropComponent
    ]
})
export class SharedModule { }
