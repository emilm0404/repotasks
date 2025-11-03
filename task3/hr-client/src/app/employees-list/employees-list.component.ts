import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { EmployeeService } from '../services/employee.service';
import { Employee } from '../models/employee';
import { HttpErrorResponse } from '@angular/common/http';

type SortField = 'LastName' | 'FirstName' | 'EmployeeNumber';

@Component({
    selector: 'app-employees-list',
    standalone: true,
    imports: [CommonModule, FormsModule, RouterLink],
    templateUrl: './employees-list.component.html',
    styleUrls: ['./employees-list.component.css']
})
export class EmployeesListComponent implements OnInit {
    search = '';
    page = 1;
    pageSize = 10;
    sort: SortField = 'LastName';      
    dir: 'asc' | 'desc' = 'asc';
    total = 0;
    rows: Employee[] = [];
    loading = false;
    errorMessage = '';

    constructor(private api: EmployeeService) { }

    ngOnInit(): void {
        this.load();
    }

    get totalPages(): number {
        return Math.max(1, Math.ceil(this.total / this.pageSize));
    }

    trackById = (_: number, e: Employee) => e.id;

    load(): void {
        this.loading = true;
        this.errorMessage = '';
        this.api.list(this.search, this.page, this.pageSize, this.sort, this.dir)
            .subscribe({
                next: res => {
                    this.rows = res.items;
                    this.total = res.totalCount;
                    this.loading = false;
                },
                error: err => {
                    this.rows = [];
                    this.total = 0;
                    this.loading = false;
                    this.errorMessage = this.resolveError(err);
                }
            });
    }

    setSort(field: SortField): void {
        if (this.sort === field) {
            this.dir = this.dir === 'asc' ? 'desc' : 'asc';
        } else {
            this.sort = field;
            this.dir = 'asc';
        }
        this.load();
    }

    remove(e: Employee): void {
        if (!confirm(`Delete ${e.firstName} ${e.lastName}?`)) return;
        this.api.delete(e.id, e.rowVersionBase64).subscribe({
            next: () => this.load(),
            error: err => {
                this.errorMessage = this.resolveError(err);
            }
        });
    }

    private resolveError(err: unknown): string {
        if (err instanceof HttpErrorResponse) {
            if (err.status === 0) {
                return 'Cannot reach the server. Is the API running?';
            }
            if (err.status === 409) {
                return err.error?.detail ?? 'Conflict detected. Please refresh.';
            }
            if (err.status === 400) {
                return err.error?.detail ?? 'Invalid request.';
            }
            if (typeof err.error === 'string') {
                return err.error;
            }
        }
        return 'Unexpected error when loading employees.';
    }
}
