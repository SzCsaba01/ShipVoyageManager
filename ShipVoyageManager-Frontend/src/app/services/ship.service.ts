import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Guid } from 'guid-typescript';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { IShip } from '../models/ship/ship.model';
import { IFilteredPaginatedShips } from '../models/ship/filteredPaginatedShips.model';
import { IGetShipsOutOfDateRange } from '../models/ship/getShipsOutOfDateRange.model';

@Injectable({
  providedIn: 'root'
})
export class ShipService {
  private _url = 'Ship';

  constructor(private http: HttpClient) {}

  public getFilteredShipsPaginated(page: number, pageSize: number, search: string): Observable<IFilteredPaginatedShips> {
    return this.http.get<IFilteredPaginatedShips>(`${environment.apiUrl}/${this._url}/GetFilteredShipsPaginated?page=${page}&pageSize=${pageSize}&search=${search}`);
  }

  public getShipsOutOfDateRange(requestBody: IGetShipsOutOfDateRange): Observable<IShip[]> {
    return this.http.put<IShip[]>(`${environment.apiUrl}/${this._url}/GetShipsOutOfDateRange`, requestBody);
  }

  public addShip(ship: IShip): Observable<any> {
    return this.http.post<IShip>(`${environment.apiUrl}/${this._url}/AddShip`, ship);
  }

  public updateShip(ship: IShip): Observable<IShip> {
    return this.http.put<IShip>(`${environment.apiUrl}/${this._url}/UpdateShip`, ship);
  }

  public deleteShip(id: Guid): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/${this._url}/DeleteShipById?id=${id}`);
  }
}
