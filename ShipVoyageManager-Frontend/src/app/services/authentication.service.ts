import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { IAuthenticationRequest } from '../models/authentication/authenticationRequest.model';
import { loginSuccess, logoutSuccess } from '../state/authentication/auth.actions';


@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  private _url = 'Authentication';
  private options = {
    responseType: 'text' as 'json',
  };

  constructor(
    private http: HttpClient,
    private route: Router,
  ) { }

  public login(userAuthentication: IAuthenticationRequest){
    return this.http
      .post<string>(`${environment.apiUrl}/${this._url}/Login`, userAuthentication, this.options);
  }

  public isAuthenticated(): Observable<string> {
    return this.http
      .get<string>(`${environment.apiUrl}/${this._url}/GetCurrentUser`, this.options);
  }

  public logout(){
    return this.http.post<any>(`${environment.apiUrl}/${this._url}/Logout`, {});
  }
}
