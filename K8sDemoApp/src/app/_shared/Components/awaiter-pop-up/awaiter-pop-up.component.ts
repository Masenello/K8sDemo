import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-awaiter-pop-up',
  templateUrl: './awaiter-pop-up.component.html',
  styleUrls: ['./awaiter-pop-up.component.css']
})
export class AwaiterPopUpComponent implements OnInit {

  private _show: boolean;

  @Input() message = '';
  @Input() msStartDelay = 0;
  @Input() containerClass = '';

  @Input() set show(value: boolean) {
    this._show = value;
    if (value) {
      this.startDelay();
    }
    else {
      this.actualVisibility = value;
    }
  }
  get show(): boolean {
    return this._show;
  }

  actualVisibility: boolean;

  constructor() { }

  ngOnInit(): void {
  }

  async startDelay() {
    await new Promise(x => setTimeout(x, this.msStartDelay));
    if (this.show) { this.actualVisibility = true; }
  }

}
