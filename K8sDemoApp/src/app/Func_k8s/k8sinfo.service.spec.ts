import { TestBed } from '@angular/core/testing';

import { K8sinfoService } from './k8sinfo.service';

describe('K8sinfoService', () => {
  let service: K8sinfoService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(K8sinfoService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
