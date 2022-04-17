import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/Func_Login/account.service';
import { HubService } from 'src/app/services/hub.service';
import { JobStatusEnum } from 'src/app/_enum/JobStatusEnum';
import { JobTypeEnum } from 'src/app/_enum/JobTypeEnum';
import { TestJobCreationRequest } from 'src/app/_models/API_Messages/TestJobCreationRequest';
import { JobStatusMessage } from 'src/app/_models/Hub_Messages/JobStatusMessage';
import { JobService } from '../job.service';

import { from } from "linq-to-typescript"
import { Subject } from 'rxjs';

@Component({
  selector: 'app-jobmanager',
  templateUrl: './jobmanager.component.html',
  styleUrls: ['./jobmanager.component.css']
})
export class JobmanagerComponent implements OnInit {

  currentJobs =new Subject<JobStatusMessage[]>();
  currentJobsTmp: Array<JobStatusMessage> = new Array<JobStatusMessage>();

  constructor(public accountService: AccountService,
    public jobService : JobService,
    private toastr: ToastrService,
    private hub:HubService) 
    {
      this.hub.receivedNewJobStatusEvent.subscribe((jobStatus:JobStatusMessage)=> 
      {
        console.log(jobStatus);
        if (jobStatus.status == JobStatusEnum.error)
          {
            this.toastr.error(`${jobStatus.userMessage}`)
          }
        else
          {
            this.toastr.info(`Job id: ${jobStatus.jobId}: Status: ${JobStatusEnum[jobStatus.status]} Percentage: ${jobStatus.progressPercentage}`)
          }

        var tmpJob:JobStatusMessage = from(this.currentJobsTmp).where(x=>x.jobId == jobStatus.jobId).firstOrDefault();
        if (tmpJob == null)
        {
          this.currentJobsTmp.push(jobStatus);
        }
        else
        {
          tmpJob.progressPercentage = jobStatus.progressPercentage;
          tmpJob.status = jobStatus.status;
          tmpJob.userMessage = jobStatus.userMessage;
        }

        this.currentJobs.next(this.currentJobsTmp);
        
      })


  }

  ngOnInit(): void {
  }

  sendTestJobRequest() {
    let jobRequest:TestJobCreationRequest =  
    {
      user:this.accountService.currentUser.getValue()?.username!,
      requestDateTime: new Date(),
      requestJobType:0
    };
    this.jobService.sendTestJobRequest(jobRequest).subscribe(result =>{
      this.toastr.info(`Job with id ${result.jobId} created by user ${result.user}`);

      this.currentJobsTmp.push(
        {
          jobId: result.jobId,
          jobType: result.jobType,
          status: result.jobStatus,
          user:result.user,
          progressPercentage: 0,
          userMessage: result.userMessage
        }
      );


    },error=>{
      console.log(error);
      this.toastr.error(error.error);
    });
  }

}
