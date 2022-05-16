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
  internalJobsList = new Subject<JobStatusMessage[]>();
  currentJobsTmp: Array<JobStatusMessage> = new Array<JobStatusMessage>();
  jobsListHeader: string;
  internalJobsListActiveJobsNumber = new BehaviorSubject<number>(0);


  constructor(public accountService: AccountService,
    public jobService: JobService,
    private toastr: ToastrService,
    private hub: HubService,
    private jobTypeEnumNamePipe: JobTypeEnumNamePipe) {

     this.jobService.newJobStatus.subscribe((newStatus:JobStatusMessage)=>
     {
        if (newStatus.status == JobStatusEnum.error) {
          this.toastr.error(`${newStatus.userMessage}`)
        }
        this.updateInternalJobsList(newStatus);
     }) 
   
  }

  ngOnInit(): void {
    this.getUserPendingJobs();
  }

  getUserPendingJobs() {
    //Clean current list
    this.resetJobsList()
    //Populate list with data from database
    this.jobService.getUserPendingJobs(this.accountService.currentUser.value.username).subscribe((userPendingJobs) => {
      userPendingJobs.forEach(element => {
        this.updateInternalJobsList(element);
      });

    })
  }

  resetJobsList() {
    this.currentJobsTmp = new Array<JobStatusMessage>();
    this.internalJobsList.next(this.currentJobsTmp);
    this.internalJobsListActiveJobsNumber.next(0)
  }

  updateInternalJobsList(jobStatus: JobStatusMessage) {
    var tmpJob: JobStatusMessage = from(this.currentJobsTmp).where(x => x.jobId == jobStatus.jobId).firstOrDefault();
    if (tmpJob == null) {
      //Job is not in list, add it
      this.currentJobsTmp.push(jobStatus);
    }
    else {
      //Update job in list
      if (jobStatus.endDate != null) {
        //Remove element (job completed)
        var index = this.currentJobsTmp.findIndex(x => x.jobId == jobStatus.jobId);
        if (index > -1) {
          this.currentJobsTmp.splice(index, 1);
        }
      }
      else {
        //Update element (job in progress)
        tmpJob.progressPercentage = jobStatus.progressPercentage;
        tmpJob.status = jobStatus.status;
        tmpJob.userMessage = jobStatus.userMessage;
      }



    }

    this.internalJobsList.next(this.currentJobsTmp);
    this.internalJobsListActiveJobsNumber.next(from(this.currentJobsTmp).where(x => x.endDate == null).count())
  }



  
}
