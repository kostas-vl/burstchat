import { provideHttpClient, withInterceptors } from "@angular/common/http";
import { ApplicationConfig } from "@angular/core";
import { provideRouter } from "@angular/router";
import { routes } from "src/app/app.routes";
import { endpointInterceptor } from "src/app/interceptors/endpoint/endpoint.interceptor";

export const appconfig: ApplicationConfig = {
    providers: [
        provideHttpClient(withInterceptors([endpointInterceptor])),
        provideRouter(routes)
    ]
}
