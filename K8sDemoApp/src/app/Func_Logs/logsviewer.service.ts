import { Injectable } from '@angular/core';
import { HubService } from '../services/hub.service';
import { ForwardLogMessage } from '../_models/ForwardLogMessage';
import { LogUtils } from '../_shared/Utils/LogUtils';
import { Observable, Subject } from 'rxjs';
import { LogMessage } from 'ngx-log-monitor';

@Injectable({
  providedIn: 'root'
})
export class LogsviewerService {

  public logMessages : Subject<LogMessage> = new Subject<LogMessage> ();

  constructor(private hub: HubService) {
    this.hub.receivedNewLogEvent.subscribe((data:ForwardLogMessage)=>
    {
      let logUtils : LogUtils = new LogUtils;    
      this.logMessages.next(
        {
          timestamp: logUtils.ExtractDateString(data.message),
          message: logUtils.ExtractMessage(data.message),
          type:logUtils.ConvertLogType(data.messageType)
        })
    });
  }

    public sendEnableLogView()
    {
      this.hub.hubSend("JoinGroup", "logviewers");
    }

    public sendDisableLogView()
    {
      this.hub.hubSend("LeaveGroup", "logviewers");
    }
}
