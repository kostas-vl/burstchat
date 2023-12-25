import { ApplicationConfig } from "@angular/core";
import { provideRouter } from "@angular/router";
import { routes } from "src/app/app.routes";

export const appconfig: ApplicationConfig = {
    providers: [
        provideRouter(routes)
    ]
}
