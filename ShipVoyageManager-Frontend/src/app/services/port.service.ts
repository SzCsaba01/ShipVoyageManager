import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Guid } from 'guid-typescript';
import { Observable } from 'rxjs';
import { IPort } from '../models/port/port.model';
import { IFilteredPaginatedPorts } from '../models/port/filteredPaginatedPorts.model';

@Injectable({
  providedIn: 'root'
})
export class PortService {
  private _url = 'Port';
  
  constructor(private http: HttpClient) { }

  public getFilteredPortsPaginated(page: number, pageSize: number, search: string): Observable<IFilteredPaginatedPorts> {
    return this.http.get<IFilteredPaginatedPorts>(`${environment.apiUrl}/${this._url}/GetFilteredPortsPaginated?page=${page}&pageSize=${pageSize}&search=${search}`);
  }

  public getAllPorts(): Observable<IPort[]> {
    return this.http.get<IPort[]>(`${environment.apiUrl}/${this._url}/GetAllPorts`);
  }

  public addPort(port: IPort): Observable<any> {
    return this.http.post<IPort>(`${environment.apiUrl}/${this._url}/AddPort`, port);
  }

  public updatePort(port: IPort): Observable<IPort> {
    return this.http.put<IPort>(`${environment.apiUrl}/${this._url}/UpdatePort`, port);
  }

  public deletePort(id: Guid): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/${this._url}/DeletePort?id=${id}`);
  }
}
