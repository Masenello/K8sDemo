import { Component, Input, OnInit } from '@angular/core';
import { from } from 'linq-to-typescript';
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

  xAxisDataBuffer:Array<Date>  = [];
  workersBuffer :Array<number>  = [];
  jobsBuffer :Array<number> = [];
 

  constructor(private directorStatusService:DirectorStatusService) {
    this.directorStatusService.newDirectorStatus.subscribe((status: DirectorStatusMessage)=>{
      this.buildChart(this.directorStatusService.buildChartData(this.targetJobType))

    })
  }

  buildChart(chartDecriptor: jobChartDescriptor)
  {
    let xAxisData:Array<Date>  = chartDecriptor.xAxisData
    let workers :Array<number>  = chartDecriptor.yWorkerAxisData
    let jobs :Array<number> = chartDecriptor.yJobsAxisData

    this.options= {
      legend: {
        data: ['Workers', 'Unassigned Jobs'],
        align: 'left',
      },
      tooltip: {},
      xAxis: {
        data: xAxisData,
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
          data: workers,
        },
        {
          name: 'Unassigned Jobs',
          type: 'line',
          showSymbol: false,
          data: jobs,
        },
      ],
    }

  }



  ngOnInit(): void {
  }

}
