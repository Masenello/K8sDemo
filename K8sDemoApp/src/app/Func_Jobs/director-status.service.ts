import { DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { from } from 'linq-to-typescript';
import { Observable } from 'rxjs/internal/Observable';
import { map } from 'rxjs/internal/operators/map';
import { Subject } from 'rxjs/internal/Subject';
import { environment } from 'src/environments/environment';
import { AccountService } from '../Func_Login/account.service';
import { HubService } from '../services/hub.service';
import { JobTypeEnum } from '../_enum/JobTypeEnum';
import { SetDirectorParametersRequest } from '../_models/API_Messages/SetDirectorParametersRequest';
import { DirectorStatusMessage } from '../_models/Hub_Messages/DirectorStatusMessage';
import { jobChartDescriptor } from '../_models/jobChartDescriptor';

@Injectable({
  providedIn: 'root'
})
export class DirectorStatusService {

  baseUrl = environment.apiUrl + "DirectorManagement/";

  public newDirectorStatus : Subject<DirectorStatusMessage> = new Subject<DirectorStatusMessage> ();
  datePipe = new DatePipe('en-US');
  directorStatusDataBuffer:Array<DirectorStatusMessage>  = [];
  directorStatusDataBufferMaxPoints = 200

  constructor(private hub: HubService, private http: HttpClient) {
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
    descriptor.jobChartTitle = `Active jobs`
    descriptor.workersChartTitle = `Registered workers`
    //Remove older data when Max points is reached
    if (this.directorStatusDataBuffer.length == this.directorStatusDataBufferMaxPoints)
    {
      this.directorStatusDataBuffer.shift()
    }

    this.directorStatusDataBuffer.forEach((dataPoint)=>
    {
      descriptor.xAxisData.push(this.datePipe.transform(dataPoint.timestamp, environment.dateTimeFormat))
      //console.log(dataPoint)

      descriptor.yWorkerAxisData.push(from(dataPoint.registeredWorkers).count())
      descriptor.yMaxWorkersAxisData.push(dataPoint.maxWorkers)
      descriptor.yTotalJobsAxisData.push(dataPoint.totalJobs)
      var activejobs = 0
      dataPoint.registeredWorkers.forEach(element=>
        {
          activejobs = activejobs + element.currentJobs
        })
      descriptor.yJobsAxisData.push(activejobs)
      descriptor.yMaxJobsAxisData.push(dataPoint.maxConcurrentTasks)
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

  sendSetDirectorParameters(directorParameters: SetDirectorParametersRequest):Observable<any>
  {
    console.log(directorParameters);
    console.log(this.baseUrl +  "SetDirectorParameters")
    return this.http.post(this.baseUrl + 
      "SetDirectorParameters", directorParameters)
    }



}
