import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { TestJobCreationResult } from '../_models/API_Messages/TestJobCreationResult';
import { TestJobCreationRequest} from '../_models/API_Messages/TestJobCreationRequest';
import { ToastrService } from 'ngx-toastr';
import { HubService } from '../services/hub.service';
import { JobStatusMessage } from '../_models/Hub_Messages/JobStatusMessage';
import { JobStatusEnum } from '../_enum/JobStatusEnum';

@Injectable({
  providedIn: 'root'
})
export class JobService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, 
    private toastr: ToastrService,
    private hub: HubService) 
  {
    this.hub.receivedNewJobStatusEvent.subscribe((data:JobStatusMessage)=> 
    {
      if (data.status == JobStatusEnum.error)
          {
            this.toastr.error(`${data.userMessage}`)
          }
          else
          {
            this.toastr.info(`Job id: ${data.jobId}: Status: ${JobStatusEnum[data.status]} Percentage: ${data.progressPercentage}`)
          }
          console.log(data);
    })
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
