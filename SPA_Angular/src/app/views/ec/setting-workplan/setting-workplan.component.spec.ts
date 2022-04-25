/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SettingWorkplanComponent } from './setting-workplan.component';

describe('SettingWorkplanComponent', () => {
  let component: SettingWorkplanComponent;
  let fixture: ComponentFixture<SettingWorkplanComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SettingWorkplanComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingWorkplanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
