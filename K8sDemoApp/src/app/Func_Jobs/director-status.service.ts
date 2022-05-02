import { DatePipe } from '@angular/common';
import { Injectable } from '@angular/core';
import { from } from 'linq-to-typescript';
import { Subject } from 'rxjs/internal/Subject';
import { environment } from 'src/environments/environment';
import { AccountService } from '../Func_Login/account.service';
import { HubService } from '../services/hub.service';
import { JobTypeEnum } from '../_enum/JobTypeEnum';
import { DirectorStatusMessage } from '../_models/Hub_Messages/DirectorStatusMessage';
import { jobChartDescriptor } from '../_models/jobChartDescriptor';

@Injectable({
  providedIn: 'root'
})
export class DirectorStatusService {

  public newDirectorStatus : Subject<DirectorStatusMessage> = new Subject<DirectorStatusMessage> ();
  datePipe = new DatePipe('en-US');
  directorStatusDataBuffer:Array<DirectorStatusMessage>  = [];
  directorStatusDataBufferMaxPoints = 200

  constructor(private hub: HubService) {
    this.hub.receivedNewDirectorStatusEvent.subscribe((data:DirectorStatusMessage)=>
    { 
      //console.log(data)
      this.directorStatusDataBuffer.push(data);
      this.newDirectorStatus.next(data)
    });


    this.hub.hubConnectionStatus.subscribe((status:boolean)=>{
      if (status)
      {
        this.sendJoinDirectorMonitorGroup()
      }
      
    })

    
  }

  buildChartData(): jobChartDescriptor
  {
    var descriptor: jobChartDescriptor = new jobChartDescriptor()
    descriptor.chartTitle = `Director status`
    if (this.directorStatusDataBuffer.length == this.directorStatusDataBufferMaxPoints)
    {
      this.directorStatusDataBuffer.shift()
    }

    this.directorStatusDataBuffer.forEach((dataPoint)=>
    {
      descriptor.xAxisData.push(this.datePipe.transform(dataPoint.timestamp, environment.dateTimeFormat))
      console.log(dataPoint)
      descriptor.yWorkerAxisData.push(from(dataPoint.registeredWorkers).count())
      descriptor.yTotalJobsAxisData.push(dataPoint.totalJobs)
      var activejobs = 0
      dataPoint.registeredWorkers.forEach(element=>
        {
          activejobs = activejobs + element.currentJobs
        })
      descriptor.yJobsAxisData.push(activejobs)
    })
    return descriptor

  }

  public sendJoinDirectorMonitorGroup()
  {
    this.hub.hubSend("JoinGroup", "directorMonitor");
    // this.addLog( {
    //   message: "User connected viewer to log stream",
    //   messageType: LogTypeEnum.Warning,
    //   program: "Application"
    // })
  }
}
