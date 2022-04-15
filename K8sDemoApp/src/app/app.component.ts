import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { DemoService } from './services/demoservice';
import { User } from './_models/user';
import { AccountService } from './Func_Login/account.service';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { JobService } from './Func_Jobs/job.service';
import { TestJobCreationRequest } from './_models/API_Messages/TestJobCreationRequest';
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



  constructor(private http: HttpClient, 
    private demoService: DemoService, 
    public accountService: AccountService, 
    private toastr: ToastrService,
    private jobService: JobService) 
  { 


    this.http.get(this.baseUrl + "DemoDatabase/GetTestData", {responseType: 'text'}).subscribe((response: any) => 
    {
      console.log(response);
	    this.response = response;	
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

  
}
