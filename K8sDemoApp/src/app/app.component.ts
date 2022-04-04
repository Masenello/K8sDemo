import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { DemoService } from './services/demoservice';
import { User } from './_models/user';
import { AccountService } from './services/account.service';
import { ToastrService } from 'ngx-toastr';
import { PresenceService } from './services/presence.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'K8sDemoApp';
  response = "No data loaded, yet";
  baseUrl = environment.apiUrl;

  user: any = undefined;



  constructor(private http: HttpClient, private demoService: DemoService, private accountService: AccountService, private toastr: ToastrService, private presence:PresenceService) 
  { 


    this.http.get(this.baseUrl + "DemoDatabase/GetTestData", {responseType: 'text'}).subscribe((response: any) => {
      console.log(response);
	  this.response = response;	

	});
  }  

  ngOnInit(){
    this.setCurrentUser();
  }

  setCurrentUser(){
    
    this.user = JSON.parse(localStorage.getItem('user')|| '{}');
    
    if (this.user == null){
      console.log("DEBUG: Loaded user from localstorage" + this.user.username);
      this.accountService.setCurrentUser(this.user);
      this.presence.createHubConnection(this.user);
    }

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
