import { Routes } from "@angular/router";
import { EditUserComponent } from "src/app/components/user/edit-user/edit-user.component";

export const routes: Routes = [
    {
        path: '',
        component: EditUserComponent,
        pathMatch: 'full'
    }
];
