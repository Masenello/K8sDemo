import { HttpClient } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, ReplaySubject, Subject } from 'rxjs';
import {map} from 'rxjs/operators'
import { environment } from 'src/environments/environment';
import { LoginRequest, LoggedUser } from '../_models/user';
import { HubService } from '../services/hub.service';
import { RoleEnum } from '../_enum/RoleEnum';
import jwt_decode from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  currentUser : BehaviorSubject<LoggedUser | null>;
  public userPressedLogOutButton:boolean = false;
  public userLoggedIn =new Subject<LoggedUser>();
  public userLoggedOut =new Subject();



  constructor(private http: HttpClient, 
    private toastr: ToastrService,
    private hubService: HubService) 
    {
      this.currentUser = new BehaviorSubject<LoggedUser | null>(null);
      
      this.hubService.hubClosedEvent.subscribe(()=>{this.manageHubClosed();});

      this.hubService.userOnLineEvent.subscribe((username)=>{
        this.toastr.info(username + " has connected");});

      this.hubService.userOffLineEvent.subscribe((username)=>{
        this.toastr.warning(username + " has disconnected");});

    }

  login(request: LoginRequest)
  {
      return this.http.post(this.baseUrl + "account/login", request).pipe(
        map((response: any)=>{
          let user:LoggedUser = response;
          if (user)
          {
            //TODO Remove this!
            user.roles =[]
            user.roles[0] = RoleEnum.Admin

            console.log("User " + user.username + " logged in");
            localStorage.setItem("user", JSON.stringify(user));
            this.setCurrentUser(user);
            this.userLoggedIn.next(user);
          }
        }))
  }

  //Called also on application init to restore user data if present in local storage
  private async setCurrentUser(user:LoggedUser)
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

  //Called when user presses logout button
  public userLogOutCommand()
  {
    this.userPressedLogOutButton = true;
    this.logout();
  }

  private logout()
  {
    var savedUser = this.getStoredUserData();
    if (savedUser != null)
    {
      this.hubService.stopHubConnection(savedUser);
    }
    this.clearStoredUserData();
    this.userLoggedOut.next();
  }

  public tryRestoreCurrentUser():boolean
  {
    var savedUser = this.getStoredUserData()
    if (savedUser != null)
    {
      console.log("DEBUG: Loaded user from localstorage: " + savedUser.username);
      this.setCurrentUser(savedUser);
      this.userLoggedIn.next(savedUser);
      return true;
    }
    return false;
  }

  private getStoredUserData():LoggedUser | null
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


  //Evaluate if Hub closed is intentional (baecause user logged out) or unexpected (hub disconnection)
  private manageHubClosed()
  {
    if (!this.userPressedLogOutButton)
    {
      this.toastr.error("Hub has disconnected, user will be logged out");
      this.logout();
    }
    this.userPressedLogOutButton = false; 
  }


  //Roles management
  public getTokenInformation(token: string) : LoggedUser
  {


    let tokenInfo = this.getDecodedJWT(token);
    let loggedUser:LoggedUser;

    loggedUser.roles = tokenInfo.role;
    loggedUser.username = tokenInfo.unique_name;
    loggedUser.token = token;

    return loggedUser;

  }

  public get roles(): typeof RoleEnum {
    return RoleEnum;
  }

  public isInRole(role: RoleEnum): boolean {
    return this.currentUser?.value.roles?.includes(role);
  }

  public hasRole(roles: RoleEnum[]): boolean {
    return !roles || roles.filter(x => this.currentUser?.value.roles?.includes(x)).length > 0;
  }

  public isTokenExpired(): boolean {
    return this.currentUser?.value.token && this.getDecodedJWT(this.currentUser.value.token)?.exp < (Math.floor((new Date).getTime() / 1000));
  }

  getDecodedJWT(token: string): any {
    try {
      return jwt_decode(token);
    } catch(Error) {
      return null;
    }
  }
  
}
