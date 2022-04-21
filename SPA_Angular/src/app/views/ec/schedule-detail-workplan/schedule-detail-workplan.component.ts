import { async } from '@angular/core/testing';
import { TotalChemicalModalComponent } from './totalChemicalModal/totalChemicalModal.component';
import { TotalInkModalComponent } from './totalInkModal/totalInkModal.component';
import { WorkplanService } from './../../../_core/_service/workplan.service';
import { IGlues } from "./../../../_core/_model/glues";
import { GlueService } from "./../../../_core/_service/glue.service";
import { PartService } from "src/app/_core/_service/part.service";
import { IPart } from "./../../../_core/_model/Part";
import { Component, OnInit, ViewChild } from "@angular/core";
import { GridComponent, QueryCellInfoEventArgs } from "@syncfusion/ej2-angular-grids";
import { ScheduleService } from "src/app/_core/_service/schedule.service";
import { ActivatedRoute, Router } from "@angular/router";
import { AlertifyService } from "src/app/_core/_service/alertify.service";
import { CheckBoxComponent } from "@syncfusion/ej2-angular-buttons";
import { Tooltip } from "@syncfusion/ej2-angular-popups";
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-schedule-detail-workplan',
  templateUrl: './schedule-detail-workplan.component.html',
  styleUrls: ['./schedule-detail-workplan.component.css']
})
export class ScheduleDetailWorkplanComponent implements OnInit {

  partData: any = [];
  particularData: any = []
  glueData: any = [];
  POdata: any =  [];
  RoleName: string;
  ScheduleID: number;
  treatment: string;
  public fieldsPosition: object = { text: "name", value: "id" };
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  modelName: string;
  modelNo: string;
  articleNo: string;
  ProcessName: string;
  ObjectName: string;
  @ViewChild("grid")
  public gridPart: GridComponent;
  inkParam: string
  @ViewChild("gridTreetment")
  public gridTreetment: GridComponent;

  @ViewChild("grid")
  public grid: GridComponent;

  public checked: boolean = false;
  showQrcode: boolean = false;
  public particular: boolean = false
  colorName: any;
  glueDataTamp: any;
  qty: any;
  item_part: any;
  POdata_tamp: any;
  locale: string = localStorage.getItem('lang')
  buildingName: string = null
  constructor(
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private scheduleService: ScheduleService,
    private router: Router,
    public modalService: NgbModal,
    private spinner: NgxSpinnerService,
    private workPlanService: WorkplanService
  ) {}

  ngOnInit(): void {
    this.ScheduleID = this.route.snapshot.params.id;
    this.treatment = this.route.snapshot.params.treatment;
    this.GetDetailSchedule();
    this.getAsyncData()
    this.RoleName = JSON.parse(localStorage.getItem("role")).name;
    this.buildingName = JSON.parse(localStorage.getItem("building"));
  }

  doubleClick(args) {
    if(args.column.field === 'totalInk') {
      const item = args.rowData.itemOfInk
      this.showModal(TotalInkModalComponent,item,'lg')
    }
    if(args.column.field === 'totalChemical') {
      const item = args.rowData.itemOfChemical
      this.showModal(TotalChemicalModalComponent,item,'lg')
    }
  }

  showModal(model,data,size){
    const modalRef = this.modalService.open(model, { size: size, centered: true, backdrop: 'static', keyboard: false });
    modalRef.componentInstance.result = data;
    modalRef.result.then((result) => {
    }, (reason) => {
    });
  }
  tooltip(args: QueryCellInfoEventArgs) {
    if (args.column.field === 'glueName' || args.column.field === 'name') {
      const tooltip: Tooltip = new Tooltip({
          content: args.data[args.column.field]
      }, args.cell as HTMLTableCellElement);
    }
  }

  tooltipTreetment(args: QueryCellInfoEventArgs) {
    if (args.column.field === 'treatmentWay' || args.column.field === 'name') {
      const tooltip: Tooltip = new Tooltip({
          content: args.data[args.column.field]
      }, args.cell as HTMLTableCellElement);
    }
  }

  dataBound(args) {
    
    this.grid.selectRow(0); 
    this.particularData = []
    this.checked = true
    this.particular = false
    if(this.treatment === 'Print')
    {
      this.grid.showColumns('Parts')
    } else {
      this.grid.hideColumns('Parts')
    }
  
  }
  goToPrintQrCode() {
    console.log(this.inkParam);
    var navigateTo = `ink/establish/schedule/detail-workplan/${this.ScheduleID}/${this.treatment}/print-qrcode/${this.inkParam}`;
    const url = this.router.serializeUrl(this.router.createUrlTree([navigateTo]))
    window.open(url,'_blank')
  }
  
  goToPrintGlue(args) {
    var navigateTo = `ink/establish/schedule/detail-workplan/${this.ScheduleID}/${this.treatment}/` + 'print-glue';
    var navigationExtras = {
      queryParams: { 
        qty: args[0].consumption, 
        article: this.articleNo, 
        recipe: args[0].recipe,
        modelName: this.modelName,
        part: args[0].part,
      },
      replaceUrl: true, // optional
    };
    const url = this.router.serializeUrl(this.router.createUrlTree([navigateTo], navigationExtras))
    window.open(url,'_blank')
  }
  dataBoundTreetment() {
   if(this.treatment === 'Print')
    {
      this.gridTreetment.showColumns('QR Code')
    } else {
      this.gridTreetment.hideColumns('QR Code')
    }
  }

  async updatePoGlue(data) {
    this.scheduleService.updatePoGlue(data.id).subscribe(async res => {
      if (res) {
        this.alertify.success('success')
        await this.getPONumberScheduleAndPart2()
        await this.getPONumberScheduleAndPart()
        // this.getPONumberSchedule()
      }
    })
  }

  async updatePart(data) {
    this.scheduleService.updatePart(data.id , this.item_part.id).subscribe(async res => {
      if (res) {
        this.alertify.success('success')
        await this.getPONumberScheduleAndPart2()
        await this.getPONumberScheduleAndPart()
      }
    })
  }


  getAllGlue() {
    this.workPlanService.getGluesByScheduleId(this.ScheduleID)
    .subscribe((res: any) => {
      this.glueData = res;
      this.glueDataTamp = res;
    });
  }
  getAllGlueWithQty(qty) {
    return new Promise((res, rej) => {
      this.workPlanService.getGluesByScheduleIdWithQtyWithLocale(this.ScheduleID, qty, this.locale).subscribe(
        (result: any) => {
          console.log(result);
          this.glueData = result;
          this.glueDataTamp = result;
          // this.particular = true
          res(result);
        },
        (error) => {
          rej(error);
        }
      );
    });
  }
  async filterGlueByPart(item) {
    this.item_part = item
    this.partData.forEach(element => {
      element.name == item.name ? element.status = true : element.status = false
    });
    this.checked = true
    this.showQrcode = true;
    // setTimeout(() => {
    // }, 300);
    this.glueData = this.glueDataTamp?.filter(x => x.part === item.name)
    // this.inkParam = this.glueData[0].recipe
    this.inkParam = this.glueDataTamp?.filter(x => x.part === item.name)
    this.spinner.hide()
    // if (this.checked === true) {
    // }
  }
  // sortGlueData(item) {
  //   return new Promise((res, rej) => {
  //     this.glueData = this.glueDataTamp?.filter(x => x.part === item.name);
  //     this.inkParam = this.glueDataTamp?.filter(x => x.part === item.name)[0];
  //     res(this.glueData);
  //   });
  // }
  async filterGlueByPartDefault(item) {
    this.item_part = item
    this.partData.forEach(element => {
      element.name == item.name ? element.status = true : element.status = false
    });
    this.checked = true
    this.showQrcode = true;
    // setTimeout(() => {
    //   console.log(this.glueData);
    // }, 300);
    this.glueData = this.glueDataTamp?.filter(x => x.part === item.name)
    // this.inkParam = this.glueData[0].recipe
    this.inkParam = this.glueDataTamp?.filter(x => x.part === item.name)
    // await this.getPONumberScheduleAndPart2()  
    // await this.getPONumberScheduleAndPart()  
    this.spinner.hide()  
  }
  GetDetailSchedule() {
    this.scheduleService
      .GetDetailSchedule(this.ScheduleID,this.locale)
      .subscribe((res: any) => {
        this.modelName = res.modelName;
        this.modelNo = res.modelNo;
        this.articleNo = res.articleNo;
        this.ProcessName = res.treatment;
        this.ObjectName = res.process;
        this.colorName = res.color;
        // this.partData = res.parts;
    });
  }
  async getAsyncData() {
    await this.getPartByScheduleID();
    await this.getPONumberScheduleAndPart2();
    await this.getPONumberScheduleAndPart();
    // await this.getPONumberSchedule();
  }

  getPartByScheduleID() {
    return new Promise((res, rej) => {
      this.scheduleService.getPartByScheduleID(this.ScheduleID).subscribe(
        (result: any) => {
          this.partData = result;
          this.item_part = this.partData[0];
          res(result);
        },
        (error) => {
          rej(error);
        }
      );
    });
   
  }
  getPONumberSchedule() {
    this.scheduleService.getPONumberScheduleID(this.ScheduleID, this.treatment).toPromise().then((res: any) => {
      this.POdata = res
    })
  }

  getPONumberScheduleAndPart() {
    return new Promise((res, rej) => {
      this.scheduleService.getPONumberScheduleIDAndPart(this.ScheduleID, this.treatment, this.item_part.id).subscribe(
        (result: any) => {
          const data = result.map((item: any) => {
            return {
              glueName: item.glueName,
              id: item.id,
              line: item.line,
              name: item.name,
              partID: item.partID,
              qty: item.qty,
              ps: item.ps,
              status: item.status,
              treatment: item.treatment,
              partStatus: this.StatusTemplate(item.partID, item.id),
            };
          });
          this.POdata = data;
          res(data);
        },
        (error) => {
          rej(error);
        }
      );
    });
    
  }
  StatusTemplate(partId, id): boolean {
    const data = this.POdata_tamp.filter(
      (item: any) => item.partID === partId && item.id == id
    ) as any[];
    if (data.length === 0) {
      return false;
    }
    const stt = data[0].partStatus || 0;

    const Status = stt;
    return Status || false;
  }
  getPONumberScheduleAndPart2() {
    return new Promise((res, rej) => {
      this.scheduleService.getPONumberScheduleIDAndPart2(this.ScheduleID, this.treatment, this.item_part.id).subscribe(
        (result: any) => {
          this.POdata_tamp = result;
          this.filterGlueByPart(this.item_part);
          res(result);
        },
        (error) => {
          rej(error);
        }
      );
    });
  }
  
  rowSelectedParticular(args) {
    this.checked = true
    // this.getAllGlue();
    this.getAllGlueWithQty(args.data.qty)
  }
  async rowSelected(args: any) {
    this.spinner.show()
    this.qty = args.data.qty
    // this.checked = false
    this.particularData = []
    if(args.isInteracted)
      this.particular = true;
    await this.getAllGlueWithQty(this.qty)
    this.filterGlueByPart(this.item_part)
    const model = {
      name: args.data.glueName,
      qty: args.data.qty
    }
    this.particularData.push(model)
    // if (this.treatment === 'Print') {
    //   this.checked = true
    //   this.getAllGlue();
    // }
  }

  NO(index) {
    return (
      (this.gridPart.pageSettings.currentPage - 1) * this.gridPart.pageSettings.pageSize + Number(index) + 1);
    }
}
