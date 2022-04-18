import { Component, OnInit } from '@angular/core';
import { GuiColumn, GuiPaging, GuiPagingDisplay } from '@generic-ui/ngx-grid';
import { from } from 'linq-to-typescript';
import { AccountService } from 'src/app/Func_Login/account.service';
import { Job } from 'src/app/_models/API_Messages/Job';
import { JobService } from '../job.service';
import { JobStatusEnumNamePipe, JobTypeEnumNamePipe } from '../jobEnumsPipes';

@Component({
  selector: 'app-jobhistorical',
  templateUrl: './jobhistorical.component.html',
  styleUrls: ['./jobhistorical.component.css']
})
export class JobhistoricalComponent implements OnInit {
  
  jobsSource =new Array<Job>();
  jobGridcolumns: Array<GuiColumn> = [
    {
      header: 'Id',
      field: 'jobId'
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
      field: 'creationDate'
    },
    {
      header: 'Start Date',
      field: 'startDate'
    },
    {
      header: 'End Date',
      field: 'endDate'
    },
    {
      header: 'Description',
      field: 'description'
    },
    {
      header: 'Errors',
      field: 'errors'
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

loading = true

  constructor(private jobService:JobService, private accountService:AccountService,
    private jobStatusEnumsPipe : JobStatusEnumNamePipe,
    private jobTypeEnumsPipe : JobTypeEnumNamePipe
    ) {


  }

  ngOnInit(): void {
    this.loadData();
  }


  loadData()
  {
    this.loading = true
    this.jobService.getUserJobs(this.accountService.currentUser.value.username).subscribe((userJobs)=> 
    {
      //console.log(userJobs)
      this.jobsSource = from(userJobs).orderByDescending(x=>x.jobId).toArray();
    })
    this.loading = false
  }

}
