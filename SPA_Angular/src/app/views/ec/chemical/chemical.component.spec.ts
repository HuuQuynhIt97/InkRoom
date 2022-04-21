import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ChemicalComponent } from './chemical.component';

describe('ChemicalComponent', () => {
  let component: ChemicalComponent;
  let fixture: ComponentFixture<ChemicalComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ChemicalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChemicalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
