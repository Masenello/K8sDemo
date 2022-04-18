import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from './account.service';

@Injectable()
export class AuthorizationInterceptor implements HttpInterceptor {

  constructor(public accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
       // add authorization header with jwt token if available
       let currentUser = this.accountService.currentUser.value;
       if (currentUser && currentUser.token) 
       {
        console.log(`Sending user token ${currentUser.token}`)
         request = request.clone({
           setHeaders: {
             Authorization: `Bearer ${currentUser.token}`,
           }
         });
         

    }
    
  return next.handle(request);
  }
}
