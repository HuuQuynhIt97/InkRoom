import { InkModalComponent } from './ink-modal/ink-modal.component';
import { InkService } from './../../../_core/_service/ink.service';
import { Component, OnInit, ViewChild } from '@angular/core';

import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ActivatedRoute } from '@angular/router';
import { Pagination } from 'src/app/_core/_model/pagination';
import { IngredientService } from 'src/app/_core/_service/ingredient.service';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { ModalNameService } from 'src/app/_core/_service/modal-name.service';
import { environment } from '../../../../environments/environment';
import { DisplayTextModel } from '@syncfusion/ej2-angular-barcode-generator';
import { Column, ColumnModel, ExcelExportProperties, GridComponent, RowDDService } from '@syncfusion/ej2-angular-grids';
import { DatePipe } from '@angular/common';
import { IInk } from 'src/app/_core/_model/Ink';
import { NgxSpinnerService } from 'ngx-spinner';
declare let $: any;
const CURRENT_DATE = new Date();
@Component({
  selector: 'app-ink',
  templateUrl: './ink.component.html',
  styleUrls: ['./ink.component.css'],
  providers: [DatePipe, RowDDService]
})
export class InkComponent implements OnInit {

  editSettings = { showDeleteConfirmDialog: false, allowEditing: true, mode: 'Normal' };
  pageSettings = { pageCount: 20, pageSizes: true, currentPage: 1, pageSize: 20 };
  data: any;
  dataInk: any;
  destData: object[] = [];
  modalReference: NgbModalRef;
  excelDownloadUrl: string;
  defaultDate = new Date(null);
  currentDate = new Date();
  srcDropOptions = { targetID: 'DestGrid' };
  public displayTextMethod: DisplayTextModel = {
    visibility: false
  };
  destDropOptions = { targetID: 'Grid' };
  public mfg = this.datePipe.transform(new Date(), 'yyyyMMdd');
  public exp = this.datePipe.transform(new Date(new Date().setMonth(new Date().getMonth() + 4)), 'yyyyMMdd');
  ink: IInk = {
    id: 0,
    name: '',
    nameEn: '',
    code: '',
    createdDate: new Date(),
    supplierID: 0,
    allow: 0,
    voc: 0,
    processID: 0,
    createdBy: 0,
    expiredTime: 0,
    daysToExpiration: 0,
    materialNO: '',
    unit: 0
  };
  pagination: Pagination;
  page = 1;
  currentPage = 1;
  itemsPerPage = 15;
  totalItems: any;
  file: any;
  toolbar = ['Search'];
  text: any;
  filterSettings: any;
  toolbarOptions: any;
  @ViewChild('InkGrid')
  public InkGrid: GridComponent;
  show: boolean ;
  InkShow: boolean = true;
  pd: Date;
  dataPicked: Array<{
    id: number,
    code: string,
    name: string,
    supplier: string,
    supplierID: number,
    batch: string,
    expiredTime: number,
    productionDate: Date,
    daysToExpiration: number,
    exp: string,
    qrCode: string
  }> = [];
  @ViewChild('printGrid')
  public printGrid: GridComponent;
  dataPrint: Array<{
    id: number,
    code: string,
    name: string,
    unit: number,
    supplier: string,
    supplierID: number,
    batch: string,
    expiredTime: number,
    productionDate: Date,
    daysToExpiration: number,
    exp: string,
    qrCode: string
  }> = [];
  locale: string = localStorage.getItem('lang')
  constructor(
    private modalNameService: ModalNameService,
    public modalService: NgbModal,
    private ingredientService: IngredientService,
    private alertify: AlertifyService,
    private datePipe: DatePipe,
    private route: ActivatedRoute,
    private spinner: NgxSpinnerService,
    private inkService: InkService
  ) { }

  ngOnInit() {
    this.filterSettings = { type: 'Excel' };
    this.toolbarOptions = ['ExcelExport','Print QR Code', 'Search'];
    this.excelDownloadUrl = `${environment.apiUrlEC}Ink/ExcelExport`;
    this.inkService.currentInk.subscribe(res => {
      if (res === 300) {
        this.getAll();
        this.ink = {
          id: 0,
          name: '',
          nameEn: '',
          code: '',
          createdDate: new Date(),
          supplierID: 0,
          allow: 0,
          voc: 0,
          processID: 0,
          createdBy: 0,
          expiredTime: 0,
          daysToExpiration: 0,
          materialNO: '',
          unit: 0
        };
      }
    });
    this.getAll();
  }

  printData() {
    let html = '';
    for (const item of this.dataPicked) {
      const content = document.getElementById(item.code);
      html += `
       <div class='content'>
        <div class='qrcode'>
         ${content.innerHTML}
         </div>
          <div class='info'>
          <ul>
            <li class='subInfo'>Name: ${item.name}</li>
              <li class='subInfo'>QR Code: ${this.datePipe.transform(item.productionDate, 'yyyyMMdd')}-${item.batch}-${item.code}</li>
              <li class='subInfo'>MFG: ${this.datePipe.transform(item.productionDate, 'yyyyMMdd')}</li>
              <li class='subInfo'>EXP: ${item.exp}</li>
          </ul>
         </div>
      </div>
      `;
    }
    this.configurePrint(html);
  }

  backList() {
    this.show = false;
    this.InkShow = true;
    this.dataPicked = [];
    // this.printGrid = [] ;
  }

  configurePrint(html) {
    const WindowPrt = window.open('', '_blank', 'left=0,top=0,width=1000,height=900,toolbar=0,scrollbars=0,status=0');
    WindowPrt.document.write(`
    <html>
      <head>
      </head>
      <style>
          body {
        width: 100%;
        height: 100%;
        margin: 0;
        padding: 0;
        background-color: #FAFAFA;
        font: 12pt "Tahoma";
    }
    * {
        box-sizing: border-box;
        -moz-box-sizing: border-box;
    }
    .page {
        width: 210mm;
        min-height: 297mm;
        padding: 20mm;
        margin: 10mm auto;
        border: 1px #D3D3D3 solid;
        border-radius: 5px;
        background: white;
        box-shadow: 0 0 5px rgba(0, 0, 0, 0.1);
    }
    .subpage {
        padding: 1cm;
        border: 5px red solid;
        height: 257mm;
        outline: 2cm #FFEAEA solid;
    }
     .content {
        height: 221px;
         border: 1px #D3D3D3 solid;
    }
    .content .qrcode {
      float:left;

      width: 177px;
      height:177px;
      margin-top: 12px;
      margin-bottom: 12px;
      margin-left: 5px;
       border: 1px #D3D3D3 solid;
    }
    .content .info {
       float:left;
       list-style: none;
    }
    .content .info ul {
       float:left;
       list-style: none;
       padding: 0;
       margin: 0;
      margin-top: 25px;
    }
    .content .info ul li.subInfo {
      padding: .20rem .75rem;
    }
    @page {
        size: A4;
        margin: 0;
    }
    @media print {
        html, body {
            width: 210mm;
            height: 297mm;
        }
        .page {
            margin: 0;
            border: initial;
            border-radius: initial;
            width: initial;
            min-height: initial;
            box-shadow: initial;
            background: initial;
            page-break-after: always;
        }
    }
      </style>
      <body onload="window.print(); window.close()">
        ${html}
      </body>
    </html>
    `);
    WindowPrt.document.close();
  }

  toolbarClickPrint(args): void {
    switch (args.item.text) {
      case 'Excel Export':
        const data = this.dataPrint.map(item => {
          return {
            supplier: item.supplier,
            name: item.name,
            unit: item.unit,
            qrCode: item.qrCode
          };
        });
        const supplierModel: ColumnModel =
        {
          field: 'supplier',
          headerText: 'Supplier',
          textAlign: 'Center',
          autoFit: true,
          width: 120,
        };
        const nameModel: ColumnModel =
        {
          field: 'name',
          headerText: 'Ingredient',
          textAlign: 'Center',
          autoFit: true,
          width: 120,
        };
        const unitModel: ColumnModel =
        {
          field: 'unit',
          headerText: 'Unit',
          textAlign: 'Center',
          autoFit: true,
          width: 80,
        };
        const qrCoderModel: ColumnModel = {
          field: 'qrCode',
          headerText: 'QR Code',
          textAlign: 'Center',
          autoFit: true,
          width: 200,
        };
        const excelExportProperties: ExcelExportProperties = {
          dataSource: data,
          columns: [
            new Column(supplierModel),
            new Column(nameModel),
            new Column(unitModel),
            new Column(qrCoderModel)
          ],
        };
        this.printGrid.excelExport(excelExportProperties);
        break;
        case 'Print QR Code': this.printBarcode(); break;
        default:
        break;
    }
  }

  printBarcode() {
    this.show = true;
    this.InkShow = false;
    this.getAllInk();

  }

  rowSelected(args) {
    setTimeout(() => {
      this.dataPicked = this.printGrid?.getSelectedRecords() as any;
    }, 300);
  }

  rowDeselected(args) {
    setTimeout(() => {
      this.dataPicked = this.printGrid?.getSelectedRecords() as any;
    }, 300);
  }

  createdSearch(args) {
    var gridElement = this.InkGrid.element;
    var span = document.createElement("span");
    span.className = "e-clear-icon";
    span.id = gridElement.id + "clear";
    span.onclick = this.cancelBtnClick.bind(this);
    gridElement.querySelector(".e-toolbar-item .e-input-group").appendChild(span);
  }

  public cancelBtnClick(args) {
    this.InkGrid.searchSettings.key = "";
    (this.InkGrid.element.querySelector(".e-input-group.e-search .e-input") as any).value = "";
  }

  ngAfterViewInit() {
    $('[data-toggle="tooltip"]').tooltip();
  }

  dataBound() {
  }

  makeid(length) {
    let result = '';
    const characters = '0123456789';
    const charactersLength = characters.length;
    for ( let i = 0; i < length; i++ ) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
   // return '59129032';
  }

  toolbarClick(args): void {
    switch (args.item.text) {
      /* tslint:disable */
      case 'Excel Export':
        this.InkGrid.excelExport();
        break;
      /* tslint:enable */
      case 'PDF Export':
        break;
      case 'Print QR Code': this.printBarcode(); break;
    }
  }

  actionBegin(args) {
    if (args.requestType === 'save' && args.action === 'edit') {
    }
  }

  getAll() {
    this.spinner.show()
    this.inkService.getInks().subscribe(res => {
      this.dataInk = res;
      this.spinner.hide()
    });
  }
  getAllInk() {
    this.inkService.getInks().subscribe((res: any) => {
      this.dataPrint = res.map((item: any) => {
        return {
          id: item.id,
          code: item.materialNO,
          name: item.name,
          supplier: item.supplier,
          supplierID: item.supplierID,
          batch: 'DEFAULT',
          unit: item.unit,
          expiredTime: item.expiredTime,
          daysToExpiration: item.daysToExpiration,
          productionDate: new Date(),
          qrCode: `${this.datePipe.transform(new Date(), 'yyyyMMdd')}-DEFAULT-${item.materialNO}`,
          exp: this.datePipe.transform(new Date(new Date().setDate(new Date().getDate() + item.daysToExpiration)), 'yyyyMMdd')
        };
      });
    });
  }

  delete(data) {
    this.alertify.confirm('Delete Ink', 'Are you sure you want to delete this Ink "' + data.name + '" ?', () => {
      this.inkService.delete(data.id).subscribe(() => {
        this.getAll();
        this.alertify.success('Ink has been deleted');
      }, error => {
        this.alertify.error('Failed to delete the Ink');
      });
    });
  }

  tooltip(data) {
    if (data) {
      return data;
    } else {
      return '';
    }
  }

  openInkModalComponent() {
    const modalRef = this.modalService.open(InkModalComponent, { size: 'md' });
    modalRef.componentInstance.ink = this.ink;
    modalRef.componentInstance.title = 'Add Ink';
    modalRef.result.then((result) => {
    }, (reason) => {
    });
  }

  openInkEditModalComponent(item) {
    const modalRef = this.modalService.open(InkModalComponent, { size: 'md' });
    modalRef.componentInstance.ink = item;
    modalRef.componentInstance.title = 'Edit Ink';
    modalRef.result.then((result) => {
    }, (reason) => {
    });
  }

  fileProgress(event) {
    this.file = event.target.files[0];
  }

  uploadFile() {
    this.spinner.show()
    const createdBy = JSON.parse(localStorage.getItem('user')).User.ID;
    this.inkService.import(this.file, createdBy).subscribe((res: any) => {
      this.getAll();
      this.alertify.success('The excel has been imported into system!');
      this.spinner.hide()
      this.modalReference.close();
    });
  }

  showModal(name) {
    this.modalReference = this.modalService.open(name, { size: 'xl' });
  }

  NO(index) {
    return (this.InkGrid.pageSettings.currentPage - 1) * this.InkGrid.pageSettings.pageSize + Number(index) + 1;
  }

}
