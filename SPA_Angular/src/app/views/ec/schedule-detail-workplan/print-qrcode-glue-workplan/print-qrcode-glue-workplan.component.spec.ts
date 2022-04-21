/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { PrintQrcodeGlueWorkplanComponent } from './print-qrcode-glue-workplan.component';

describe('PrintQrcodeGlueWorkplanComponent', () => {
  let component: PrintQrcodeGlueWorkplanComponent;
  let fixture: ComponentFixture<PrintQrcodeGlueWorkplanComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintQrcodeGlueWorkplanComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintQrcodeGlueWorkplanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
