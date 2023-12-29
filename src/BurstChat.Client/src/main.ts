import { enableProdMode } from '@angular/core';

import { environment } from './environments/environment';
import { bootstrapApplication } from '@angular/platform-browser';
import { RootComponent } from 'src/app/components/root/root.component';
import { appconfig } from 'src/app/app.config';

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(RootComponent, appconfig)
    .catch(err => console.error(err));

