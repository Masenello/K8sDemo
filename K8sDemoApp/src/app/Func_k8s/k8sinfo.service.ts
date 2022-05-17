import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { PodInfoDto } from '../_models/API_Messages/PodInfo';
import { PodLogDto } from '../_models/API_Messages/PodLogDto';

@Injectable({
  providedIn: 'root'
})
export class K8sinfoService {

  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {
    
  }

  getPodInfo():Observable<PodInfoDto[]>{
    return this.http.get<PodInfoDto[]>(this.baseUrl + "k8s/GetPodInfo",{});
  }

  
  getPodLog(podname:string):Observable<PodLogDto>{
    return this.http.get<PodLogDto>(this.baseUrl + "k8s/GetPodLog/"+podname,{});
  }
}
