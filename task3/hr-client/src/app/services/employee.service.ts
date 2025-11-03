import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Employee } from '../models/employee';
import { PagedResult } from '../models/paged-result';

@Injectable({ providedIn: 'root' })
export class EmployeeService {
    private readonly base = `${environment.apiUrl}/employees`;

    constructor(private http: HttpClient) { }

    list(
        search = '',
        page = 1,
        pageSize = 10,
        sort: 'LastName' | 'FirstName' | 'EmployeeNumber' = 'LastName',
        dir: 'asc' | 'desc' = 'asc'
    ): Observable<PagedResult<Employee>> {
        const trimmedSearch = (search ?? '').trim();

        let params = new HttpParams()
            .set('page', String(page))
            .set('pageSize', String(pageSize))
            .set('sort', sort)
            .set('dir', dir);

        if (trimmedSearch.length) {
            params = params.set('search', trimmedSearch);
        }

        return this.http.get<PagedResult<Employee>>(this.base, { params });
    }

    get(id: number): Observable<Employee> {
        return this.http.get<Employee>(`${this.base}/${id}`);
    }

    create(dto: Partial<Employee>): Observable<Employee> {
        return this.http.post<Employee>(this.base, dto);
    }

    update(id: number, dto: Partial<Employee>): Observable<Employee> {
        return this.http.put<Employee>(`${this.base}/${id}`, dto);
    }

    delete(id: number, rowVersionBase64: string): Observable<void> {
        const params = new HttpParams().set('rowVersionBase64', rowVersionBase64);
        return this.http.delete<void>(`${this.base}/${id}`, { params });
    }
}
