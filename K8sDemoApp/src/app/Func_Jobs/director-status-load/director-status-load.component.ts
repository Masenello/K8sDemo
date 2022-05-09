import { Component, OnInit } from '@angular/core';
import { DirectorStatusMessage } from 'src/app/_models/Hub_Messages/DirectorStatusMessage';
import { DirectorStatusService } from '../director-status.service';

@Component({
  selector: 'app-director-status-load',
  templateUrl: './director-status-load.component.html',
  styleUrls: ['./director-status-load.component.css']
})
export class DirectorStatusLoadComponent implements OnInit {

  gaugeValue = 0;
  gaugeLabel = "Jobs";
  foregroundColor = "#2ecc71";


  constructor(private directorStatusService: DirectorStatusService) {

    directorStatusService.newDirectorStatus.subscribe((status: DirectorStatusMessage) => {
      var freejobs = status.maxConcurrentTasks - status.totalJobs;
      if (status.totalJobs > status.maxConcurrentTasks)
      {
        this.foregroundColor = "#e74c3c"
      }
      else
      {
        this.foregroundColor = "#2ecc71"
      }

      this.gaugeValue= status.totalJobs
    })
  }

  ngOnInit(): void {
  }
}
