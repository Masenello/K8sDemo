import { environment } from 'src/environments/environment';

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})

export class DemoService {

    constructor(private http: HttpClient) { }

    sendDemoRabbitMessage(): Observable<any>{
        return this.http.post('http://localhost/demo/SendTestRabbitMessage', {});
    }
}