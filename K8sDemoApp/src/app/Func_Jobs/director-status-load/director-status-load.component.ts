import { Component, OnInit } from '@angular/core';
import { DirectorStatusMessage } from 'src/app/_models/Hub_Messages/DirectorStatusMessage';
import { DirectorStatusService } from '../director-status.service';

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


  constructor(private directorStatusService: DirectorStatusService) {

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
}
