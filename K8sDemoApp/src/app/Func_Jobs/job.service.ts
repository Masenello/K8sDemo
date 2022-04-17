import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { TestJobCreationResult } from '../_models/API_Messages/TestJobCreationResult';
import { TestJobCreationRequest} from '../_models/API_Messages/TestJobCreationRequest';



@Injectable({
  providedIn: 'root'
})
export class JobService {

  baseUrl = environment.apiUrl;


  constructor(private http: HttpClient) 
  {
    
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
