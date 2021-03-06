import { HttpClient } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, ReplaySubject, Subject } from 'rxjs';
import {map} from 'rxjs/operators'
import { environment } from 'src/environments/environment';
import { LoginRequest} from '../_models/user';
import { HubService } from '../services/hub.service';
import jwt_decode from 'jwt-decode';
import { UserDto } from '../_models/API_Messages/UserDto';
import { RoleDto } from '../_models/API_Messages/RoleDto';
import { RoleEnum } from '../_enum/RoleEnum';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  currentUser : BehaviorSubject<UserDto | null>;
  public userPressedLogOutButton:boolean = false;
  public userLoggedIn =new Subject<UserDto>();
  public userLoggedOut =new Subject();



  constructor(private http: HttpClient, 
    private toastr: ToastrService,
    private hubService: HubService) 
    {
      this.currentUser = new BehaviorSubject<UserDto | null>(null);
      
      this.hubService.hubClosedEvent.subscribe(()=>{this.manageHubClosed();});

      this.hubService.userOnLineEvent.subscribe((username)=>{
        this.toastr.info(username + " has connected");});

      this.hubService.userOffLineEvent.subscribe((username)=>{
        this.toastr.warning(username + " has disconnected");});

    }

  login(request: LoginRequest)
  {
      return this.http.post<UserDto>(this.baseUrl + "account/login", request).subscribe(loggedUser=>{
        console.log("User " + loggedUser.username + " logged in")
        //console.log(loggedUser)
        localStorage.setItem("user", JSON.stringify(loggedUser))
        this.setCurrentUser(loggedUser)
        this.userLoggedIn.next(loggedUser)
        this.toastr.success(`Welcome ${loggedUser.username}`)
      }, error =>{
        console.log(error)
        this.toastr.error("Log in failed")
      })
    
  }

  //Called also on application init to restore user data if present in local storage
  private async setCurrentUser(user:UserDto)
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

  private getStoredUserData():UserDto | null
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
  public getTokenInformation(token: string) : UserDto
  {


    let tokenInfo = this.getDecodedJWT(token);
    let loggedUser:UserDto;

    loggedUser.roles = tokenInfo.role;
    loggedUser.username = tokenInfo.unique_name;
    loggedUser.token = token;

    return loggedUser;

  }

  // public get roles(): typeof RoleEnum {
  //   return RoleEnum;
  // }

  public isInRole(role: RoleEnum): boolean {
    //get user roles from token role1,role2,...
    if (this.currentUser == undefined) return false
    let allTokenRoles : string 
    let tokenRoles:string[]
    try {
      allTokenRoles= this.getDecodedJWT(this.currentUser?.value.token).role
      tokenRoles = allTokenRoles.split(",")
      for (let i=0; i<tokenRoles.length; i++ )
      {
        if (tokenRoles[i] == RoleEnum[role])
        {
          //console.log(`User: ${this.currentUser?.value.username} has role ${RoleEnum[role]}`)
          return true;
        }
      }
      return false
    } catch (error) {
      //console.error(`No roles defined for current user`)
      return false
    }
  }

  public hasRole(requestedRoles: RoleEnum[]): boolean {
    //If no role is specified any role is ok
    if (requestedRoles == undefined) return true
    //Check if any of the requested roles are active for user
    for (let i=0; i<requestedRoles.length; i++)
    {
      if (this.isInRole(requestedRoles[i]))
      {
        return true
      }
    }
    return false
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
