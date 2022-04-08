import { HttpClient } from '@angular/common/http';
import { CompileShallowModuleMetadata } from '@angular/compiler';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators'
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  currentUser : BehaviorSubject<User | null>;
  userPressedLogOutButton:boolean = false;

  hubUrl  = environment.hubUrl;
  private hubConnection: HubConnection | undefined;
  public hubConnectionStatus: BehaviorSubject<boolean>;

  constructor(private http: HttpClient, private toastr: ToastrService) {
    this.currentUser = new BehaviorSubject<User | null>(null); 
    this.hubConnectionStatus = new BehaviorSubject<boolean>(false);

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
      await this.createHubConnection(user);
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

  private logout()
  {
    var savedUser = this.getStoredUserData();
    if (savedUser != null)
    {
      this.stopHubConnection(savedUser);
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

  private async createHubConnection(user: User)
  {
    //Create connection
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + "client", {
      accessTokenFactory:()=>user.token})
      .withAutomaticReconnect()
      .build()
    
    //Start connection and register to messages. This part is async ...
    await this.hubConnection
      .start().then(()=>
      {

        this.hubConnectionStatus.next(true)
            
        this.hubConnection?.on("UserIsOnLine", username => 
        {
              this.toastr.info(username + " has connected");
            })

        this.hubConnection?.on("UserIsOffLine", username => 
        {
              this.toastr.warning(username + " has disconnected");
            })

        this.hubConnection?.on("UserIsOffLine", username => 
        {
              this.toastr.warning(username + " has disconnected");
            })

        this.hubConnection?.onreconnecting(()=>
        {
          this.toastr.warning("Hub connection lost: trying to reconnect to server...");
        });

        this.hubConnection?.onreconnected(()=>
        {
          this.toastr.info("Hub connection restored");
          this.hubConnection?.send("UserAppLogIn", user.username).catch(error=>console.log(error));
        });



        this.hubConnection?.onclose(()=>
        {
          this.manageHubDisconnection();
        });

        //Notify to Hub user has logged in (to pass user name)
        
        this.hubConnection?.send("UserAppLogIn", user.username).catch(error=>console.log(error));

      })
      .catch(error => 
        {
          console.log(error);
          this.toastr.error("Failed connection with Hub");
          throw(error);
        });

  }

  private manageHubDisconnection()
  {
    this.hubConnectionStatus.next(false);
    if (!this.userPressedLogOutButton)
    {
      this.toastr.error("Hub has disconnected, user will be logged out");
      this.logout();
    }
    this.userPressedLogOutButton = false; 
  }

  private stopHubConnection(user: User)
  {
    //Notify to Hub user has connected
    this.hubConnection?.send("UserAppLogOff", user.username)
    .then(()=>
    {
      this.hubConnection?.stop();
      this.hubConnectionStatus.next(false);
    })
    .catch((error=> console.log(error)))
  }

}
