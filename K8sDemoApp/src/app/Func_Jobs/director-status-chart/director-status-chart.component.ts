import { Component, Input, OnInit } from '@angular/core';
import { JobTypeEnum } from 'src/app/_enum/JobTypeEnum';
import { DirectorStatusMessage } from 'src/app/_models/Hub_Messages/DirectorStatusMessage';
import { jobChartDescriptor } from 'src/app/_models/jobChartDescriptor';
import { DirectorStatusService } from '../director-status.service';

@Component({
  selector: 'app-director-status-chart',
  templateUrl: './director-status-chart.component.html',
  styleUrls: ['./director-status-chart.component.css']
})
export class DirectorStatusChartComponent implements OnInit {

  options : any

 

  constructor(private directorStatusService:DirectorStatusService) {
    this.directorStatusService.newDirectorStatus.subscribe((status: DirectorStatusMessage)=>{
      this.buildChart(this.directorStatusService.buildChartData())

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
        data: ['Workers', 'Active Jobs', 'Total Jobs'],
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
          name: 'Active Jobs',
          type: 'line',
          showSymbol: false,
          data: chartDecriptor.yJobsAxisData,
        },
        {
          name: 'Total Jobs',
          type: 'line',
          showSymbol: false,
          data: chartDecriptor.yTotalJobsAxisData,
        },
      ],
    }

  }



  ngOnInit(): void {
  }

}
