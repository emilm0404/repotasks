export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    sort?: string;
    dir?: 'asc' | 'desc';
}
