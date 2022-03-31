import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {

  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection | undefined;
  constructor(private toastr: ToastrService) { }

  createHubConnection(user: User){
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + "presence", {
      accessTokenFactory:()=>user.token})
      .withAutomaticReconnect()
      .build()

      this.hubConnection
      .start().then(()=>{
             //Register to connected users update messages (notifications on other users connection)
            this.hubConnection?.on("UserIsOnLine", username => {
              this.toastr.info(username + " has connected");
            })

            this.hubConnection?.on("UserIsOffLine", username => {
              this.toastr.warning(username + " has disconnected");
            })
            
            //Notify to Hub user has connected
            this.hubConnection?.send("UserAppLogIn", user.username).catch(error=>console.log(error));

      }).catch(error => console.log(error));

  }



  stopHubConnection(user: User){
    //Notify to Hub user has connected
    this.hubConnection?.send("UserAppLogOff", user.username).then(()=>{
      this.hubConnection?.stop();
    }).catch((error=> console.log(error)))
    
  }
}
