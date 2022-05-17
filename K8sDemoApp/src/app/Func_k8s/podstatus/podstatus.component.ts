import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { PodInfoDto } from 'src/app/_models/API_Messages/PodInfo';
import { PodUtils } from 'src/app/_shared/Utils/PodUtils';
import { K8sinfoService } from '../k8sinfo.service';
import { PodlogsviewerComponent } from '../podlogsviewer/podlogsviewer.component';

@Component({
  selector: 'app-podstatus',
  templateUrl: './podstatus.component.html',
  styleUrls: ['./podstatus.component.css']
})
export class PodstatusComponent implements OnInit {

  @Input() pod: PodInfoDto;

  iconPath: string;

  constructor(private podUtils: PodUtils, 
    private k8sinfoservice: K8sinfoService,
    public dialog: MatDialog) {
    this.podUtils = new PodUtils()
  }

  ngOnInit(): void {
    this.iconPath = this.podUtils.GetPodIconPath(this.pod.name);
    // console.log("Icon path for " + this.pod.name +" : "+ this.iconPath)
  }

  getPodLog() {
    this.k8sinfoservice.getPodLog(this.pod.name).subscribe((podLog)=>{
      console.log(podLog.log)
      const dialogRef = this.dialog.open(PodlogsviewerComponent, {
        data: { title: podLog.podName, log: podLog.log,  logsStartDate: podLog.logsStartDate },
      });
    }, (erorr)=>
    {
      console.log(erorr)
    })
  }

}
