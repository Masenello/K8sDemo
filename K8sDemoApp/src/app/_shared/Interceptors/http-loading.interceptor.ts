import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/internal/operators/finalize';
import { LoadingPopUpManagerService } from '../Services/loading-pop-up-manager.service';

@Injectable()
export class HttpLoadingInterceptor implements HttpInterceptor {

  constructor(private loadingPopUpManagerService : LoadingPopUpManagerService) {}
  
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.loadingPopUpManagerService.show({ message: 'Loading data' , msStartDelay: 250});
    return next.handle(request).pipe(finalize(() => this.loadingPopUpManagerService.hide()));
  }
}
