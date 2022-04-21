import { IPart } from './../../../_core/_model/Part';
import { ISchedule } from './../../../_core/_model/SChedule';
import { ScheduleService } from './../../../_core/_service/schedule.service';
import { Component, OnInit, ViewChild, TemplateRef, Input } from '@angular/core';
import { ModalNameService } from 'src/app/_core/_service/modal-name.service';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { GridComponent, Column } from '@syncfusion/ej2-angular-grids';
import { BuildingUserService } from 'src/app/_core/_service/building.user.service';
import { environment } from '../../../../environments/environment';
import { BPFCEstablishService } from 'src/app/_core/_service/bpfc-establish.service';
import { DatePipe } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { JwtHelperService } from '@auth0/angular-jwt';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TreatmentService } from 'src/app/_core/_service/treatment.service';
import { CalendarView } from '@syncfusion/ej2-angular-calendars';
@Component({
  selector: 'app-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.css'],
  providers: [DatePipe]
})
export class ScheduleComponent implements OnInit {
  @Input()
  required: boolean | string
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 20 };
  data: any[];
  dataTamp: any[];
  editSettings: object;
  toolbar: object;
  file: any;
  detailData: [];
  detailPartData: [];
  @ViewChild('grid')
  public gridObj: GridComponent;
  modalReference: NgbModalRef;
  modalAddReference: NgbModalRef;
  @ViewChild('importModal', { static: true })
  importModal: TemplateRef<any>;
  @ViewChild('AddModal', { static: true })
  AddModal: TemplateRef<any>;
  editModal: TemplateRef<any>;
  excelDownloadUrl: string;
  users: any[];
  filterSettings: { type: string; };
  editSettingsPartSchedule = {
    showDeleteConfirmDialog: false,
    allowEditing: true,
    allowAdding: true,
    allowDeleting: true,
    mode: 'Normal',
  };
  editSettingsSchedule = {
    showDeleteConfirmDialog: false,
    allowEditing: true,
    allowAdding: true,
    allowDeleting: true,
    mode: 'Normal',
  };
  toolbarEditSchedule = [
    'Cancel',
  ];
  toolbarEditPartSchedule = [
    'Add',
    'Delete',
    'Cancel',
  ];
  public ProcessData: any = [];
  modalPart: IPart = {
    id: 0,
    name: '',
    status: false,
    scheduleID: 0,
    objectID: 0

  };
  scheduleForm: FormGroup;
  public schedule: ISchedule = {
    season: '',
    modelName: '',
    modelNo: '',
    articleNo: '',
    object: '',
    createdBy: 0,
    process: '',
    part: '',
    listPart: [],
    productionDate: ''
  }
  items: any ;
  public dateValue: any = new Date();
  jwtHelper = new JwtHelperService();
  public fieldsProcess: object = { text: 'name', value: 'name' };
  public textProcess = 'Select Treament';
  scheduleID: any;
  name: string;
  treatmentData: object
  color: string = '#17a2b8'
  public flag: boolean = false;
  scheduleDate: any = new Date(); ;
  systemDate: Date;
  public start: CalendarView = 'Year';
  public depth: CalendarView = 'Year';
  public format: string = 'MMMM y'
  constructor(
    private alertify: AlertifyService,
    private buildingUserService: BuildingUserService,
    public modalService: NgbModal,
    private datePipe: DatePipe,
    private router: Router,
    private scheduleService: ScheduleService,
    private spinner: NgxSpinnerService,
    private formBuilder: FormBuilder,
    private treatmentWayService: TreatmentService
  ) {
    this.scheduleForm = this.formBuilder.group({
      season: [null],
      modelName : [null, Validators.required],
      modelNo : [null, Validators.required],
      articleNo : [null, Validators.required],
      object : [null, Validators.required],
      process : [null, Validators.required],
      listPart : [[]],
      createdBy : [0],
      productionDate : [new Date()],
    })
  }

  ngOnInit() {
    this.excelDownloadUrl = `${environment.apiUrlEC}schedule/ExcelExport`;
    this.toolbar = ['Import Excel', 'Export Excel', 'Search' , 'Add'];
    this.filterSettings = { type: 'Excel' };
    this.editSettings = { allowEditing: false, allowAdding: true, allowDeleting: false, newRowPosition: 'Normal' };
    this.getAllUsers();
    this.getAllTreatment();
  }
  onClickDefault() {
    this.systemDate = null
    this.getAll()
  }
  onChangeDate(args) {
    this.scheduleDate = this.datePipe.transform(args.value,'yyyy-MM-dd')
  }
  onChangeSystemDate(args) {
    this.systemDate = args.value as Date
    if (this.systemDate !== null)
      this.getAllByDate();
  }
  search(args) {
    this.gridObj.search(this.name)
  }
  getAllByDate() {
    this.spinner.show();
      setTimeout(() =>{
        this.scheduleService.getAllWithDate(this.systemDate.toDateString()).subscribe( (res: any) => {
          this.data = res.map(item => {
            return {
              id: item.id,
              approvalStatus: item.approvalStatus === true ? 'Yes' : 'No',
              articleNo: item.articleNo,
              establishDate: this.datePipe.transform(item.establishDate, 'yyyy-MM-dd'),
              productionDate: this.datePipe.transform(item.productionDate, 'yyyy-MM-dd'),
              artProcess: item.artProcess,
              finishedStatus: item.finishedStatus === true ? 'Yes' : 'No',
              modelNo: item.modelNo,
              modelName: item.modelName,
              season: item.season,
              treatment: item.treatment,
              process: item.process,
              parts: item.parts,
              approvalBy: item.approvalBy,
              createdBy: item.createdBy,
              processID: item.processID,
              color: item.color,
            };
          });
          this.dataTamp = this.data
        this.spinner.hide();
      });
    }, 1000)
  }
  getAllTreatment() {
    this.treatmentWayService.getAllProcess().subscribe(res => {
      this.treatmentData = res
    })
  }
  public dataBound(): void {
    this.flag = true;
  }
  
  filterTreatment(item) {
    if (!this.flag) { return; }
    if(item.name === 'Print')
    {
      this.gridObj.hideColumns('Process')
      this.flag = true;
    } else {
      this.gridObj.showColumns('Process')
    }
    this.data = this.dataTamp.filter(x => x.treatment === item.name)
  }
  filterDefault() {
    this.data = this.dataTamp
  }
 
  onClickToolbar(args) {
    const target: HTMLElement = (args.target as HTMLElement).closest('button');

    switch (target?.id) {
      case 'Import Excel':
        this.showModal(this.importModal) ;
        break;
      case 'Add':
        args.cancel = true ;
        this.openAddModal(this.AddModal) ;
        break ;
      case 'Export Excel':
        // const data = this.data.map(item => {
        //   return {
        //     approvalStatus: item.approvalStatus === true ? 'Yes' : 'No',
        //     articleNo: item.articleNo,
        //     establishDate: this.datePipe.transform(item.establishDate, 'yyyy-MM-dd'),
        //     productionDate: this.datePipe.transform(item.productionDate, 'yyyy-MM-dd'),
        //     artProcess: item.artProcess,
        //     finishedStatus: item.finishedStatus === true ? 'Yes' : 'No',
        //     modelNo: item.modelNo,
        //     modelName: item.modelName,
        //     treatment: item.treatment,
        //     process: item.process,
        //     parts: item.parts.join(' - ') || '#N/A',
        //   };
        // });
        (this.gridObj.columns[0] as Column).visible = false;
        (this.gridObj.columns[10] as Column).visible = false;
        const exportProperties = {
          // dataSource: data,
          fileName: 'ScheduleData.xlsx'
        };
        this.gridObj.excelExport(exportProperties);
        break;
    }
  }
  getAllProcess() {
    this.treatmentWayService.getAllProcess().subscribe((res: any) => {
      this.ProcessData = res;
    });
  }
  actionCompletePart(e: any): void {
    if (e.requestType === 'add') {
      (e.form.elements.namedItem('name') as HTMLInputElement).focus();
      (e.form.elements.namedItem('id') as HTMLInputElement).disabled = true;
    }
  }

  onChange(args, dateValue){
    this.dateValue = this.datePipe.transform(args.value, 'yyyy-MM-dd')
    this.schedule.productionDate = this.dateValue
  }

  onChangeTreatment(args) {
    this.schedule.process = args.value
  }

  save() {
    this.spinner.show()
    let decodedToken = this.jwtHelper.decodeToken(localStorage.getItem('token'));
    this.schedule.createdBy = Number(decodedToken.nameid) ;

    this.scheduleForm.value.createdBy = Number(decodedToken.nameid) ;
    this.scheduleForm.value.productionDate = this.datePipe.transform(this.scheduleForm.value.productionDate, 'yyyy-MM-dd')
    this.scheduleService.CreateSchedule(this.scheduleForm.value).subscribe((res: any) => {
      if (res) {
        this.alertify.success('Add Schedule Success!');
        this.getAll();
        this.modalReference.close();
        this.clearForm();
        this.spinner.hide()
      }
      else {
        this.alertify.error('System Error!');
      }
    })
  }

  ProductionDateChange(args,data) {
    const ProductionDateChange = this.datePipe.transform(args.value, 'yyyy-MM-dd')
    if (args.isInteracted) {
      this.scheduleService.ProductionDateChange(ProductionDateChange , data.id).subscribe((res) => {
        if (res) {
          this.alertify.success('Update Success')
          this.getAll();
          this.GetDetailSchedule(data.id);
        }
      })
    }
  }

  clearForm() {
    this.schedule.season = '',
    this.schedule.modelName = '',
    this.schedule.modelNo = '',
    this.schedule.articleNo = '',
    this.schedule.object = '',
    this.schedule.createdBy = 0,
    this.schedule.process = '',
    this.schedule.part = '',
    this.schedule.productionDate = ''
  }

  createdSearch(args) {
    var gridElement = this.gridObj.element;
    var span = document.createElement("span");
    span.className = "e-clear-icon";
    span.id = gridElement.id + "clear";
    span.onclick = this.cancelBtnClick.bind(this);
    gridElement.querySelector(".e-toolbar-item .e-input-group").appendChild(span);
  }

  public cancelBtnClick(args) {
    this.gridObj.searchSettings.key = "";
    (this.gridObj.element.querySelector(".e-input-group.e-search .e-input") as any).value = "";
  }

  delete(data) {
    this.alertify.confirm('Delete Schedule', 'Are you sure you want to delete this Schedule "' + data.id + '" ?', () => {
      this.scheduleService.DeleteSchedule(data.id).subscribe(() => {
        this.getAllUsers();
        this.alertify.success('Schedule has been deleted');
      }, error => {
        this.alertify.error('Failed to delete the Schedule');
      });
    });
  }

  detail(data) {
    const url = this.router.serializeUrl(this.router.createUrlTree([`ink/establish/schedule/detail/${data.id}`]))
    window.open(url,'_blank')
    // return this.router.navigate([`ink/establish/schedule/detail/${data.id}`]);
  }

  actionBegin(args) {

  }

  actionBeginEditSchedule(args) {
    if (args.requestType === 'save') {
      this.scheduleService.EditSchedule(args.data).subscribe((res: any) => {
        if (res) {
          this.alertify.success('Update Schedule Success');
          this.getAll();
        }
      });
    }
  }

  rowDeselectedEdit(args) {
  }

  actionBeginSchedulePart(args) {

    if (args.requestType === 'save') {
      if (args.action === 'edit') {
        this.scheduleService.EditPartSchedule(args.data).subscribe((res: any) => {
          if (res) {
            this.alertify.success('Update Schedule Success');
            this.getAll();
          }
        });
      }
      if (args.action === 'add') {
        this.modalPart.id = 0;
        this.modalPart.name = args.data.name;
        this.modalPart.scheduleID = this.scheduleID;
        this.add(this.modalPart);
      }
    }
    if (args.requestType === 'delete') {
      this.deletePart(args.data[0].id);
    }

  }

  deletePart(id) {
    this.scheduleService.deletePart(id).subscribe((res) => {
      this.alertify.success('Delete Part Success')
      this.getAll();
      this.GetDetailSchedule(this.scheduleID);
    })
  }

  add(modalPart) {
    this.scheduleService.addPart(modalPart).subscribe(() => {
      this.alertify.success('Add successfully');
      this.getAll();
      this.GetDetailSchedule(this.scheduleID);
    });
  }

  recordDoubleClick(args, editModal) {
    this.GetDetailSchedule(args.rowData.id);
    this.modalReference = this.modalService.open(editModal, { size: 'xxl' });
  }

  GetDetailSchedule(id) {
    this.scheduleService.getDetailScheduleEdit(id).subscribe((res: any) => {
      console.log(res);
      this.detailData = res;
      this.scheduleID = res[0].id
      this.detailPartData = res[0].parts;
    });
  }

  openAddModal(AddModal) {
    this.getAllProcess();
    this.modalReference = this.modalService.open(AddModal, { size: 'md' });
  }

  excelExportComplete(): void {
    (this.gridObj.columns[0] as Column).visible = false;
    (this.gridObj.columns[10] as Column).visible = false;
  }

  toolbarClick(args) {
    switch (args.item.text) {
      case 'Import Excel':
        this.showModal(this.importModal) ;
        break;
      case 'Add':
        args.cancel = true ;
        this.openAddModal(this.AddModal) ;
        break ;
      case 'Export Excel':
        // const data = this.data.map(item => {
        //   return {
        //     approvalStatus: item.approvalStatus === true ? 'Yes' : 'No',
        //     articleNo: item.articleNo,
        //     establishDate: this.datePipe.transform(item.establishDate, 'yyyy-MM-dd'),
        //     productionDate: this.datePipe.transform(item.productionDate, 'yyyy-MM-dd'),
        //     artProcess: item.artProcess,
        //     finishedStatus: item.finishedStatus === true ? 'Yes' : 'No',
        //     modelNo: item.modelNo,
        //     modelName: item.modelName,
        //     treatment: item.treatment,
        //     process: item.process,
        //     parts: item.parts.join(' - ') || '#N/A',
        //   };
        // });
        (this.gridObj.columns[0] as Column).visible = false;
        (this.gridObj.columns[10] as Column).visible = false;
        const exportProperties = {
          // dataSource: data,
          fileName: 'ScheduleData.xlsx'
        };
        this.gridObj.excelExport(exportProperties);
        break;
    }
  }

  fileProgress(event) {
    this.file = event.target.files[0];
  }

  uploadFile() {
    this.spinner.show()
    if (this.file === undefined || this.file.length === 0){
      this.alertify.error('No data upload')
      this.spinner.hide();
      return;
    }
    const createdBy = JSON.parse(localStorage.getItem('user')).User.ID;
    this.scheduleService.import(this.file, createdBy, this.datePipe.transform(this.scheduleDate, 'yyyy-MM-dd'))
    .subscribe((res: any) => {
      this.getAll();
      this.modalReference.close();
      this.alertify.success('The excel has been imported into system!');
      this.spinner.hide()
    });
  }

  getAllUsers() {
    this.buildingUserService.getAllUsers(1, 1000).subscribe((res: any) => {
      this.users = res.result;
      this.getAll();
    });
  }

  getAll() {
    this.spinner.show();
    this.scheduleService.getAll().subscribe( (res: any) => {
      this.data = res.map(item => {
        return {
          id: item.id,
          approvalStatus: item.approvalStatus === true ? 'Yes' : 'No',
          articleNo: item.articleNo,
          establishDate: this.datePipe.transform(item.establishDate, 'yyyy-MM-dd'),
          productionDate: this.datePipe.transform(item.productionDate, 'yyyy-MM-dd'),
          artProcess: item.artProcess,
          finishedStatus: item.finishedStatus === true ? 'Yes' : 'No',
          modelNo: item.modelNo,
          modelName: item.modelName,
          season: item.season,
          treatment: item.treatment,
          process: item.process,
          parts: item.parts,
          approvalBy: item.approvalBy,
          createdBy: item.createdBy,
          processID: item.processID,
          color: item.color,
        };
      });
      this.dataTamp = this.data
      this.spinner.hide();
    });
    //   setTimeout(() =>{
    // }, 1000)
  }

  tooltip(data) {
    if (data) {
      return data.join(' - ');
    } else {
      return '';
    }
  }

  tooltips(data) {
    if (data) {
      return data;
    } else {
      return '';
    }
  }

  showModal(importModal) {
    this.modalReference = this.modalService.open(importModal, { size: 'xl' });
    this.modalReference.result.then((result) => {
      this.file = []
    }, (reason) => {
      this.file = []
    });
  }

  showModalEdit(editModal) {
    this.modalReference = this.modalService.open(editModal, { size: 'xl' });
  }

  NO(index) {
    return (this.gridObj.pageSettings.currentPage - 1) * this.gridObj.pageSettings.pageSize + Number(index) + 1;
  }

}
