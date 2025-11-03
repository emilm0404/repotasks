import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EmployeeService } from '../services/employee.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
    selector: 'app-employee-form',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './employee-form.component.html',
    styleUrls: ['./employee-form.component.css']
})
export class EmployeeFormComponent implements OnInit {
    id?: number;
    rowVersionBase64 = '';
    form!: FormGroup;
    saving = false;
    errorMessage = '';

    constructor(
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private api: EmployeeService
    ) { }

    ngOnInit(): void {
        this.form = this.fb.group({
            firstName: ['', [Validators.required, Validators.maxLength(100)]],
            lastName: ['', [Validators.required, Validators.maxLength(100)]],
            employeeNumber: ['', [Validators.required, Validators.maxLength(32), Validators.pattern(/^[A-Z0-9-]{3,20}$/)]],
        });

        const employeeNumberControl = this.form.get('employeeNumber');
        employeeNumberControl?.valueChanges.subscribe(value => {
            if (typeof value !== 'string') return;
            const normalized = value.trim().toUpperCase();
            if (normalized !== value) {
                employeeNumberControl.setValue(normalized, { emitEvent: false });
            }
        });

        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.id = +id;
            this.api.get(this.id).subscribe(e => {
                this.form.patchValue({
                    firstName: e.firstName,
                    lastName: e.lastName,
                    employeeNumber: e.employeeNumber
                });
                this.rowVersionBase64 = e.rowVersionBase64;
            });
        }
    }

    save(): void {
        this.form.markAllAsTouched();
        if (this.form.invalid) return;
        this.saving = true;
        this.errorMessage = '';

        const dto = {
            firstName: this.form.value.firstName?.trim(),
            lastName: this.form.value.lastName?.trim(),
            employeeNumber: this.form.value.employeeNumber?.trim().toUpperCase()
        };

        const request$ = this.id
            ? this.api.update(this.id, { ...dto, rowVersionBase64: this.rowVersionBase64 })
            : this.api.create(dto);

        request$.subscribe({
            next: () => this.router.navigate(['/employees']),
            error: err => {
                this.errorMessage = this.resolveError(err);
                this.saving = false;
            },
            complete: () => this.saving = false
        });
    }

    cancel(): void {
        this.router.navigate(['/employees']);
    }

    private resolveError(err: unknown): string {
        if (err instanceof HttpErrorResponse) {
            if (err.status === 409) {
                return err.error?.detail ?? 'Conflict detected. Please refresh and try again.';
            }
            if (err.status === 400) {
                return err.error?.detail ?? 'Please fix the highlighted fields and retry.';
            }
            if (err.status === 404) {
                return 'Employee not found. It may have been deleted.';
            }
            if (typeof err.error === 'string') {
                return err.error;
            }
        }
        return 'Unexpected error. Please retry.';
    }
}
