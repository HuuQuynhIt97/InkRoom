/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { TotalInkModalComponent } from './totalInkModal.component';

describe('TotalInkModalComponent', () => {
  let component: TotalInkModalComponent;
  let fixture: ComponentFixture<TotalInkModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TotalInkModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TotalInkModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
