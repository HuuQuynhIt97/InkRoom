import { ITreatmentWay, ITreatmentWay2 } from './../../../_core/_model/TreatmentWay';
import { TreatmentService } from './../../../_core/_service/treatment.service';
import { IngredientService } from './../../../_core/_service/ingredient.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { GridComponent } from '@syncfusion/ej2-angular-grids';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-treament-way',
  templateUrl: './treament-way.component.html',
  styleUrls: ['./treament-way.component.css']
})
export class TreamentWayComponent implements OnInit {

  public pageSettings = { pageCount: 20, pageSizes: true, currentPage: 1, pageSize: 10 };
  public toolbarOptions = [ 'Add', 'Delete', 'Cancel', 'Search'];
  public editSettings = { showDeleteConfirmDialog: false, allowEditing: true, allowAdding: true, allowDeleting: true, mode: 'Normal' };
  public data: object[];
  filterSettings = { type: 'Excel' };
  modelTreatmentWay: ITreatmentWay = {
    id: 0,
    name: null,
    nameEn: null,
    Process: null
  };
  public ProcessData: any = [];
  public textProcess = 'Select Treament';
  public fieldsSup: object = { text: 'name', value: 'name' };
  @ViewChild('grid') grid: GridComponent;
  public textGlueLineName = 'Select ';
  public treatmentWay: ITreatmentWay2[];
  public setFocus: any;
  nameDefault: any;
  processDefault: any;
  nameEnDefault: any;
  constructor(
    private alertify: AlertifyService,
    public modalService: NgbModal,
    private ingredientService: IngredientService,
    private treatmentWayService: TreatmentService
  ) { }

  ngOnInit(): void {
    this.getAll();
    this.getAllProcess();
  }

  onChangeTreatment(args) {
    this.modelTreatmentWay.Process = args.value;
  }

  getAll() {
    this.treatmentWayService.getAll().subscribe((res) => {
      console.log(res);
      this.treatmentWay = res;
    });
  }
  getAllProcess() {
    this.treatmentWayService.getAllProcess().subscribe((res: any) => {
      this.ProcessData = res;
    });
  }

  actionBegin(args) {
    let nameNew = null;
    let nameEnNew = null;
    if (args.requestType === "beginEdit" ) {
      this.nameDefault = args.rowData.name ;
      this.nameEnDefault = args.rowData.nameEn ;
      this.processDefault = args.rowData.process;
      this.modelTreatmentWay.id = args.rowData.id || 0;
      this.modelTreatmentWay.name = args.rowData.name;
      this.modelTreatmentWay.nameEn = args.rowData.nameEn;
      this.modelTreatmentWay.Process = args.rowData.process;
    }
    if (args.requestType === 'save') {
      if (args.action === 'edit') {
        this.modelTreatmentWay.id = args.data.id || 0 ;
        this.modelTreatmentWay.name = args.data.name ;
        this.modelTreatmentWay.nameEn = args.data.nameEn ;
        nameNew = args.data.name ;
        nameNew = args.data.nameEn ;
        if (this.nameDefault !== nameNew || this.nameEnDefault !== nameEnNew || this.processDefault !== this.modelTreatmentWay.Process) {
          this.update(this.modelTreatmentWay) ;
        } else {
          this.grid.refresh() ;
        }
      }
      if (args.action === 'add') {
        const dataSource = this.grid.dataSource as any
        const exist = dataSource.filter(x => x.name === args.data.name && x.process === this.modelTreatmentWay.Process)
        if (this.modelTreatmentWay.Process === undefined || this.modelTreatmentWay.Process === null) {
          this.alertify.error("Please select Treatment");
          args.cancel = true;
          return ;
        }
        if (exist.length > 0) {
          this.alertify.error("Data already exists");
          args.cancel = true;
          return ;
        }
        this.modelTreatmentWay.id = 0 ;
        this.modelTreatmentWay.name = args.data.name ;
        if (args.data.name !== undefined && this.modelTreatmentWay.Process !== null) {
          this.add(this.modelTreatmentWay) ;
        } else {
          this.getAll() ;
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
    this.alertify.delete("Delete Treatment Way",'Are you sure you want to delete this Treatment Way "' + id + '" ?')
    .then((result) => {
      if (result) {
        this.treatmentWayService.delete(id).subscribe(() => {
          this.alertify.success("Treatment Way has been deleted");
          this.getAll();
        })
      }
    })
    .catch((err) => {
      this.getAll();
      this.grid.refresh();
    });

  }

  update(modalSup) {
    this.treatmentWayService.update(modalSup).subscribe(res => {
      this.alertify.success('Updated successfully!');
      this.getAll();
    });
  }

  add(modalSup) {
    this.treatmentWayService.create(modalSup).subscribe(() => {
      this.alertify.success('Add Treatment way successfully');
      this.getAll();
      this.modelTreatmentWay.name = '';
      this.modelTreatmentWay.nameEn = '';
    });
  }

  save() {
    this.treatmentWayService.create(this.modelTreatmentWay).subscribe(() => {
      this.alertify.success('Add Treatment way successfully');
      this.getAll();
      this.modelTreatmentWay.name = '';
      this.modelTreatmentWay.nameEn = '';
    });
  }

  NO(index) {
    return (this.grid.pageSettings.currentPage - 1) * this.grid.pageSettings.pageSize + Number(index) + 1;
  }

}
