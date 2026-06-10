import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CityEdge, CityNode, RouteRequest, RouteResponse } from '../models/city.models';

@Injectable({
  providedIn: 'root'
})
export class RouteService {
  private readonly apiUrl = 'http://localhost:5131/api';

  constructor(private readonly http: HttpClient) {}

  getNodes(): Observable<CityNode[]> {
    return this.http.get<CityNode[]>(`${this.apiUrl}/route/nodes`);
  }

  getEdges(): Observable<CityEdge[]> {
    return this.http.get<CityEdge[]>(`${this.apiUrl}/route/edges`);
  }

  calculateRoutes(request: RouteRequest): Observable<RouteResponse[]> {
    return this.http.post<RouteResponse[]>(`${this.apiUrl}/route/calculate`, request);
  }
}