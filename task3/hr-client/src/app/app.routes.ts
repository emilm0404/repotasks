import { Routes } from '@angular/router';
import { EmployeesListComponent } from './employees-list/employees-list.component';
import { EmployeeFormComponent } from './employee-form/employee-form.component';

export const routes: Routes = [
    { path: '', redirectTo: 'employees', pathMatch: 'full' },
    { path: 'employees', component: EmployeesListComponent },
    { path: 'employees/new', component: EmployeeFormComponent },
    { path: 'employees/:id/edit', component: EmployeeFormComponent },
    { path: '**', redirectTo: 'employees' }
];
