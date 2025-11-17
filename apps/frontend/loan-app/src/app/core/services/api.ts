import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/v1`;

  get<T>(path: string) { return this.http.get<T>(`${this.url}/${path}`); }
  post<T>(path: string, body: any) { return this.http.post<T>(`${this.url}/${path}`, body); }
  put<T>(path: string, body: any) { return this.http.put<T>(`${this.url}/${path}`, body); }
  delete<T>(path: string) { return this.http.delete<T>(`${this.url}/${path}`); }
}
