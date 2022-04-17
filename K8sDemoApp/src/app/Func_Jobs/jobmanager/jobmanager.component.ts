import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/Func_Login/account.service';
import { HubService } from 'src/app/services/hub.service';
import { JobStatusEnum } from 'src/app/_enum/JobStatusEnum';
import { JobTypeEnum } from 'src/app/_enum/JobTypeEnum';
import { TestJobCreationRequest } from 'src/app/_models/API_Messages/TestJobCreationRequest';
import { JobStatusMessage } from 'src/app/_models/Hub_Messages/JobStatusMessage';
import { JobService } from '../job.service';

@Component({
  selector: 'app-jobmanager',
  templateUrl: './jobmanager.component.html',
  styleUrls: ['./jobmanager.component.css']
})
export class JobmanagerComponent implements OnInit {

  jobs: JobStatusMessage[] = [
    {
      jobId: 1,
      jobType: JobTypeEnum.test,
      status: JobStatusEnum.running,
      user:"pimpi",
      progressPercentage: 27,
      userMessage: "Tutto bello"
    },
    {
      jobId: 2,
      jobType: JobTypeEnum.test,
      status: JobStatusEnum.inserted,
      user:"ciccio",
      progressPercentage: 0,
      userMessage: ""
    },
    {
      jobId: 3,
      jobType: JobTypeEnum.test,
      status: JobStatusEnum.running,
      user:"pimpi",
      progressPercentage: 27,
      userMessage: "Tutto bello"
    },
  ];

  constructor(public accountService: AccountService,
    public jobService : JobService,
    private toastr: ToastrService,
    private hub:HubService) {
    
    this.hub.receivedNewJobStatusEvent.subscribe((data:JobStatusMessage)=> 
    {
      
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
    },error=>{
      console.log(error);
      this.toastr.error(error.error);
    });
  }

}
