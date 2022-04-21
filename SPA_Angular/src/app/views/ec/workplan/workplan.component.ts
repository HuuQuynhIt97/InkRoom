import { IPart } from './../../../_core/_model/Part';
import { ISchedule } from './../../../_core/_model/SChedule';
import { ScheduleService } from './../../../_core/_service/schedule.service';
import { Component, OnInit, ViewChild, TemplateRef, Input } from '@angular/core';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { GridComponent, Column, QueryCellInfoEventArgs } from '@syncfusion/ej2-angular-grids';
import { BuildingUserService } from 'src/app/_core/_service/building.user.service';
import { environment } from '../../../../environments/environment';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { JwtHelperService } from '@auth0/angular-jwt';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EmitType } from '@syncfusion/ej2-base';
import { Tooltip } from '@syncfusion/ej2-angular-popups';
import { CalendarView } from '@syncfusion/ej2-angular-calendars';
@Component({
  selector: 'app-workplan',
  templateUrl: './workplan.component.html',
  styleUrls: ['./workplan.component.css'],
  providers: [DatePipe]
})
export class WorkplanComponent implements OnInit {

  @Input()
  required: boolean | string
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 20 };
  data: any[];
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
  public ProcessData = [
    {
      id: 1,
      name: 'Spray',
    },
    {
      id: 2,
      name: 'Print',
    },
  ];
  modelPart: IPart = {
    id: 0,
    name: '',
    status: false,
    scheduleID: 0,
    objectID: 0

  };
  scheduleForm: FormGroup;
  public schedule: ISchedule = {
    season:'',
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
  workPlanDate: any = new Date(); 
  jwtHelper = new JwtHelperService();
  public fieldsProcess: object = { text: 'name', value: 'name' };
  public textProcess = 'Select Treament';
  scheduleID: any;
  name: string;
  systemDate: Date;
  apiUrl = environment.apiUrlEC.replace('/api', '') + 'images/workplan-format.png';


  @ViewChild('content', { static: true })
  content: TemplateRef<any>;
  total: any;
  added: any;
  dataExist: any;
  noAdd: any;
  failedAdd: any;
  public start: CalendarView = 'Year';
  public depth: CalendarView = 'Year';
  public format: string = 'MMMM y'
  workPlanDate_system: string;
  time_upload: any;
  locale: string = localStorage.getItem('lang')
  constructor(
    private alertify: AlertifyService,
    private buildingUserService: BuildingUserService,
    public modalService: NgbModal,
    private datePipe: DatePipe,
    private router: Router,
    private scheduleService: ScheduleService,
    private spinner: NgxSpinnerService,
    private formBuilder: FormBuilder,
  ) {
    this.scheduleForm = this.formBuilder.group({
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
    this.excelDownloadUrl = `${environment.apiUrlEC}workplan/ExcelExport`;
    this.toolbar = [ 'Search'];
    this.filterSettings = { type: 'Excel' };
    this.editSettings = { allowEditing: false, allowAdding: false, allowDeleting: false, newRowPosition: 'Normal' };
    this.getAllUsers();
  }

  export() {
    this.spinner.show();
    this.scheduleService.export(this.failedAdd).subscribe((data: any) => {
      const blob = new Blob([data],
        { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      const downloadURL = window.URL.createObjectURL(data);
      const link = document.createElement('a');
      link.href = downloadURL;
      link.download = `WorkPlan.xlsx`;
      link.click();
      this.spinner.hide();
    });
    
  }

  onChangeSystemDate(args) {
    this.systemDate = args.value as Date
    this.workPlanDate_system = this.datePipe.transform(args.value,'yyyy-MM')
    if (this.systemDate !== null)
      this.getAllByDate();
  }

  onClickDefault() {
    this.systemDate = null
    this.workPlanDate_system = null
    this.getAll()
  }

  getAllByDate() {
    this.spinner.show();
      setTimeout(() =>{
        this.scheduleService.getAllWorkPlanWithDate(this.systemDate.toDateString()).subscribe( (res: any) => {
          this.data = res.result ;
          this.time_upload = res.time_upload ;
          // this.data = res.map(item => {
          //   return {
          //     id: item.id,
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
          //     parts: item.parts,
          //     approvalBy: item.approvalBy,
          //     createdBy: item.createdBy,
          //     processID: item.processID,
          //   };
          // });
        this.spinner.hide();
      });
    }, 300)
  }

  search(args) {
    this.gridObj.search(this.name)
  }

  onChangeDate(args) {
    this.workPlanDate = this.datePipe.transform(args.value,'yyyy-MM-dd')
    
  }

  public queryCellInfoEvent: EmitType<QueryCellInfoEventArgs> = (args: QueryCellInfoEventArgs) => {
    const data = args.data as any;
    const fields = ['line','modelName', 'modelNo', 'articleNo'];
    if (fields.includes(args.column.field)) {
      args.rowSpan = this.data.filter(
        item => item.modelName === data.modelName &&
        item.line === data.line &&
        item.modelNo === data.modelNo &&
        item.articleNo === data.articleNo
      ).length;
    }
  }

  actionCompletePart(e: any): void {
    if (e.requestType === 'add') {
      (e.form.elements.namedItem('name') as HTMLInputElement).focus();
      (e.form.elements.namedItem('id') as HTMLInputElement).disabled = true;
    }
  }

  onChange(args,dateValue){
    this.dateValue = this.datePipe.transform(args.value, 'yyyy-MM-dd')
    this.schedule.productionDate = this.dateValue
  }

  onChangeTreatment(args) {
    this.schedule.process = args.value
  }

  save() {
    let decodedToken = this.jwtHelper.decodeToken(localStorage.getItem('token'));
    this.schedule.createdBy = Number(decodedToken.nameid) ;
    this.scheduleForm.value.createdBy = Number(decodedToken.nameid) ;
    this.scheduleForm.value.productionDate = this.datePipe.transform(this.scheduleForm.value.productionDate, 'yyyy-MM-dd')
    this.scheduleService.CreateSchedule(this.scheduleForm.value).subscribe((res: any) => {
      if (res) {
        this.getAll();
        this.modalReference.close();
        this.clearForm();
        this.alertify.success('Add Schedule Success!');
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
    return this.router.navigate([`ink/establish/schedule/detail/${data.id}`]);
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
        this.modelPart.id = 0;
        this.modelPart.name = args.data.name;
        this.modelPart.scheduleID = this.scheduleID;
        this.add(this.modelPart);
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

  recordDoubleClick(args) {
    const url = this.router.serializeUrl(this.router.createUrlTree([`ink/establish/schedule/detail-workplan/${args.id}/${args.treatment}`]))
    window.open(url,'_blank')
  
    // this.GetDetailSchedule(args.rowData.id);
    // this.modalReference = this.modalService.open(editModal, { size: 'xxl' });
  }

  GetDetailSchedule(id) {
    this.scheduleService.GetDetailSchedule(id,this.locale).subscribe((res: any) => {
      this.detailData = res ;
      this.scheduleID = res[0].id ;
      this.detailPartData = res[0].parts ;
    });
  }

  openAddModal(AddModal) {
    this.modalReference = this.modalService.open(AddModal, { size: 'md' });
  }

  excelExportComplete(): void {
    (this.gridObj.columns[0] as Column).visible = false;
    (this.gridObj.columns[11] as Column).visible = false;
  }

  toolbarClick(args) {
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
        (this.gridObj.columns[11] as Column).visible = false;
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
  
  openVerticallyCentered(content) {
    this.modalService.open(content, { centered: true });
  }

  uploadFile() {
    this.spinner.show();
    if (this.file === undefined || this.file.length === 0){
      this.alertify.error('No data upload')
      this.spinner.hide();
      return;
    }
    this.scheduleService.importWorkPlan(this.file, this.datePipe.transform(this.workPlanDate, 'yyyy-MM-dd'))
    .subscribe((res: any) => {
      if(res.status) {
        this.modalReference.close();
        this.getAll();
        this.total = res.total
        this.added = res.added
        this.dataExist = res.dataExist
        this.noAdd = res.noAdd
        this.failedAdd = res.failedAdd
        // this.alertify.success('The excel has been imported into system!');
        this.openVerticallyCentered(this.content) ;
        // this.alertify.success2(`The excel has been imported into system 
        // <br> Total rows in excel: ${res.total} 
        // <br> Successfully saved: ${res.added}
        // <br> Duplicate: ${res.dataExist}
        // <br> Failure to add: ${res.noAdd}
        // `, true);
        this.spinner.hide();
      } else {
        this.alertify.error('The uploaded file has a date format error, please check the format and re-upload')
        this.spinner.hide();
      }
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
      setTimeout(() =>{
        this.scheduleService.getAllWorkPlan().subscribe( (res: any) => {
          this.data = res.result ;
          this.time_upload = res.time_upload ;
          // this.data = res.map(item => {
          //   return {
          //     id: item.id,
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
          //     parts: item.parts,
          //     approvalBy: item.approvalBy,
          //     createdBy: item.createdBy,
          //     processID: item.processID,
          //   };
          // });
        this.spinner.hide();
      });
    }, 300)
  }

  tooltip(args) {
    if (args.column.field === 'modelName' && args.data.status === false) {
      const tooltip: Tooltip = new Tooltip({
          content: 'Recipe No Data'
      }, args.cell as HTMLTableCellElement);
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
