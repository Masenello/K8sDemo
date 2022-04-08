import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { environment } from 'src/environments/environment';
import { JobStatusEnum } from '../_enum/JobStatusEnum';
import { JobStatusMessage } from '../_models/JobStatusMessage';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class HubService {

  hubUrl  = environment.hubUrl;
  private hubConnection: HubConnection | undefined;
  public hubConnectionStatus: BehaviorSubject<boolean>;
  public logoutRequestEvent: BehaviorSubject<number>;
  public c_event :CustomEvent;


  constructor(private toastr: ToastrService) 
  {
    this.hubConnectionStatus = new BehaviorSubject<boolean>(false);
    this.logoutRequestEvent = new BehaviorSubject<number>(0);

    this.c_event = new CustomEvent("evaluateUserLogOut",{});
  }

  public async createHubConnection(user: User)
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

        this.hubConnection?.on("ReportJobProgress",(data:JobStatusMessage) => 
        {
          this.toastr.info(`Job id: ${data.jobId}: Status: ${JobStatusEnum[data.status]} Percentage: ${data.progressPercentage}`)
          console.log(data);
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

  public stopHubConnection(user: User)
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

  private manageHubDisconnection()
  {
    this.hubConnectionStatus.next(false);
    //Trigger a reuqest to evaluate the hub close event
    this.logoutRequestEvent.next(this.logoutRequestEvent.value + 1);
  }



}
