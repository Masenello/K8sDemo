import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AccountService } from '../../Func_Login/account.service';
import { User } from '../../_models/user';
import { NavigationService } from '../navigation.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}
  currentUser = new Observable<User>();

  constructor(public accountService: AccountService, 
    private toastr:ToastrService,
    private navigationService:NavigationService) { }

  ngOnInit(): void {
    this.currentUser = this.accountService.currentUser;
  }

  login(){
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
    }, error=>
    {
      console.log(error)
      this.toastr.error("Login failed");
    });
    
  }

  logout(){
    this.accountService.userLogOutCommand();
    this.navigationService.navigate("login");
  }

}
