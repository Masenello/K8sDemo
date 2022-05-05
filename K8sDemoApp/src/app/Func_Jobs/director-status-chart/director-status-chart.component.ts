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

  jobsChartOptions : any
  workersChartOptions : any

  constructor(private directorStatusService:DirectorStatusService) {
    this.directorStatusService.newDirectorStatus.subscribe((status: DirectorStatusMessage)=>{
      this.buildChart(this.directorStatusService.buildChartData())

    })
  }

  buildChart(chartDecriptor: jobChartDescriptor)
  {

    this.jobsChartOptions= {
      title: {
        text: chartDecriptor.jobChartTitle,
        x: 'center'
      },
      legend: {
        data: ['Active Jobs', 'Total Jobs', 'Max Jobs'],
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
          name: 'Active Jobs',
          type: 'line',
          showSymbol: true,
          data: chartDecriptor.yJobsAxisData,
        },
        {
          name: 'Total Jobs',
          type: 'line',
          showSymbol: true,
          data: chartDecriptor.yTotalJobsAxisData,
        },
        {
          name: 'Max Jobs',
          type: 'line',
          showSymbol: false,
          data: chartDecriptor.yMaxJobsAxisData,
        },
      ],
    }

    this.workersChartOptions= {
      title: {
        text: chartDecriptor.workersChartTitle,
        x: 'center'
      },
      legend: {
        data: ['Workers','Max Workers'],
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
          showSymbol: true,
          data: chartDecriptor.yWorkerAxisData,
        },
        {
          name: 'Max Workers',
          type: 'line',
          showSymbol: false,
          data: chartDecriptor.yMaxWorkersAxisData,
        },
      ],
    }

  }



  ngOnInit(): void {
  }

}
