import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/internal/Subject';

@Injectable({
  providedIn: 'root'
})
export class LoadingPopUpManagerService {

  awaiter: Subject<AwaiterInfo>
  constructor() {
    this.awaiter = new Subject<AwaiterInfo>();
   }

   public show(info?: AwaiterInfo) {
    this.awaiter.next(info ?? { message: 'Loading' })
  }
  
  public hide() {
    this.awaiter.next(null);
  }
}



export class AwaiterInfo {
  message: string;
  msStartDelay?: number;
}
