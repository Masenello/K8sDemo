import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { DemoService } from './services/demoservice';
import { User } from './_models/user';
import { AccountService } from './services/account.service';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { JobService } from './services/job.service';
import { TestJobCreationRequest } from './_models/TestJobCreationRequest';
import { HubService } from './services/hub.service';
import {LogMessage, LogMessage as NgxLogMessage} from 'ngx-log-monitor';
import { Observable, timer } from 'rxjs';
import { map, take } from 'rxjs/operators';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'K8sDemoApp';
  response = "No data loaded, yet";
  baseUrl = environment.apiUrl;

  logmessage:LogMessage;

  constructor(private http: HttpClient, 
    private demoService: DemoService, 
    public accountService: AccountService, 
    private toastr: ToastrService,
    private jobService: JobService,
    private hubservice: HubService) 
  { 


    this.http.get(this.baseUrl + "DemoDatabase/GetTestData", {responseType: 'text'}).subscribe((response: any) => 
    {
      console.log(response);
	    this.response = response;	
	  });

    this.hubservice.logMessages.subscribe((response: LogMessage) =>{
      this.logmessage=response;
    });

  }  

  ngOnInit(){
    //When application is loaded try to restore current user 
    //if existing user data is present in localstorage
    this.accountService.tryRestoreCurrentUser();
  }

  sendRabbitMessageApi() {
    this.demoService.sendDemoRabbitMessage().subscribe(result =>{
      this.toastr.info("Message published!");
    },error=>{
      console.log(error);
      this.toastr.error(error.error);
    });
  }

  sendTestJobRequest() {
    let jobRequest:TestJobCreationRequest =  
    {
      user:this.accountService.currentUser.getValue()?.username!,
      requestDateTime: new Date(),
      requestJobType:0
    };
    this.jobService.sendTestJobRequest(jobRequest).subscribe(result =>{
      this.toastr.info(`Job with id ${result.jobId} created by user ${result.user}`);
    },error=>{
      console.log(error);
      this.toastr.error(error.error);
    });
  }

 
  enableLogview() {
    this.hubservice.sendEnableLogView();
  }

  disableLogview() {
    this.hubservice.sendDisableLogView();
  }

  restoredLogs: NgxLogMessage[] = [
    {message: 'A simple restored log message'},
    {message: 'A success restored message', type: 'SUCCESS'},
    {message: 'A warning restored message', type: 'WARN'},
    {message: 'An error restored message', type: 'ERR'},
    {message: 'An info restored message', type: 'INFO'},
  ];

  logs: NgxLogMessage[] = [
    {message: 'A simple log message'},
    {message: 'A success message', type: 'SUCCESS'},
    {message: 'A warning message', type: 'WARN'},
    {message: 'An error message', type: 'ERR'},
    {message: 'An info message', type: 'INFO'},
  ];

  // logStream$ = timer(0, 1000).pipe(
  //   take(this.logs.length),
  //   map(i => this.logs[i])
  // );




}
