import { Component, Input, OnInit } from '@angular/core';
import { JobTypeEnum } from 'src/app/_enum/JobTypeEnum';
import { DirectorStatusMessage } from 'src/app/_models/Hub_Messages/DirectorStatusMessage';
import { jobChartDescriptor } from 'src/app/_models/jobChartDescriptor';
import { DirectorStatusService } from '../director-status.service';

@Component({
  selector: 'app-job-type-status',
  templateUrl: './job-type-status.component.html',
  styleUrls: ['./job-type-status.component.css']
})
export class JobTypeStatusComponent implements OnInit {

  @Input() targetJobType: JobTypeEnum;


  options : any

 

  constructor(private directorStatusService:DirectorStatusService) {
    this.directorStatusService.newDirectorStatus.subscribe((status: DirectorStatusMessage)=>{
      this.buildChart(this.directorStatusService.buildChartData(this.targetJobType))

    })
  }

  buildChart(chartDecriptor: jobChartDescriptor)
  {

    this.options= {
      title: {
        text: chartDecriptor.chartTitle,
        x: 'center'
      },
      legend: {
        data: ['Workers', 'Unassigned Jobs'],
        align: 'left',
        y: 'bottom',
      },
      tooltip: {},
      xAxis: {
        data: chartDecriptor.xAxisData,
        silent: false,
        splitLine: {
          show: false,
        },
      },
      yAxis: {},
      series: [
        {
          name: 'Workers',
          type: 'line',
          showSymbol: false,
          data: chartDecriptor.yWorkerAxisData,
        },
        {
          name: 'Unassigned Jobs',
          type: 'line',
          showSymbol: false,
          data: chartDecriptor.yJobsAxisData,
        },
      ],
    }

  }



  ngOnInit(): void {
  }

}
