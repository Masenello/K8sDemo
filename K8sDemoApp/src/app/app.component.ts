import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { DemoService } from './services/demoservice';
import { AccountService } from './Func_Login/account.service';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { NavigationService } from './Func_Navigation/navigation.service';
import { RouterOutlet } from '@angular/router';



@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'K8sDemoApp ';
  response = "No data loaded, yet";
  baseUrl = environment.apiUrl;



  constructor(private http: HttpClient, 
    private demoService: DemoService, 
    public accountService: AccountService, 
    private toastr: ToastrService,
    private navigationService : NavigationService) 
  { 


    this.http.get(this.baseUrl + "DemoDatabase/GetTestData", {responseType: 'text'}).subscribe((response: any) => 
    {
      console.log(response);
	    this.response = response;	
	  });



  }  

  ngOnInit(){
    this.navigationService.navigate("login");
  }

  sendRabbitMessageApi() {
    this.demoService.sendDemoRabbitMessage().subscribe(result =>{
      this.toastr.info("Message published!");
    },error=>{
      console.log(error);
      this.toastr.error(error.error);
    });
  }

  prepareRoute(outlet: RouterOutlet) {
    return outlet?.activatedRouteData;
  }

  
}
