import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/services/account.service';
import { TestJobCreationRequest } from 'src/app/_models/TestJobCreationRequest';
import { JobService } from '../job.service';

@Component({
  selector: 'app-jobmanager',
  templateUrl: './jobmanager.component.html',
  styleUrls: ['./jobmanager.component.css']
})
export class JobmanagerComponent implements OnInit {

  constructor(public accountService: AccountService,
    public jobService : JobService,
    private toastr: ToastrService) { }

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
