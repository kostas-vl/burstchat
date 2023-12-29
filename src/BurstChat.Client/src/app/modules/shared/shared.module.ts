import { NgModule } from '@angular/core';
import { ImageCropperModule } from 'ngx-image-cropper';
import { CommonModule } from '@angular/common';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { CardComponent } from 'src/app/modules/shared/components/card/card.component';
import { CardHeaderComponent } from 'src/app/modules/shared/components/card-header/card-header.component';
import { CardBodyComponent } from 'src/app/modules/shared/components/card-body/card-body.component';
import { CardFooterComponent } from 'src/app/modules/shared/components/card-footer/card-footer.component';
import { DialogComponent } from 'src/app/modules/shared/components/dialog/dialog.component';
import { AvatarComponent } from 'src/app/modules/shared/components/avatar/avatar.component';
import { ImageCropComponent } from 'src/app/modules/shared/components/image-crop/image-crop.component';
import { ShimmerListComponent } from 'src/app/modules/shared/components/shimmer-list/shimmer-list.component';
import { ExpanderComponent } from 'src/app/modules/shared/components/expander/expander.component';

import { library } from '@fortawesome/fontawesome-svg-core';
import { faAngleRight, faAngleDown } from '@fortawesome/free-solid-svg-icons';

@NgModule({
    imports: [
        CommonModule,
        ImageCropperModule,
        FontAwesomeModule,
        CardComponent,
        CardHeaderComponent,
    ],
    declarations: [
        CardBodyComponent,
        CardFooterComponent,
        DialogComponent,
        AvatarComponent,
        ImageCropComponent,
        ShimmerListComponent,
        ExpanderComponent
    ],
    exports: [
        CardComponent,
        CardHeaderComponent,
        CardBodyComponent,
        CardFooterComponent,
        DialogComponent,
        AvatarComponent,
        ImageCropComponent,
        ShimmerListComponent,
        ExpanderComponent
    ]
})
export class SharedModule {

    /**
     * Creates an instance of SharedModule.
     * @memberof SharedModule
     */
    constructor() {
        library.add(faAngleRight, faAngleDown);
    }

}
