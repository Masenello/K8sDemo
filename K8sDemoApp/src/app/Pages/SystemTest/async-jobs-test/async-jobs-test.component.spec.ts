import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AsyncJobsTestComponent } from './async-jobs-test.component';

describe('AsyncJobsTestComponent', () => {
  let component: AsyncJobsTestComponent;
  let fixture: ComponentFixture<AsyncJobsTestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AsyncJobsTestComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AsyncJobsTestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
