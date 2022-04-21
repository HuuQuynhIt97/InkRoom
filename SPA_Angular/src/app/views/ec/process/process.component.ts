import { IProcess } from './../../../_core/_model/Process';
import { TreatmentService } from './../../../_core/_service/treatment.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { GridComponent } from '@syncfusion/ej2-angular-grids';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-process',
  templateUrl: './process.component.html',
  styleUrls: ['./process.component.scss']
})
export class ProcessComponent implements OnInit {

  public pageSettings = { pageCount: 20, pageSizes: true, currentPage: 1, pageSize: 10 };
  public toolbarOptions = [ 'Add', 'Delete', 'Cancel', 'Search'];
  public editSettings = { showDeleteConfirmDialog: false, allowEditing: true, allowAdding: true, allowDeleting: true, mode: 'Normal' };
  public data: object[];
  filterSettings = { type: 'Excel' };
  
  public ProcessData: any = [];
  public textProcess = 'Select Treament';
  public fieldsSup: object = { text: 'name', value: 'name' };
  @ViewChild('grid') grid: GridComponent;
  public textGlueLineName = 'Select ';
  public treatmentWay: object[];
  public setFocus: any;
  nameDefault: any;
  processDefault: any;
  color: string = '#17a2b8'
  modelProcess: IProcess = {
    id: 0,
    name: null,
    color: null
  };
  colorDefault: any;
  constructor(
    private alertify: AlertifyService,
    public modalService: NgbModal,
    private treatmentWayService: TreatmentService
  ) { }

  ngOnInit(): void {
    this.getAllProcess();
  }

  getAllProcess() {
    this.treatmentWayService.getAllProcess().subscribe((res: any) => {
      this.ProcessData = res;
    });
  }

  actionBegin(args) {
    let nameNew = null;
    let coloNew = null;
    if (args.requestType === "beginEdit" ) {
      this.nameDefault = args.rowData.name ;
      this.colorDefault = args.rowData.color ;
      this.processDefault = args.rowData.process;
      this.modelProcess.id = args.rowData.id || 0;
      this.modelProcess.name = args.rowData.name;
      this.modelProcess.color = args.rowData.color;
    }
    if (args.requestType === 'save') {
      if (args.action === 'edit') {
        this.modelProcess.id = args.data.id || 0 ;
        this.modelProcess.name = args.data.name ;
        this.modelProcess.color = this.color ;
        const dataSource = this.grid.dataSource as any
        const exist = dataSource.filter(x => x.name === args.data.name && args.data.color === this.color)
        if (exist.length > 0) {
          this.alertify.error("Data already exists");
          args.cancel = true;
          return ;
        }
         nameNew = args.data.name ;
         coloNew = this.color;
        if (this.nameDefault !== nameNew || this.colorDefault !== coloNew  ) {
          this.update(this.modelProcess) ;
        } else {
          this.grid.refresh() ;
        }
      }
      if (args.action === 'add') {
        const dataSource = this.grid.dataSource as any
        const exist = dataSource.filter(x => x.name === args.data.name)
        if (args.data.name === undefined ) {
          this.alertify.error("Please Input Treatment");
          args.cancel = true;
          return ;
        }
        if (exist.length > 0) {
          this.alertify.error("Data already exists");
          args.cancel = true;
          return ;
        }
        this.modelProcess.id = 0 ;
        this.modelProcess.name = args.data.name ;
        this.modelProcess.color = this.color;
        if (args.data.name !== undefined) {
          this.add(this.modelProcess) ;
        } else {
          this.getAllProcess() ;
          this.grid.refresh() ;
        }
      }
    }
    if (args.requestType === 'delete') {
      this.delete(args.data[0].id) ;
    }
  }

  toolbarClick(args): void {
    switch (args.item.text) {
      /* tslint:disable */
      case 'Excel Export':
        this.grid.excelExport();
        break;
      /* tslint:enable */
      default:
        break;
    }
  }

  actionComplete(e: any): void {
    if (e.requestType === 'add') {
      (e.form.elements.namedItem('name') as HTMLInputElement).focus();
      (e.form.elements.namedItem('id') as HTMLInputElement).disabled = true;
    }
    if (e.requestType === 'beginEdit') {
      (e.form.elements.namedItem('name') as HTMLInputElement).focus();
      (e.form.elements.namedItem('id') as HTMLInputElement).disabled = true;
    }
  }

  onDoubleClick(args: any): void {
    this.setFocus = args.column;  // Get the column from Double click event
  }

  delete(id) {
    this.alertify.delete("Delete Process",'Are you sure you want to delete this Process "' + id + '" ?')
    .then((result) => {
      if (result) {
        this.treatmentWayService.deleteProcess(id).subscribe(() => {
          this.alertify.success("Process has been deleted");
          this.getAllProcess();
        })
      }
    })
    .catch((err) => {
      this.getAllProcess();
      this.grid.refresh();
    });

  }

  update(modalSup) {
    this.treatmentWayService.updateProcess(modalSup).subscribe(res => {
      this.alertify.success('Updated successfully!');
      this.getAllProcess();
      this.color = '#17a2b8'
    });
  }

  add(modalSup) {
    this.treatmentWayService.createProcess(modalSup).subscribe(() => {
      this.alertify.success('Add Process successfully');
      this.getAllProcess();
      this.modelProcess.name = '';
      this.color = '#17a2b8'
    });
  }

  save() {
    this.treatmentWayService.createProcess(this.modelProcess).subscribe(() => {
      this.alertify.success('Add Process successfully');
      this.getAllProcess();
      this.modelProcess.name = '';
    });
  }

  NO(index) {
    return (this.grid.pageSettings.currentPage - 1) * this.grid.pageSettings.pageSize + Number(index) + 1;
  }

}
