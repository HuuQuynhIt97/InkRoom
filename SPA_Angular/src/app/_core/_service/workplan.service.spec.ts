/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { WorkplanService } from './workplan.service';

describe('Service: Workplan', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [WorkplanService]
    });
  });

  it('should ...', inject([WorkplanService], (service: WorkplanService) => {
    expect(service).toBeTruthy();
  }));
});
