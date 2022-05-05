import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { GuiColumn, GuiDataType, GuiPaging, GuiPagingDisplay, GuiSorting, GuiSortingOrder } from '@generic-ui/ngx-grid';
import { from } from 'linq-to-typescript';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/app/Func_Login/account.service';
import { Job } from 'src/app/_models/API_Messages/Job';
import { environment } from 'src/environments/environment';
import { JobService } from '../job.service';
import { JobStatusEnumNamePipe, JobTypeEnumNamePipe } from '../jobEnumsPipes';

@Component({
  selector: 'app-jobhistorical',
  templateUrl: './jobhistorical.component.html',
  styleUrls: ['./jobhistorical.component.css']
})
export class JobhistoricalComponent implements OnInit {
  
  datePipe = new DatePipe('en-US');
  jobsSource =new Array<Job>();
  jobGridcolumns: Array<GuiColumn> = [
    {
      header: 'Id',
      field: 'jobId',
      type: GuiDataType.NUMBER,
      sorting: {
        enabled: true,
        order:GuiSortingOrder.DESC
        
    }
    },
    {
      header: 'Job Type',
      field: 'jobType',
      formatter: (v) => this.jobTypeEnumsPipe.transform(v)
    },
    {
      header: 'Status',
      field: 'status',
      formatter: (v) => this.jobStatusEnumsPipe.transform(v)
    },
    {
      header: 'Creation Date',
      field: 'creationDate',
      formatter: (v) => this.datePipe.transform(v,environment.dateTimeFormat)
    },
    {
      header: 'Start Date',
      field: 'startDate',
      formatter: (v) => this.datePipe.transform(v,environment.dateTimeFormat)
    },
    {
      header: 'End Date',
      field: 'endDate',
      formatter: (v) => this.datePipe.transform(v,environment.dateTimeFormat)
    },
    {
      header: 'Errors',
      field: 'errors'
    },
    {
      header: 'Worker',
      field: 'workerId'
    },
];

paging: GuiPaging = {
  enabled: true,
  page: 1,
  pageSize: 10,
  pageSizes: [10, 25, 50],
  pagerTop: true,
  pagerBottom: true,
  display: GuiPagingDisplay.BASIC
};

sorting: GuiSorting = {
  enabled: true
};


loading = true

  constructor(private jobService:JobService, private accountService:AccountService,
    private jobStatusEnumsPipe : JobStatusEnumNamePipe,
    private jobTypeEnumsPipe : JobTypeEnumNamePipe,
    private toastr:ToastrService,
    ) {


  }

  ngOnInit(): void {
    this.loadData();
  }

  manualLoadData()
  {
    this.loadData()
    this.toastr.info("Jobs historical data loaded from database")
  }


  loadData()
  {
    this.loading = true
    this.jobService.getUserJobs(this.accountService.currentUser.value.username).subscribe((userJobs)=> 
    {
      this.jobsSource = userJobs;
    })
    this.loading = false

    
  }

}
