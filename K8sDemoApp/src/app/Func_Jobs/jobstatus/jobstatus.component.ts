import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { HubService } from 'src/app/services/hub.service';
import { JobStatusEnum } from 'src/app/_enum/JobStatusEnum';
import { JobTypeEnum } from 'src/app/_enum/JobTypeEnum';
import { JobStatusMessage } from 'src/app/_models/Hub_Messages/JobStatusMessage';

@Component({
  selector: 'app-jobstatus',
  templateUrl: './jobstatus.component.html',
  styleUrls: ['./jobstatus.component.css']
})
export class JobstatusComponent implements OnInit {

  job:JobStatusMessage =     {
    jobId: 1,
    jobType: JobTypeEnum.test,
    status: JobStatusEnum.running,
    user:"pimpi",
    progressPercentage: 27,
    userMessage: "Aaaaaargh!"
  }

  constructor() {

  }

  ngOnInit(): void {
  }

 

}
