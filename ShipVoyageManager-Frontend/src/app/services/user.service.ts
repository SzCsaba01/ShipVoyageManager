import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { IUserChangePassword } from '../models/user/userChangePassword.model';
import { IUserRegistration } from '../models/user/userRegistration.model';
import { IFilteredPaginatedUsers } from '../models/user/filteredPaginatedUsers.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private _url = 'User';

  constructor(private http: HttpClient) {}

  public getFilteredUsersPaginated(page: number, pageSize: number, searchTerm: string) {
    return this.http.get<IFilteredPaginatedUsers>(
      `${environment.apiUrl}/${this._url}/GetFilteredUsersPaginated?page=${page}&pageSize=${pageSize}&search=${searchTerm}`
    );
  }

  public verifyIfResetPasswordTokenExists(
    resetPasswordToken: string
  ): Observable<boolean> {
    return this.http.get<boolean>(
      `${environment.apiUrl}/${this._url}/VerifyIfResetPasswordTokenExists?resetPasswordToken=${resetPasswordToken}`
    );
  }

  public changePassword(userChangePassword: IUserChangePassword) {
    return this.http.put(
      `${environment.apiUrl}/${this._url}/ChangePassword`,
      userChangePassword
    );
  }

  public sendResetPasswordEmail(email: string) {
    return this.http.put(
      `${environment.apiUrl}/${this._url}/SendResetPasswordEmail?email=${email}`,
      {}
    );
  }

  public verifyEmailByRegistrationToken(registrationToken: string) {
    const encodedToken = encodeURIComponent(registrationToken);
    return this.http.put(
      `${environment.apiUrl}/${this._url}/VerifyEmailByRegistrationToken?registrationToken=${encodedToken}`,
      {}
    );
  }

  public register(user: IUserRegistration) {
    return this.http.post(
      `${environment.apiUrl}/${this._url}/Register`,
      user
    );
  }

  public deleteUser(username: string) {
    return this.http.delete(
      `${environment.apiUrl}/${this._url}/DeleteUser?username=${username}`
    );
  }
}
