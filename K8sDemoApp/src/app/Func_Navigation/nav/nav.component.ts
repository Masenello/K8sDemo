import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AccountService } from '../../Func_Login/account.service';
import { LoggedUser } from '../../_models/user';
import { MenuColor, MenuEntry, MenuIconType, MenuLocation } from '../menu-entry.model';
import { NavigationService } from '../navigation.service';
import { appMenus } from '../../Func_Navigation/nav-menus';
import { environment } from 'src/environments/environment';



@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}

  navBarOpen: boolean;
  menus: MenuEntry[];

  guiVersion: string = environment.appVersion
  

  constructor(public accountService: AccountService, 
    private toastr:ToastrService,
    public navigationService:NavigationService) {

    this.menus = appMenus;

    this.accountService.userLoggedIn.subscribe((user:any) =>{
      this.navigationService.navigate("");
      this.navBarOpen = true;
    });

    this.accountService.userLoggedOut.subscribe((user:any) =>{
      this.navigationService.navigate("login");
      this.navBarOpen = false;
    });

    }

  ngOnInit(): void {
    //When application is loaded try to restore current user 
    //if existing user data is present in localstorage
    this.accountService.tryRestoreCurrentUser()
  }


  logout(){
    this.accountService.userLogOutCommand();
  }




}
