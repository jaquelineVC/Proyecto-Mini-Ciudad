import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IncidentRequest, IncidentResponse } from '../models/city.models';

@Injectable({
  providedIn: 'root'
})
export class IncidentService {
  private readonly apiUrl = 'http://localhost:5131/api';

  constructor(private readonly http: HttpClient) {}

  getActiveIncidents(): Observable<IncidentResponse[]> {
    return this.http.get<IncidentResponse[]>(`${this.apiUrl}/incident`);
  }

  reportIncident(request: IncidentRequest): Observable<IncidentResponse> {
    return this.http.post<IncidentResponse>(`${this.apiUrl}/incident`, request);
  }

  removeIncident(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/incident/${id}`);
  }

  getStreetNames(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/incident/streets`);
  }
}