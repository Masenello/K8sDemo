import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { JobCreationResult } from '../_models/API_Messages/JobCreationResult';
import { TestJobCreationRequest} from '../_models/API_Messages/JobCreationRequest';
import { JobStatusMessage } from '../_models/Hub_Messages/JobStatusMessage';
import { Job } from '../_models/API_Messages/Job';
import { HubService } from '../services/hub.service';
import { JobStatusEnum } from '../_enum/JobStatusEnum';
import { AccountService } from '../Func_Login/account.service';



@Injectable({
  providedIn: 'root'
})
export class JobService {

  baseUrl = environment.apiUrl + "Job/";
  public newJobStatus : Subject<JobStatusMessage> = new Subject<JobStatusMessage> ();


  constructor(private http: HttpClient, private hub: HubService) 
  {
     //Subscribe to Job updates from HUB
     this.hub.receivedNewJobStatusEvent.subscribe((newJobStatus: JobStatusMessage) => {
       //console.log(newJobStatus);
      this.newJobStatus.next(newJobStatus);
    })
  }

  getUserPendingJobs(username: string): Observable<JobStatusMessage[]>{
    return this.http.get<JobStatusMessage[]>(this.baseUrl +"GetUserPendingJobs/" +username, {responseType: 'json'});
  }

  getUserJobs(username: string): Observable<Job[]>{
    return this.http.get<Job[]>(this.baseUrl + "GetUserJobs/"+username, {responseType: 'json'});
  }


  sendJobCreationRequest(jobRequest: TestJobCreationRequest) : Observable<JobCreationResult>{
        
    return this.http.post<JobCreationResult>(this.baseUrl + "RequestNewJob", 
    {
        "user": jobRequest.user,
        "requestDateTime": jobRequest.requestDateTime,
        "requestedJobType": jobRequest.requestJobType
    });
    }
}
