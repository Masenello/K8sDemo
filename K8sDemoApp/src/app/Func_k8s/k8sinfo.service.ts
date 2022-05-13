import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { PodInfoDto } from '../_models/API_Messages/PodInfo';

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
}
