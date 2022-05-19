import { Component, OnInit } from '@angular/core';
import { NavigationService } from 'src/app/Func_Navigation/navigation.service';

@Component({
  selector: 'app-not-found',
  templateUrl: './not-found.component.html',
  styleUrls: ['./not-found.component.css']
})
export class NotFoundComponent implements OnInit {

  error: any;

  constructor(
    private navService: NavigationService
  ) {

   }

  ngOnInit(): void {
  }

}
