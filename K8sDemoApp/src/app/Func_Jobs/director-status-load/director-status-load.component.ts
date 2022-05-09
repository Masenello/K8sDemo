import { Component, OnInit } from '@angular/core';
import { DirectorStatusMessage } from 'src/app/_models/Hub_Messages/DirectorStatusMessage';
import { DirectorStatusService } from '../director-status.service';

@Component({
  selector: 'app-director-status-load',
  templateUrl: './director-status-load.component.html',
  styleUrls: ['./director-status-load.component.css']
})
export class DirectorStatusLoadComponent implements OnInit {

  gaugeChartOptions: any


  constructor(private directorStatusService: DirectorStatusService) {

    directorStatusService.newDirectorStatus.subscribe((status: DirectorStatusMessage) => {
      var freejobs = status.maxConcurrentTasks - status.totalJobs;
      if (freejobs < 0) freejobs = 0;
      this.buildChart(status.totalJobs, freejobs);
    })
  }

  ngOnInit(): void {
  }

  buildChart(currentJobs: number, freeJobs: number) {
    this.gaugeChartOptions = {
      title: {
        text: 'Director load',
        x: 'center'
      },
      tooltip: {
        trigger: 'item',
        formatter: '{a} <br/>{b} : {c} ({d}%)'
      },
      legend: {
        x: 'center',
        y: 'bottom',
        data: ['currentJobs', 'freejobs']
      },
      calculable: true,
      series: [
        {
          name: 'area',
          type: 'pie',
          radius: [110, 30],
          roseType: 'area',
          data: [
            { value: currentJobs, name: 'currentJobs' },
            { value: freeJobs, name: 'freejobs' },
          ]
        }
      ]
    };
  }

}
