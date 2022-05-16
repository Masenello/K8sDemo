import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-podlogsviewer',
  templateUrl: './podlogsviewer.component.html',
  styleUrls: ['./podlogsviewer.component.css']
})
export class PodlogsviewerComponent implements OnInit {

  logViewerTitle: string
  logText: string
  constructor(@Inject(MAT_DIALOG_DATA) public data: { title: string, log: string }) {
    this.logViewerTitle = data.title
    this.logText = data.log
  }

  ngOnInit(): void {
  }



}
