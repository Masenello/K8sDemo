import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AccountService } from '../services/account.service';
import { User } from '../_models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}
  currentUser = new Observable<User>();

  constructor(public accountService: AccountService, private toastr:ToastrService) { }

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
  }

}
