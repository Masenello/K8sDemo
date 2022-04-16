import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { NavigationService } from '../Func_Navigation/navigation.service';
import { LoginRequest } from '../_models/user';
import { AccountService } from './account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  
  user = <LoginRequest>{};
  hidePassword = true;

  constructor(private accountService:AccountService,
    private toastr:ToastrService) {

    }

  ngOnInit(): void {

  }

  onSubmit(){
    console.log("Request log in user: " + this.user.username + "password: "+ this.user.password )
    this.accountService.login(this.user).subscribe(response => {
      console.log(response);
    }, error=>
    {
      console.log(error)
      this.toastr.error("Login failed");
    });
    
  }

}
