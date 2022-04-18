import { Component, OnInit } from '@angular/core';
import { LogMessage } from 'ngx-log-monitor';
import { LogsviewerService } from '../logsviewer.service';

@Component({
  selector: 'app-logsviewer',
  templateUrl: './logsviewer.component.html',
  styleUrls: ['./logsviewer.component.css']
})
export class LogsviewerComponent implements OnInit {

  logmessage:LogMessage;
  
  constructor(public logViewerService:LogsviewerService) {

    this.logViewerService.logMessages.subscribe((response: LogMessage) =>{
      this.logmessage=response;
    });
  }

  ngOnInit(): void {
  }

  enableLogview() {
    this.logViewerService.sendEnableLogView();
  }

  disableLogview() {
    this.logViewerService.sendDisableLogView();
  }

}
