import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { JobhistoricalComponent } from 'src/app/Func_Jobs/jobhistorical/jobhistorical.component';
import { JobmanagerComponent } from 'src/app/Func_Jobs/jobmanager/jobmanager.component';

@Component({
  selector: 'app-async-jobs-test',
  templateUrl: './async-jobs-test.component.html',
  styleUrls: ['./async-jobs-test.component.css']
})
export class AsyncJobsTestComponent implements OnInit {

  @ViewChild(JobhistoricalComponent) jobsHistoricalComponent: JobhistoricalComponent;
  @ViewChild(JobmanagerComponent) jobManagerComponent: JobmanagerComponent;

  constructor() { }

  ngOnInit(): void {
  }

  tabChanged = (tabChangeEvent: MatTabChangeEvent): void => {
    console.log('tabChangeEvent => ', tabChangeEvent); 
    console.log('index => ', tabChangeEvent.tab.textLabel); 
    switch(tabChangeEvent.tab.textLabel)
    {
      case "Jobs Historical":
        this.jobsHistoricalComponent.loadData();
        break;
      case "Pending Jobs":
        this.jobManagerComponent.getUserPendingJobs();
        break;

    }
    
}

}
