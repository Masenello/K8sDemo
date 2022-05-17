import { DatePipe } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-podlogsviewer',
  templateUrl: './podlogsviewer.component.html',
  styleUrls: ['./podlogsviewer.component.css']
})
export class PodlogsviewerComponent implements OnInit {

  logViewerTitle: string
  logText: string
  logStartDate: string
  datePipe = new DatePipe('en-US'); 

  constructor(@Inject(MAT_DIALOG_DATA) public data: { title: string, log: string,  logsStartDate: Date}) {
    this.logViewerTitle = data.title
    this.logText = data.log
    this.logStartDate = this.datePipe.transform(data.logsStartDate, environment.dateTimeFormat)
  }

  ngOnInit(): void {
  }



}
