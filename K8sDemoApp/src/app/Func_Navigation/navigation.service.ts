import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../Func_Login/account.service';

@Injectable({
  providedIn: 'root'
})
export class NavigationService {

  constructor(private router: Router) {

  }

  public navigate(pageName: string, state?: any): void {
    console.log(`Navigating to ${pageName}`)
    this.router.navigate([pageName], { state: state });
  }

  public navigateByQuery(pageName: string, queryParams: any): void {
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() =>
    this.router.navigate([pageName], { queryParams: queryParams }));
  }
}
