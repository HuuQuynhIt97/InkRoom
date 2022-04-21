import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ChemicalModalComponent } from './chemical-modal.component';

describe('ChemicalModalComponent', () => {
  let component: ChemicalModalComponent;
  let fixture: ComponentFixture<ChemicalModalComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ChemicalModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChemicalModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
