import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { environment } from 'src/environments/environment';
import { UserDto } from '../_models/API_Messages/UserDto';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  
  baseUrl = environment.apiUrl + "Users/";

  constructor(private http: HttpClient) { }

  
  getUsers(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(this.baseUrl + 'GetUsers');
  }

  deleteUser(username:string): Observable<any> {
    return this.http.delete(this.baseUrl +"DeleteUser/"+ username);
  }
  
}
