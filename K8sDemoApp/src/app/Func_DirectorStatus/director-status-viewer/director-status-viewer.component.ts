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


  }

  ngOnInit(): void {
  }

}
