/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { PrintQrcodeWorkplanComponent } from './print-qrcode-workplan.component';

describe('PrintQrcodeWorkplanComponent', () => {
  let component: PrintQrcodeWorkplanComponent;
  let fixture: ComponentFixture<PrintQrcodeWorkplanComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrintQrcodeWorkplanComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintQrcodeWorkplanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
