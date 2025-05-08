import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Guid } from 'guid-typescript';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { IVoyage } from '../models/voyage/voyage.model';
import { IPaginatedVoyages } from '../models/voyage/paginatedVoyages.model';

@Injectable({
  providedIn: 'root'
})
export class VoyageService {
  private _url = 'Voyage';

  constructor(private http: HttpClient) { }

  public getFilteredVoyagesPaginated(page: number, pageSize: number): Observable<IPaginatedVoyages> {
    return this.http.get<IPaginatedVoyages>(`${environment.apiUrl}/${this._url}/GetFilteredVoyagesPaginated?page=${page}&pageSize=${pageSize}`);
  }

  public addVoyage(voyage: IVoyage): Observable<any> {
    return this.http.post<IVoyage>(`${environment.apiUrl}/${this._url}/AddVoyage`, voyage);
  }

  public updateVoyage(voyage: IVoyage): Observable<IVoyage> {
    return this.http.put<IVoyage>(`${environment.apiUrl}/${this._url}/UpdateVoyage`, voyage);
  }

  public deleteVoyage(id: Guid): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/${this._url}/DeleteVoyage?id=${id}`);
  }
}
