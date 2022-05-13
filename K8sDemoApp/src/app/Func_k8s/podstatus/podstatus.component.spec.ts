import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PodstatusComponent } from './podstatus.component';

describe('PodstatusComponent', () => {
  let component: PodstatusComponent;
  let fixture: ComponentFixture<PodstatusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PodstatusComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PodstatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
