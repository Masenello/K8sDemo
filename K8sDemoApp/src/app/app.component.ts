import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
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
    public accountService: AccountService, 
    private toastr: ToastrService,
    private navigationService : NavigationService) 
  { 

  }  

  ngOnInit(){
    this.navigationService.navigate("login");
  }

  prepareRoute(outlet: RouterOutlet) {
    return outlet?.activatedRouteData;
  }

  
}
