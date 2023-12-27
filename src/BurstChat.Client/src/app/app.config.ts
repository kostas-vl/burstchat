import { ApplicationConfig } from "@angular/core";
import { provideRouter } from "@angular/router";
import { routes } from "src/app/app.routes";
import { StorageService } from "src/app/services/storage/storage.service";

export const appconfig: ApplicationConfig = {
    providers: [
        StorageService,
        provideRouter(routes)
    ]
}
