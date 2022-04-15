import { EventEmitter, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { LogMessage } from 'ngx-log-monitor';
import { ToastrService } from 'ngx-toastr';
import { Observable, Subject } from 'rxjs';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { environment } from 'src/environments/environment';
import { JobStatusEnum } from '../_enum/JobStatusEnum';
import { ForwardLogMessage } from '../_models/ForwardLogMessage';
import { JobStatusMessage } from '../_models/JobStatusMessage';
import { User } from '../_models/user';
import { LogUtils } from '../_shared/Utils/LogUtils';

@Injectable({
  providedIn: 'root'
})
export class HubService {

  hubUrl  = environment.hubUrl;
  private hubConnection: HubConnection | undefined;
  public hubConnectionStatus: BehaviorSubject<boolean>;

  

  //Hub events
  public logoutRequestEvent =new EventEmitter();
  public receivedNewLogEvent =new EventEmitter();
  public receivedNewJobStatusEvent =new EventEmitter();


  constructor(private toastr: ToastrService) 
  {
    this.hubConnectionStatus = new BehaviorSubject<boolean>(false);
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

    

        //Notify to Hub user has logged in (to pass user name)
        
        this.hubConnection?.send("UserAppLogIn", user.username).catch(error=>console.log(error));


        this.hubConnection?.on("JobStatusMessage",(data:JobStatusMessage) => 
        { this.receivedNewJobStatusEvent.emit(data);});
        this.hubConnection?.on("ForwardLogMessage",(data:ForwardLogMessage) => 
        {this.receivedNewLogEvent.emit(data);});

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
    this.logoutRequestEvent.emit();
  }

  public hubSend(methodName:string, args:any)
  {
    this.hubConnection?.send(methodName, args).catch(error=>console.log(error));
  }





}
