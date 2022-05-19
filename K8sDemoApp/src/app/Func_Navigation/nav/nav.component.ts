import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../../Func_Login/account.service';
import { NavigationService } from '../navigation.service';
import { appMenus } from '../../Func_Navigation/nav-menus';
import { environment } from 'src/environments/environment';
import { AwaiterInfo, LoadingPopUpManagerService } from 'src/app/_shared/Services/loading-pop-up-manager.service';
import { MenuEntry } from '../menu-entry.model';



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

  awaiterInfo: AwaiterInfo;


  constructor(public accountService: AccountService,
    private toastr: ToastrService,
    public navigationService: NavigationService,
    private loadingPopUpManagerService: LoadingPopUpManagerService) {

    this.menus = appMenus;

    this.accountService.userLoggedIn.subscribe((user: any) => {
      this.navigationService.navigate("home");
      this.navBarOpen = true;
    });

    this.accountService.userLoggedOut.subscribe((user: any) => {
      this.navigationService.navigate("login");
      this.navBarOpen = false;
    });

    this.loadingPopUpManagerService.awaiter.subscribe(info => {
      // setTimeout is necessary to avoid NG0100 error
      // https://blog.angular-university.io/angular-debugging/
      setTimeout(() => {
        this.awaiterInfo = info;
      })
    });

  }

  ngOnInit(): void {
    //When application is loaded try to restore current user 
    //if existing user data is present in localstorage
    this.accountService.tryRestoreCurrentUser()
  }


  logout() {
    this.accountService.userLogOutCommand();
  }




}
