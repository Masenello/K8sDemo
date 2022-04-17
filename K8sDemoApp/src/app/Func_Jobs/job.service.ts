import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { TestJobCreationResult } from '../_models/API_Messages/TestJobCreationResult';
import { TestJobCreationRequest} from '../_models/API_Messages/TestJobCreationRequest';
import { JobStatusMessage } from '../_models/Hub_Messages/JobStatusMessage';



@Injectable({
  providedIn: 'root'
})
export class JobService {

  baseUrl = environment.apiUrl;


  constructor(private http: HttpClient) 
  {
    
  }

  getUserPendingJobs(username: string): Observable<JobStatusMessage[]>{
    return this.http.get<JobStatusMessage[]>(this.baseUrl + "Job/"+username, {responseType: 'json'});
  }
  

  sendTestJobRequest(jobRequest: TestJobCreationRequest) : Observable<TestJobCreationResult>{
        
    return this.http.post<TestJobCreationResult>(this.baseUrl + "job/RequestNewJob", 
    {
        "user": jobRequest.user,
        "requestDateTime": jobRequest.requestDateTime,
        "requestedJobType": jobRequest.requestJobType
    });
    }
}
