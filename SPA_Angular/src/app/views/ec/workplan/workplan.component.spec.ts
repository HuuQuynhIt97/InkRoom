import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { WorkplanComponent } from './workplan.component';

describe('WorkplanComponent', () => {
  let component: WorkplanComponent;
  let fixture: ComponentFixture<WorkplanComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkplanComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkplanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
