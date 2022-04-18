import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/internal/Subject';
import { HubService } from '../services/hub.service';
import { DirectorStatusMessage } from '../_models/Hub_Messages/DirectorStatusMessage';

@Injectable({
  providedIn: 'root'
})
export class DirectorStatusService {

  public directorStatus : Subject<DirectorStatusMessage> = new Subject<DirectorStatusMessage> ();

  constructor(private hub: HubService,) {
    this.hub.receivedNewDirectorStatusEvent.subscribe((data:DirectorStatusMessage)=>
    { 
      //console.log(data)
      this.directorStatus.next(data);
    });
  }
}
