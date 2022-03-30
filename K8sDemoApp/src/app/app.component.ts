import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { DemoService } from './services/demoservice';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'K8sDemoApp';
  response = "No data loaded, yet";
  constructor(private http: HttpClient, private demoService: DemoService) 
  { 
    //this.http.get('http://localhost/demo', {responseType: 'text'}).subscribe((response: any) => {
    //  console.log(response);
	  //this.response = response;		

    this.http.get('http://localhost/GetTestData', {responseType: 'text'}).subscribe((response: any) => {
      console.log(response);
	  this.response = response;	

	});
  }  


  sendRabbitMessageApi() {

    this.demoService.sendDemoRabbitMessage().subscribe(data =>{
    });
  }


}
