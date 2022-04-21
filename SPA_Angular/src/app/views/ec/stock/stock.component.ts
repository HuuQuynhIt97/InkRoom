import { StockService } from './../../../_core/_service/stock.service';
import { Component, OnInit, ViewChild, ElementRef} from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { DisplayTextModel } from '@syncfusion/ej2-angular-barcode-generator';
import { IngredientService } from 'src/app/_core/_service/ingredient.service';
import { DatePipe } from '@angular/common';
import { GridComponent } from '@syncfusion/ej2-angular-grids';

@Component({
  selector: 'app-stock',
  templateUrl: './stock.component.html',
  styleUrls: ['./stock.component.css'],
  providers: [
    DatePipe
  ]
})
export class StockComponent implements OnInit {

  @ViewChild('scanQRCode') scanQRCodeElement: ElementRef;
  public displayTextMethod: DisplayTextModel = {
    visibility: false
  };
  // public filterSettings: object;
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 20 };
  @ViewChild('grid') public grid: GridComponent;
  // toolbarOptions: string[];
  @ViewChild('scanText', { static: false }) scanText: ElementRef;
  @ViewChild('ingredientinfoGrid') ingredientinfoGrid: GridComponent;
  qrcodeChange: any;
  data: [];
  dataOut: [];
  checkout = false;
  checkin = true;
  public ingredients: any = [];
  test: any = 'form-control w3-light-grey';
  checkCode: boolean;
  autofocus: boolean = false ;
  subName: string ;
  toolbarOptions = ['Search'];
  filterSettings = { type: 'Excel' };
  constructor(
    public modalService: NgbModal,
    private alertify: AlertifyService,
    private datePipe: DatePipe,
    public ingredientService: IngredientService,
    public stockService: StockService,
  ) { }
  public ngOnInit(): void {
    this.GetAllStock();
    this.getAllInkChemical();
  }

  NO(index) {
    return (this.ingredientinfoGrid.pageSettings.currentPage - 1) * this.ingredientinfoGrid.pageSettings.pageSize + Number(index) + 1;
  }

  dataBound() {

  }

  OutputChange(args) {
    this.checkin = false;
    this.checkout = true;
    this.GetAllStock();
  }

  InputChange(args) {
    this.checkin = true;
    this.checkout = false;
    this.GetAllStock();
    // this.qrcodeChange = null ;
  }

  toolbarClick(args): void {
    switch (args.item.text) {
      /* tslint:disable */
      case 'Excel Export':
        this.grid.excelExport();
        break;
      /* tslint:enable */
      case 'PDF Export':
        break;
    }
  }

  // sau khi scan input thay doi
  async onNgModelChangeScanQRCode(args) {
    const input = args.split('-') ;
    const barcode = args.split('-')[2] ;
    if (input.length !== 3) {
      return ;
    }
    if (input[2].length !== 8) {
      return ;
    }
    const levels = [1, 2, 3, 4] ;
    const building = JSON.parse(localStorage.getItem('level')) ;
    let buildingName = building.name ;
    if (levels.includes(building.level)) {
      buildingName = 'E' ;
    }
    this.findIngredientCode(barcode) ;
    if (this.checkin === true) {
      if (this.checkCode === true) {
        const userID = JSON.parse(localStorage.getItem('user')).User.ID ;
        this.stockService.ScanInput(args, this.subName, buildingName, userID).subscribe((res: any) => {
          if (res === true) {
            this.GetAllStock() ;
          }
        }) ;
      } else {
        this.alertify.error('Wrong Chemical!') ;
      }
    } else {

      if (this.checkCode === true) {
        const userID = JSON.parse(localStorage.getItem('user')).User.ID ;
        this.stockService.ScanOutput(args, this.subName, buildingName, userID).subscribe((res: any) => {
          if (res === true) {
            this.GetAllStock() ;
          } else {
            this.alertify.error(res.message) ;
          }
        }) ;
      } else {
        this.alertify.error('Wrong Chemical!') ;
      }
    }
  }

  // load tat danh sach trong stock trong ngay hom nay
  GetAllStock() {
    this.stockService.getStocks().subscribe((res: any) => {
      this.data = res;
    });
  }

  // tim Qrcode dang scan co ton tai khong
  findIngredientCode(code) {
    for (const item of this.ingredients) {
      if (item.materialNo === code) {
        // return true;
        this.checkCode = true;
        this.subName = item.subname ;
        break;
      } else {
        this.checkCode = false;
      }
    }
  }

  // lay toan bo danh sach muc & hoa chat
  getAllInkChemical() {
    this.stockService.getInkChemicals().subscribe((res: any) => {
      this.ingredients = res;
    });
  }

  // // dung de convert color input khi scan nhung chua can dung
  // ConvertClass(res) {
  //   if (res.length !== 0) {
  //     this.test = 'form-control success-scan';
  //   } else {
  //     this.test = 'form-control error-scan';
  //     this.alertify.error('Wrong Chemical!');
  //   }
  // }

  // xoa Receiving
  delete(item) {
    this.stockService.delete(item.id).subscribe(() => {
      this.alertify.success('Delete Success!');
      this.GetAllStock();
    });
  }

  // // luu du lieu sau khi scan Qrcode vao IngredientReport
  // confirm() {
  //   this.alertify.confirm('Do you want confirm this', 'Do you want confirm this', () => {
  //     this.alertify.success('Confirm Success');
  //   });
  // }

}
