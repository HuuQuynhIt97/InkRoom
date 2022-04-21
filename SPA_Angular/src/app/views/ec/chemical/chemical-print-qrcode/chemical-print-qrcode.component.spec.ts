import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ChemicalPrintQrcodeComponent } from './chemical-print-qrcode.component';

describe('ChemicalPrintQrcodeComponent', () => {
  let component: ChemicalPrintQrcodeComponent;
  let fixture: ComponentFixture<ChemicalPrintQrcodeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ChemicalPrintQrcodeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChemicalPrintQrcodeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
