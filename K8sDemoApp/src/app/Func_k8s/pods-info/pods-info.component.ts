import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs/internal/Observable';
import { PodInfoDto } from 'src/app/_models/API_Messages/PodInfo';
import { environment } from 'src/environments/environment';
import { K8sinfoService } from '../k8sinfo.service';

@Component({
  selector: 'app-pods-info',
  templateUrl: './pods-info.component.html',
  styleUrls: ['./pods-info.component.css']
})
export class PodsInfoComponent implements OnInit {

  podInfoList: PodInfoDto[]
  kubernetesDashboard : string = environment.kubernetesDashboard
  constructor(private k8sInfoService: K8sinfoService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.getPodInfo()
  }

  getPodInfo()
  {
    this.k8sInfoService.getPodInfo().subscribe((info) => {
      this.podInfoList = info
      this.toastr.success("Pod information retrieved from Kubernetes API")
    }, (error) => {
      console.log(error)
      this.toastr.error("Failed to read pod information")
    });
  }

  openKubernetesDashboard()
  {
    window.open(this.kubernetesDashboard, "_blank");
  }

}
