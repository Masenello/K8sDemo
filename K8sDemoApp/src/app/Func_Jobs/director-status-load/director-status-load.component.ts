import { Component, OnInit } from '@angular/core';
import { AccountService } from 'src/app/Func_Login/account.service';
import { TestJobCreationRequest } from 'src/app/_models/API_Messages/JobCreationRequest';
import { DirectorStatusMessage } from 'src/app/_models/Hub_Messages/DirectorStatusMessage';
import { DirectorStatusService } from '../director-status.service';
import { JobService } from '../job.service';

@Component({
  selector: 'app-director-status-load',
  templateUrl: './director-status-load.component.html',
  styleUrls: ['./director-status-load.component.css']
})
export class DirectorStatusLoadComponent implements OnInit {

  jobsGaugeValue = 0;
  jobsGaugeLabel = "Jobs Number";
  jobsGaugeForegroundColor = "#2ecc71";

  durationGaugeValue = 0;
  durationGaugeLabel = "Job Duration [s]";
  durationGaugeForegroundColor = "#2ecc71";

  jobsToAdd: number = 50


  constructor(private directorStatusService: DirectorStatusService, private accountService:AccountService, private jobService:JobService) {

    directorStatusService.newDirectorStatus.subscribe((status: DirectorStatusMessage) => {
      //Jobs gauge
      if (status.totalJobs > status.maxConcurrentTasks)
      {
        this.jobsGaugeForegroundColor = "#e74c3c"
      }
      else
      {
        this.jobsGaugeForegroundColor = "#2ecc71"
      }

      this.jobsGaugeValue= status.totalJobs
      //duration gauge
      this.durationGaugeValue= status.maxOpenJobDuration
    })
  }

  ngOnInit(): void {
  }

  sendTestJobRequest() {

    for (let i = 0; i < this.jobsToAdd; i++) {
      this.addNewTestJob();
    }


  }

  addNewTestJob() {
    let jobRequest: TestJobCreationRequest =
    {
      user: this.accountService.currentUser.getValue()?.username!,
      requestDateTime: new Date(),
      requestJobType: 0
    };
    this.jobService.sendJobCreationRequest(jobRequest).subscribe(result => {
      //this.toastr.info(`Job with id ${result.jobId} created by user ${result.user}`);
      // this.updateInternalJobsList(
      //   {
      //     jobId: result.jobId,
      //     jobType: result.jobType,
      //     status: result.jobStatus,
      //     user: result.user,
      //     progressPercentage: 0,
      //     userMessage: result.userMessage,
      //     endDate: null,
      //   });

    }, error => {
      console.log(error);
      // this.toastr.error(error.error);
    });
  }
}
