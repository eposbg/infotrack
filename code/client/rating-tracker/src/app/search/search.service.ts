import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { SearchQuery } from './models';

@Injectable()
export class SearchService {
  loading = false;
  constructor(private httpClient: HttpClient) {}

  search(searchQuery: SearchQuery): Observable<any> {
    return this.httpClient.post(`${environment.apiUrl}/search`, searchQuery);
  }
}
