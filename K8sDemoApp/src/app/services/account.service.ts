import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators'
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { HubService } from './hub.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  currentUser : BehaviorSubject<User | null>;
  public userPressedLogOutButton:boolean = false;



  constructor(private http: HttpClient, 
    private toastr: ToastrService,
    private hubService: HubService) 
    {
      this.currentUser = new BehaviorSubject<User | null>(null);
      
      this.hubService.logoutRequestEvent.subscribe((newValue)=>
      {
        //Evaluate if Hub closed is intentional (baecause user logged out) or unexpected (hub disconnection)
          this.evaluateUserLogOut();
      });

    }

  login(model: any)
  {
      return this.http.post(this.baseUrl + "account/login", model).pipe(
        map((response: any)=>{
          const user = response;
          if (user)
          {
            console.log("User " + user.username + " logged in");
            localStorage.setItem("user", JSON.stringify(user));
            this.setCurrentUser(user);
          }
        }))
  }

  //Called also on application init to restore user data if present in local storage
  private async setCurrentUser(user:User)
  {
    try
    {
      await this.hubService.createHubConnection(user);
      this.currentUser.next(user);
    }
    catch
    {
      this.toastr.error("Log in failed");
      this.clearStoredUserData()
    }
      
  }

  userLogOutCommand()
  {
    this.userPressedLogOutButton = true;
    this.logout();
  }

  public logout()
  {
    var savedUser = this.getStoredUserData();
    if (savedUser != null)
    {
      this.hubService.stopHubConnection(savedUser);
    }
    this.clearStoredUserData();
  }

  tryRestoreCurrentUser()
  {
    var savedUser = this.getStoredUserData()
    if (savedUser != null)
    {
      console.log("DEBUG: Loaded user from localstorage: " + savedUser.username);
      this.setCurrentUser(savedUser);
    }
  }

  private getStoredUserData():User | null
  {
    var savedUser = JSON.parse(localStorage.getItem('user')|| '{}');
    if (savedUser != null && savedUser.username != undefined)
    {
      return savedUser;
    }
    return null;
  }

  private clearStoredUserData()
  {
    localStorage.removeItem("user");
    this.currentUser.next(null);
  }

  private evaluateUserLogOut()
  {
    if (!this.userPressedLogOutButton)
    {
      this.toastr.error("Hub has disconnected, user will be logged out");
      this.logout();
    }
    this.userPressedLogOutButton = false; 
  }
  
}
