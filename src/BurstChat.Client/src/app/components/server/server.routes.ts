import { Routes } from "@angular/router";
import { EditServerComponent } from "src/app/components/server/edit-server/edit-server.component";

export const routes: Routes = [
    {
        path: 'edit/:id',
        component: EditServerComponent,
        pathMatch: 'full'
    }
];
