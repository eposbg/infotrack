import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable()
export class SearchService {
    loading = false;
    constructor(private httpClient: HttpClient) { }
    
    search(keyword: string, targetDomain: string) {
        return this.httpClient.get(`https://api.example.com/search?keyword=${keyword}&domain=${targetDomain}`);
    }

}