import { EventEmitter, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { Observable, Subject } from 'rxjs';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { environment } from 'src/environments/environment';
import { DirectorStatusMessage } from '../_models/Hub_Messages/DirectorStatusMessage';
import { ForwardLogMessage } from '../_models/Hub_Messages/ForwardLogMessage';
import { JobStatusMessage } from '../_models/Hub_Messages/JobStatusMessage';
import { LoggedUser } from '../_models/user';


@Injectable({
  providedIn: 'root'
})
export class HubService {

  hubUrl  = environment.hubUrl;
  private hubConnection: HubConnection;
  public hubConnectionStatus: BehaviorSubject<boolean>= new BehaviorSubject<boolean>(false);

  

  //Hub events
  public hubClosedEvent =new Subject();
  public receivedNewLogEvent =new Subject<ForwardLogMessage>();
  public receivedNewJobStatusEvent =new Subject<JobStatusMessage>();
  public receivedNewDirectorStatusEvent =new Subject<DirectorStatusMessage>();
  public userOnLineEvent =new Subject();
  public userOffLineEvent =new Subject();


  constructor(private toastr: ToastrService) 
  {
    this.hubConnectionStatus = new BehaviorSubject<boolean>(false);
  }

  public async createHubConnection(user: LoggedUser)
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
        //Hub is connected
        this.hubConnectionStatus.next(true)
            
        this.hubConnection?.onreconnecting(()=>
        {this.toastr.warning("Hub connection lost: trying to reconnect to server...");});

        this.hubConnection?.onreconnected(()=>
        {
          this.toastr.info("Hub connection restored");
          this.hubSend("UserAppLogIn", user.username);
        });

        this.hubConnection?.onclose(()=>
        {
          this.hubConnectionStatus.next(false);
          this.hubClosedEvent.next();
        });

        //Notify to Hub user has logged in (to pass user name)
        this.hubSend("UserAppLogIn", user.username);


        //Add other services events 
        this.hubConnection?.on("JobStatusMessage",(data:JobStatusMessage) => 
        {this.receivedNewJobStatusEvent.next(data);});
        this.hubConnection?.on("ForwardLogMessage",(data:ForwardLogMessage) => 
        {this.receivedNewLogEvent.next(data);});
        this.hubConnection?.on("DirectorStatusMessage",(data:DirectorStatusMessage) => 
        {this.receivedNewDirectorStatusEvent.next(data);});
        this.hubConnection?.on("UserIsOnLine", username => 
        {this.userOnLineEvent.next(username);})
        this.hubConnection?.on("UserIsOffLine", username => 
        {this.userOffLineEvent.next(username);})

      })
      .catch(error => 
        {
          console.log(error);
          this.toastr.error("Failed connection to Hub");
          throw(error);
        });

  }

  //Called at user log out
  public stopHubConnection(user: LoggedUser)
  {
    //Notify to Hub user has connected
    this.hubSend("UserAppLogOff", user.username)
    .then(()=>
    {
      this.hubConnection?.stop();
      this.hubConnectionStatus.next(false);
    })
    .catch((error=> console.log(error)))
  }


  public hubSend(methodName:string, args:any):Promise<void>
  {
    return this.hubConnection?.send(methodName, args).catch(error => console.log(error));
  }





}
