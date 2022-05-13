import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PodsInfoComponent } from './pods-info.component';

describe('PodsInfoComponent', () => {
  let component: PodsInfoComponent;
  let fixture: ComponentFixture<PodsInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PodsInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PodsInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
