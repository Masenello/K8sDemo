import { Component, Input, OnInit } from '@angular/core';
import { JobStatusMessage } from 'src/app/_models/Hub_Messages/JobStatusMessage';
import { JobStatusEnumNamePipe } from '../jobEnumsPipes';


@Component({
  selector: 'app-jobstatus',
  templateUrl: './jobstatus.component.html',
  styleUrls: ['./jobstatus.component.css']
})
export class JobstatusComponent implements OnInit {
  @Input() job: JobStatusMessage;




  constructor() {

  }

  ngOnInit(): void {
  }


}
