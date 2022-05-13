import { Component, Input, OnInit } from '@angular/core';
import { PodInfoDto } from 'src/app/_models/API_Messages/PodInfo';

@Component({
  selector: 'app-podstatus',
  templateUrl: './podstatus.component.html',
  styleUrls: ['./podstatus.component.css']
})
export class PodstatusComponent implements OnInit {

  @Input() pod: PodInfoDto;
  constructor() { }

  ngOnInit(): void {
  }

}
