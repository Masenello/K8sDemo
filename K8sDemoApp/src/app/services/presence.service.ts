import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})

export class PresenceService {

  public connectionStatus: BehaviorSubject<boolean>;

  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection | undefined;

  constructor(private toastr: ToastrService) 
  {
    this.connectionStatus = new BehaviorSubject<boolean>(false);
  }

  createHubConnection(user: User)
  {
    //Create connection
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + "presence", {
      accessTokenFactory:()=>user.token})
      .withAutomaticReconnect()
      .build()
    
    //Start connection and register to messages
    this.hubConnection
      .start().then(()=>{

        this.connectionStatus.next(true)
            
        this.hubConnection?.on("UserIsOnLine", username => {
              this.toastr.info(username + " has connected");
            })

        this.hubConnection?.on("UserIsOffLine", username => {
              this.toastr.warning(username + " has disconnected");
            })

        this.hubConnection?.on("UserIsOffLine", username => {
              this.toastr.warning(username + " has disconnected");
            })

        this.hubConnection?.onreconnecting(()=>
        {
          this.manageHubDisconnection();
        });

        //Notify to Hub user has logged in
        this.hubConnection?.send("UserAppLogIn", user.username).catch(error=>console.log(error));

      

      }).catch(error => console.log(error));

  }


  manageHubDisconnection()
  {
    this.connectionStatus.next(false)
    this.toastr.error("SignalR hub has disconnected, user will be logged out");
    //this.account.logout;
  }


  stopHubConnection(user: User)
  {
    //Notify to Hub user has connected
    this.hubConnection?.send("UserAppLogOff", user.username)
    .then(()=>
    {
      this.hubConnection?.stop();
      this.connectionStatus.next(true)
    })
    .catch((error=> console.log(error)))
  }
}
