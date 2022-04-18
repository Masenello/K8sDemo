import { Injectable } from '@angular/core';
import { from } from 'linq-to-typescript';
import { Subject } from 'rxjs/internal/Subject';
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

  buildChartData(targetJobType:JobTypeEnum): jobChartDescriptor
  {
    var descriptor: jobChartDescriptor = new jobChartDescriptor()
    if (this.directorStatusDataBuffer.length == this.directorStatusDataBufferMaxPoints)
    {
      this.directorStatusDataBuffer.shift()
    }

    this.directorStatusDataBuffer.forEach((dataPoint)=>
    {
      descriptor.xAxisData.push(new Date())
      descriptor.yWorkerAxisData.push(from(dataPoint.registeredWorkers).where(x=>x.workerJobType == targetJobType).count())
      var jobsOfTargetType = from(dataPoint.jobsList).firstOrDefault(x=>x.jobType == targetJobType)
      if (jobsOfTargetType == null)
      {
        descriptor.yJobsAxisData.push(0)
      }
      else
      {
        descriptor.yJobsAxisData.push(jobsOfTargetType.jobCount)
      }
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
