import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/Func_Login/account.service';
import { HubService } from 'src/app/services/hub.service';
import { JobStatusEnum } from 'src/app/_enum/JobStatusEnum';
import { JobTypeEnum } from 'src/app/_enum/JobTypeEnum';
import { TestJobCreationRequest } from 'src/app/_models/API_Messages/JobCreationRequest';
import { JobStatusMessage } from 'src/app/_models/Hub_Messages/JobStatusMessage';
import { JobService } from '../job.service';

import { from } from "linq-to-typescript"
import { BehaviorSubject, Subject } from 'rxjs';
import { JobTypeEnumNamePipe } from '../jobEnumsPipes';

@Component({
  selector: 'app-jobmanager',
  templateUrl: './jobmanager.component.html',
  styleUrls: ['./jobmanager.component.css']
})
export class JobmanagerComponent implements OnInit {
  
  @Input() targetJobType: JobTypeEnum;
  internalJobsList =new Subject<JobStatusMessage[]>();
  currentJobsTmp: Array<JobStatusMessage> = new Array<JobStatusMessage>();
  jobsListHeader:string;
  internalJobsListActiveJobsNumber =new BehaviorSubject<number>(0);

  constructor(public accountService: AccountService,
    public jobService : JobService,
    private toastr: ToastrService,
    private hub:HubService,
    private jobTypeEnumNamePipe:JobTypeEnumNamePipe) 
    {
      //Subscribe to Job updates from HUB
      this.hub.receivedNewJobStatusEvent.subscribe((jobStatus:JobStatusMessage)=> 
      {
        //console.log(jobStatus);
        if (jobStatus.status == JobStatusEnum.error)
        {
          this.toastr.error(`${jobStatus.userMessage}`)
        }
        this.updateInternalJobsList(jobStatus);
      })

      

  }

  ngOnInit(): void {
    this.getUserPendingJobs();
  }

  getUserPendingJobs()
  {
    this.jobService.getUserPendingJobs(this.accountService.currentUser.value.username).subscribe((userPendingJobs)=> 
    {
      userPendingJobs.forEach(element => {
        this.updateInternalJobsList(element);
      });
      
    })
  }

  updateInternalJobsList(jobStatus:JobStatusMessage)
  {
    var tmpJob:JobStatusMessage = from(this.currentJobsTmp).where(x=>x.jobId == jobStatus.jobId).firstOrDefault();
    if (tmpJob == null)
    {
      //Job is not in list, add it
      this.currentJobsTmp.push(jobStatus);
    }
    else
    {
        //Update job in list
        if (jobStatus.endDate != null)
        {
          //Remove element (job completed)
          var index =  this.currentJobsTmp.findIndex(x => x.jobId==jobStatus.jobId);
          if (index > -1) {
            this.currentJobsTmp.splice(index, 1);
          }
        }
        else
        {
          //Update element (job in progress)
          tmpJob.progressPercentage = jobStatus.progressPercentage;
          tmpJob.status = jobStatus.status;
          tmpJob.userMessage = jobStatus.userMessage;
        }



    }
    //TODO Remove completed jobs from list, now they are only hidden!
    this.internalJobsList.next(this.currentJobsTmp);
    this.jobsListHeader= `${from(this.currentJobsTmp).where(x=>x.status == JobStatusEnum.completed).count()} Pending jobs for user: ${this.accountService.currentUser.value.username}`;
    this.internalJobsListActiveJobsNumber.next(from(this.currentJobsTmp).where(x=>x.status != JobStatusEnum.completed).count())
  }

  sendTestJobRequest() {
    let jobRequest:TestJobCreationRequest =  
    {
      user:this.accountService.currentUser.getValue()?.username!,
      requestDateTime: new Date(),
      requestJobType:0
    };
    this.jobService.sendJobCreationRequest(jobRequest).subscribe(result =>{
      this.toastr.info(`Job with id ${result.jobId} created by user ${result.user}`);
      this.updateInternalJobsList(
        {
          jobId: result.jobId,
          jobType: result.jobType,
          status: result.jobStatus,
          user:result.user,
          progressPercentage: 0,
          userMessage: result.userMessage,
          endDate: null,
        });

    },error=>{
      console.log(error);
      this.toastr.error(error.error);
    });
  }



}
