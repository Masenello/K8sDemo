import { Injectable } from '@angular/core';
import { HubService } from '../services/hub.service';
import { ForwardLogMessage } from '../_models/Hub_Messages/ForwardLogMessage';
import { LogUtils } from '../_shared/Utils/LogUtils';
import { Observable, Subject } from 'rxjs';
import { LogMessage } from 'ngx-log-monitor';
import { LogViewerPipe } from './log-viewer.pipe';
import { LogTypeEnum } from '../_enum/LogTypeEnum';

@Injectable({
  providedIn: 'root'
})
export class LogsviewerService {

  public logMessages : Subject<LogMessage> = new Subject<LogMessage> ();
  public historyMessages : Array<LogMessage> = new Array<LogMessage> ();

  constructor(private hub: HubService, private logPipe: LogViewerPipe) {
    this.hub.receivedNewLogEvent.subscribe((data:ForwardLogMessage)=>
    {
      this.addLog(data);
    });
  }

    public sendEnableLogView()
    {
      this.hub.hubSend("JoinGroup", "logviewers");
      this.addLog( {
        message: "User connected viewer to log stream",
        messageType: LogTypeEnum.Warning,
        program: "Application"
      })
    }

    public sendDisableLogView()
    {
      this.hub.hubSend("LeaveGroup", "logviewers");
      this.addLog( {
        message: "User disconnected viewer from log stream",
        messageType: LogTypeEnum.Warning,
        program: "Application"
      })
    }

    public addLog(log:ForwardLogMessage)
    {
      this.logMessages.next(this.logPipe.transform(log));
      this.historyMessages.push(this.logPipe.transform(log));
    }

}
