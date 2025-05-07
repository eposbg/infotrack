import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchQuery, SearchResult } from './models';
import { environment } from '../../environments/environment';

@Injectable()
export class RankingService {
  loading = false;
  constructor(private httpClient: HttpClient) {}

  search(searchQuery: SearchQuery): Observable<SearchResult> {
    return this.httpClient.post<SearchResult>(
      `${environment.apiUrl}/ranking`,
      searchQuery
    );
  }
}
