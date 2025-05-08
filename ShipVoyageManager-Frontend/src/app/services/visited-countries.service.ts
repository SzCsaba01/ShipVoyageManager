import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IVisitedCountry } from '../models/visited-country/visitedCountry.model';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class VisitedCountriesService {
  private _url = 'VisitedCountries';

  constructor(private http: HttpClient) { }

  public getVisitedCountries(): Observable<IVisitedCountry[]>{
    return this.http.get<IVisitedCountry[]>(`${environment.apiUrl}/${this._url}/GetVisitedCountries`);
  }

}
