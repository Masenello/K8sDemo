import { Component, OnInit } from '@angular/core';
import { JobTypeEnum } from 'src/app/_enum/JobTypeEnum';
import { DirectorStatusMessage } from 'src/app/_models/Hub_Messages/DirectorStatusMessage';
import { DirectorStatusService } from '../director-status.service';

@Component({
  selector: 'app-director-status-viewer',
  templateUrl: './director-status-viewer.component.html',
  styleUrls: ['./director-status-viewer.component.css']
})
export class DirectorStatusViewerComponent implements OnInit {

  workerList:string
  jobsList:string

  constructor(private directorStatusService:DirectorStatusService) {

    this.directorStatusService.directorStatus.subscribe((status: DirectorStatusMessage)=>{
      
      this.jobsList = ""
      if (status.jobsList!=null)
      {
        this.jobsList ="Jobs:";
        status.jobsList.forEach(p =>{
          this.jobsList = this.jobsList + JobTypeEnum[p.jobType] + ":" + p.jobCount 
      })
      }
    

      this.workerList=""
      if (status.registeredWorkers!=null)
      {
        this.workerList = "Workers:"
        status.registeredWorkers.forEach(x =>{
          this.workerList = this.workerList + x.workerId + ":" + JobTypeEnum[x.workerJobType]
        })
      }
      // console.log(this.jobsList)
      // console.log(this.workerList)

      
    })

  }

  ngOnInit(): void {
  }

}
