import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { TestJobCreationResult } from '../_models/TestJobCreationResult';
import { TestJobCreationRequest} from '../_models/TestJobCreationRequest';
import { AccountService } from './account.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class JobService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, 
    private toastr: ToastrService) 
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
