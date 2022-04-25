import { Component, OnInit } from '@angular/core';
import { PageSettingsModel } from '@syncfusion/ej2-angular-grids';

@Component({
  selector: 'app-setting-workplan',
  templateUrl: './setting-workplan.component.html',
  styleUrls: ['./setting-workplan.component.css']
})
export class SettingWorkplanComponent implements OnInit {
  data: any = [];
  searchSettings: any = { hierarchyMode: 'Parent' };
  toolbarOptions = ['Add', 'Edit', 'Delete', 'Cancel', 'Search'];
  sortSettings = { columns: [{ field: 'buildingName', direction: 'Ascending' }] };
  selectOptions: object;
  pageSettings: PageSettingsModel;
  editSettings = { showDeleteConfirmDialog: false, allowEditing: true, allowAdding: true, allowDeleting: true, mode: 'Normal' };
  constructor() { }

  ngOnInit() {
  }
  onClickDefault(){

  }
  onDoubleClick(args: any): void {
  }
  toolbarClick(args: any) {

  }
  actionFailure(e: any): void {
  }
  actionBegin(args) {

  }
  rowDataBound(args) {

  }
  actionComplete(args) {

  }

  dataBound() {

  }
}
