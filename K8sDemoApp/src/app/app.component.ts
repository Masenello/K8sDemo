import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { DemoService } from './services/demoservice';
import { User } from './_models/user';
import { AccountService } from './services/account.service';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { JobService } from './services/job.service';
import { TestJobCreationRequest } from './_models/TestJobCreationRequest';


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




}
