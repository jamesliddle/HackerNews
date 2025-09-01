import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class StoryService {
  constructor(private http: HttpClient) {}

  getNewest(query: string, page: number, pageSize: number): Observable<any> {
    const params = new URLSearchParams();
    params.set('page', String(page));
    params.set('pageSize', String(pageSize));
    if (query?.trim()) params.set('query', query.trim());
    const url = `${environment.apiUrl}/stories?${params.toString()}`;
    return this.http.get<any>(url);
  }
}
