import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { SetDirectorParametersRequest } from 'src/app/_models/API_Messages/SetDirectorParameters';
import { DirectorStatusService } from '../director-status.service';

@Component({
  selector: 'app-director-set-parameters',
  templateUrl: './director-set-parameters.component.html',
  styleUrls: ['./director-set-parameters.component.css']
})
export class DirectorSetParametersComponent implements OnInit {

  parameters: SetDirectorParametersRequest

  constructor(private directorService:DirectorStatusService, private toastr:ToastrService) {
    this.parameters = new SetDirectorParametersRequest();
    this.parameters.idleSecondsBeforeScaleDown = 30;
    this.parameters.maxJobsPerWorker = 20;
    this.parameters.maxWorkers = 5;
    this.parameters.scalingEnabled = true;
}

  ngOnInit(): void {
  }

  onSubmit(){
    //console.log("Sending paramters" )
    this.directorService.sendSetDirectorParameters(this.parameters).subscribe();
    
  }

}
