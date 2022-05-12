import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { KarmaResult, UriBuilder } from './models';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
    constructor(
        private httpClient: HttpClient
    ) { }

    getKarmaData(orderBy: string, descending: boolean, page: number, pageSize: number) {
        const url = new UriBuilder(`${environment.api}/users/karma`)
            .withQueryParam('sort.orderBy', orderBy)
            .withQueryParam('sort.descending', descending ? 'true' : 'false')
            .withQueryParam('pagination.page', page.toString())
            .withQueryParam('pagination.pageSize', pageSize.toString())
            .toString();

        return this.httpClient.get<KarmaResult>(url, { headers: { 'ApiKey': environment.client_id } });
    }
}
