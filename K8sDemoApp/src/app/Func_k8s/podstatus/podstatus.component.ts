import { Component, Input, OnInit } from '@angular/core';
import { PodInfoDto } from 'src/app/_models/API_Messages/PodInfo';
import { PodUtils } from 'src/app/_shared/Utils/PodUtils';

@Component({
  selector: 'app-podstatus',
  templateUrl: './podstatus.component.html',
  styleUrls: ['./podstatus.component.css']
})
export class PodstatusComponent implements OnInit {

  @Input() pod: PodInfoDto;

  iconPath:string;
  
  constructor(private podUtils:PodUtils) {
    this.podUtils = new PodUtils()
   }

  ngOnInit(): void {
    this.iconPath = this.podUtils.GetPodIconPath(this.pod.name);
    // console.log("Icon path for " + this.pod.name +" : "+ this.iconPath)
  }

}
