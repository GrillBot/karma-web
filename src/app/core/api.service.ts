import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { KarmaResult } from './models';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
    constructor(
        private httpClient: HttpClient
    ) { }

    getKarmaData(orderBy: string, descending: boolean, page: number, pageSize: number) {
        const data = {
            sort: {
                orderBy: orderBy,
                descending: descending
            },
            pagination: {
                page: page,
                pageSize: pageSize
            }
        };
        const url = `${environment.api}/users/karma`;
        return this.httpClient.post<KarmaResult>(url, data, { headers: { 'ApiKey': environment.client_id } });
    }
}
