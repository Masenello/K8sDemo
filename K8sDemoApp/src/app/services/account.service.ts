import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators'
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presence: PresenceService) { }

  login(model: any){
    return this.http.post(this.baseUrl + "account/login", model).pipe(
      map((response: any)=>{
        const user = response;
        if (user){
          console.log("User " + user.username + " logged in");
          localStorage.setItem("user", JSON.stringify(user));
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
        }
      })
    )
  }

  setCurrentUser(user:User){
    this.currentUserSource.next(user);
  }

  logout(){
    var savedUser = localStorage.getItem("user");
    if (savedUser != null)
    {
      this.presence.stopHubConnection(<User>JSON.parse(savedUser));
    }
    localStorage.removeItem("user");
    this.currentUserSource.next(undefined);
    
  }
}
