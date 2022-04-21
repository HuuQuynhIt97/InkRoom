import { EmitType } from '@syncfusion/ej2-base';
import { IGlues } from "./../../../_core/_model/glues";
import { GlueService } from "./../../../_core/_service/glue.service";
import { PartService } from "src/app/_core/_service/part.service";
import { IPart } from "./../../../_core/_model/Part";
import { Component, OnInit, ViewChild, ViewEncapsulation } from "@angular/core";
import { GridComponent, QueryCellInfoEventArgs } from "@syncfusion/ej2-angular-grids";
import { ScheduleService } from "src/app/_core/_service/schedule.service";
import { ActivatedRoute } from "@angular/router";
import { AlertifyService } from "src/app/_core/_service/alertify.service";
import { CheckBoxComponent } from "@syncfusion/ej2-angular-buttons";
import { Tooltip } from "@syncfusion/ej2-angular-popups";
@Component({
  selector: "app-schedule-detail",
  templateUrl: "./schedule-detail.component.html",
  styleUrls: ["./schedule-detail.component.css"],
  encapsulation: ViewEncapsulation.None
})
export class ScheduleDetailComponent implements OnInit {
  partData: any = [];
  treatmentWayData: any = [];
  glueData: any = [];
  glueDataTamp: any = [];
  supData: [];
  previewData: any = [];
  RoleName: string;
  textPreview: string;
  chemicalData: any;
  chemicalDataDefault: any = [];
  saveData: any = [];
  saveDataTamp: any = [];
  public editSettings = {
    showDeleteConfirmDialog: false,
    allowEditing: true,
    allowAdding: true,
    allowDeleting: true,
    mode: "Normal"
  };
  editSettingss = {
    allowEditing: true,
    autoFit: true,
    mode: 'Batch'
  };
  editSettingsCheckbox = {
    showDeleteConfirmDialog: false,
    allowEditing: false,
    allowAdding: false,
    allowDeleting: false,
    autoFit: false,
    mode: "Normal",
  };
  editSettingsDefault = {
    showDeleteConfirmDialog: false,
    allowEditing: false,
    allowAdding: false,
    allowDeleting: false,
    autoFit: true,
    mode: "Normal",
  };
  ScheduleID: number;
  toolbar = [
    {
      text: "Add New",
      tooltipText: "Add new row",
      prefixIcon: "e-add",
      id: "AddNew",
    },
    "Delete",
    "Cancel",
    "Search",
  ];
  editparamsChemical: any = { params: { format: "n" } };
  toolbarChemical = [
    // {
    //   text: 'Add New',
    //   tooltipText: 'Add new row',
    //   prefixIcon: 'e-add',
    //   id: 'AddNew',
    // },
    "Cancel",
    "Search",
  ];
  toolbarCheckbox = [
    // 'Search',
  ];
  modalPart: IPart = {
    id: 0,
    name: "",
    objectID: 0,
    status: false,
    scheduleID: 0,
  };
  modalGlues: IGlues = {
    id: 0,
    name: "",
    glueID: 0,
    partID: 0,
    treatmentWayID: 0,
    consumption: "0",
    scheduleID: 0,
    sequence: 0
  };
  public fieldsPosition: object = { text: "name", value: "id" };
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  pageSettingsChemical = { pageCount: 20, pageSizes: true, pageSize: 10 };
  modelName: string;
  modelNo: string;
  articleNo: string;
  ProcessName: string;
  ObjectName: string;
  public rowIndex: any = "";
  isShow: boolean = false;
  isShowDefault: boolean = false;
  @ViewChild("grid")
  public gridPart: GridComponent;
  @ViewChild("gridChemical")
  public gridChemical: GridComponent;

  B: any;
  C: any;
  D: any;
  E: any;
  partID: number;
  treatmentID: number;
  glueID: number;
  namePart: any = "";
  public checked: boolean = false;
  public approveStatus: boolean = false;
  public finishStatus: boolean = false;
  public toolbarOptions = ["Add", "Delete", "Cancel", "Search"];
  filterSettings = { type: "Excel" };
  public checkedwifi: boolean = true;

  public checkModify: boolean = false;
  public textModify: string;
  public partModify: string;
  selectedRow = [];
  selIndex: number[];
  selIndexSequence: number[];
  public checkInkChemicalExist: boolean = false;
  oldDetail: any[];
  modified: boolean = true;
  @ViewChild("checkbox")
  public checkbox: CheckBoxComponent;
  public dropState: any;
  toGlueID: any;
  subId: number;
  chemicalDataTamp: any;
  flag: boolean;
  idBatchSave: number
  color: any;
  locale: string = localStorage.getItem('lang')
  constructor(
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private scheduleService: ScheduleService,
    private glueService: GlueService,
    private partService: PartService
  ) {}

  ngOnInit(): void {
    this.selIndex = [];
    this.selectedRow = [];
    this.removeLocalStore("details");
    this.ScheduleID = this.route.snapshot.params.id;
    this.GetDetailSchedule();
    setTimeout(() => {
      this.getAllGlue();
    }, 400);
    this.RoleName = JSON.parse(localStorage.getItem("role")).name;
    this.oldDetail = [];
  }
  
  cellEdit(args) {
    
    if (args.rowData.status === false) {
      this.alertify.warning("Please select the ink/chemical ! <br> Vui lòng chọn mực/hóa chất", true);
      args.cancel = true;
      return;
    }
    if (args.rowData.status) {
      if (args.rowData.modify === false) {
        this.alertify.warning("Can not modify this chemical <br> Không thể sửa đổi hóa chất này", true);
        args.cancel = true;
        return;
      }
    }
  }
  public keyHandler(e) {
    if (e.keyCode === 13) {
      this.gridChemical.editModule.saveCell();
    }
  }

  cellSave(args) {
    if (args.columnName === 'percentage') {
      
      let details = this.getLocalStore("details");
        for (let i in this.chemicalData) {
          if (this.chemicalData[i].id == args.rowData.id && this.chemicalData[i].subname == args.rowData.subname) {
            this.chemicalData[i].percentage = args.value;
            this.chemicalData[i].consumption = args.rowData.consumption;
            break;
          }
        }
        for (let i in details) {
          if (details[i].id == args.rowData.id && details[i].subname == args.rowData.subname) {
            details[i].percentage = args.value;
            details[i].consumption = args.rowData.consumption;
            break;
          }
        }
      this.setLocalStore("details", details);
    }

    if (args.columnName === 'consumption') {
      
      let details = this.getLocalStore("details");
        for (let i in this.chemicalData) {
          if (this.chemicalData[i].id == args.rowData.id && this.chemicalData[i].subname == args.rowData.subname) {
            this.chemicalData[i].percentage = args.rowData.percentage;
            this.chemicalData[i].consumption = args.value;
            break;
          }
        }
        for (let i in details) {
          if (details[i].id == args.rowData.id && details[i].subname == args.rowData.subname) {
            details[i].percentage = args.rowData.percentage;
            details[i].consumption = args.value;
          }
        }
      this.setLocalStore("details", details);
    }

  }
 
  dataBound(args) {
    // this.flag = true;
  }
  public queryCellInfoEvent: EmitType<QueryCellInfoEventArgs> = (args) => {
    const data = args.data as any ;
    const dataTamp = this.getLocalStore("details").filter(x => x.subname === 'Ink')
    const fields = ['consumption'];
    if (fields.includes(args.column.field) && data.subname === 'Ink' && data.status) {
      args.rowSpan = dataTamp.length;
    }
  }

  filterGlueByPart(item) {
    this.glueData = this.glueDataTamp.filter(x => x.part === item.name)
  }
  tooltip(args: QueryCellInfoEventArgs) {
    if (args.column.field === 'name') {
      const tooltip: Tooltip = new Tooltip({
          content: args.data[args.column.field]
      }, args.cell as HTMLTableCellElement);
    }
  }
  rowDrop(e){
    this.selIndexSequence = [e.dropIndex + 1]
    setTimeout(() => {
      const model = {
        GlueDefaultID: e.data[0].id,
        FromIndex: e.fromIndex + 1,
        ToIndex: e.dropIndex + 1,
        ScheduleID: e.data[0].scheduleID,
        GlueChangeID: this.toGlueID
      }
      this.partService.updateGlueSequence(model).subscribe(res => {
        this.getAllGlue()
      })
    }, 300);

  }
  CheckInkChemicalExist(id) {
    for (const item of this.chemicalDataDefault) {
      if (item.id === id) {
        this.checkInkChemicalExist = true;
        break;
      } else {
        this.checkInkChemicalExist = false;
      }
    }
  }

  public changeHandler(args, data): void {
    this.checkModify = true;
    
    if (args.checked) {
      for (let i in this.chemicalData) {
        if (
          this.chemicalData[i].id == data.id &&
          this.chemicalData[i].subname === data.subname
        ) {
          this.chemicalData[i].status = true;
          if (data.subname === 'Ink') {
            this.GetChemicalBySupplier(this.subId)
          }
          break;
        }
      }
    } else {
      for (let i in this.chemicalData) {
        if (
          this.chemicalData[i].id == data.id &&
          this.chemicalData[i].subname === data.subname
        ) {
          this.chemicalData[i].status = false;
          if (data.subname === 'Ink') {
            this.GetChemicalBySupplier(this.subId)
          }
          break;
        }
      }
    }

    if (args.checked) {
      this.previewData = [];
      this.saveData.push(data);
      const details = this.getLocalStore("details");
      details.push(data);
      this.setLocalStore("details", details);
    } else {
      const details = this.getLocalStore("details");
      for (var i = 0; i < details.length; i++) {
        if (details[i].id == data.id) {
          details.splice(i, 1);
          break;
        }
      }
      this.setLocalStore("details", details);
    }
  }

  ClickSub(item) {
    if (this.gridPart.getSelectedRowIndexes().length === 0) {
      this.alertify.warning("Please select Part first!", true);
    } else {
      if (item === 0) {
        this.previewData = [];
        this.isShow = true;
        this.getInkChemicalByglueID();
      } else {
        this.isShow = true;
        this.subId = item.id
        this.GetChemicalBySupplier(item.id);
      }
    }
  }

  getAllGlue() {
    this.glueService.getGlueByScheduleIDWithLocale(this.ScheduleID, this.locale)
    .subscribe((res: any) => {
      this.glueData = res;
      this.glueDataTamp = res;
    });
  }

  GetChemicalBySupplier(id) {
    this.scheduleService.GetChemicalBySupplier(id).subscribe((res: any) => {
      this.chemicalData = res.map((item, index) => {
        return {
          index: index,
          status: false,
          id: item.id,
          name: item.name,
          subname: item.subname,
          percentage: item.percentage,
          modify: item.modify,
          code: item.code,
          consumption: 0
        };
      });
      let details = this.getLocalStore("details");
      if (details.length > 0) {
        for (let item in this.chemicalData) {
          for (let item2 in details) {
            if (this.chemicalData[item].id == details[item2].id) {
              this.chemicalData[item].status = true;
              this.chemicalData[item].percentage = details[item2].percentage;
              this.chemicalData[item].consumption = details[item2].consumption;
            }
          }
        }
      }
      this.chemicalData = this.chemicalData.sort((a,b)=> b.status - a.status)
      this.isShow = true;
    });
  }

  done() {
    this.scheduleService.done(this.ScheduleID).subscribe(() => {
      this.alertify.success("success");
      this.GetDetailSchedule();
    });
  }

  approve() {
    const userid = JSON.parse(localStorage.getItem("user")).User.ID;
    this.scheduleService.approval(this.ScheduleID, userid).subscribe((res) => {
      this.alertify.success("success");
      this.GetDetailSchedule();
    });
  }

  GetDetailSchedule() {
    this.scheduleService
      .GetDetailSchedule(this.ScheduleID,this.locale)
      .subscribe((res: any) => {
        this.modelName = res.modelName;
        this.color = res.color;
        this.modelNo = res.modelNo;
        this.articleNo = res.articleNo;
        this.ProcessName = res.treatment;
        this.ObjectName = res.process;
        this.partData = res.parts;
        this.treatmentWayData = res.treatmentWay;
        this.supData = res.supplier;
        this.finishStatus = res.finishedStatus;
        this.approveStatus = res.approvalStatus;
        this.modalPart.objectID = res.inkTblObjectID;
      });
  }

  dataBoundPart() {
    if (this.selectedRow.length) {
      this.gridPart.selectRows(this.selectedRow);
    } else {
      this.gridPart.selectRows(this.selIndex);
    }
  }

  toolbarClick(args: any): void {
    switch (args.item.text) {
      case "Add":
        if (this.checkModify === true) {
          this.alertify
            .valid(
              "Warning",
              `Are you sure you want to discard these changes ? <br> Cảnh báo ! Bạn có chắc rằng muốn bỏ qua những thay đổi chưa được lưu của ${this.textModify}`
            )
            .then((result) => {
              this.checkModify = false;
            })
            .catch((err) => {
              args.cancel = false;
              this.gridPart.refresh();
            });
        } else {
        break;
      }
    }
  }

  ChemicalToolbarClick(args: any): void {}

  compareArray(a1: Array<any>, a2: Array<any>) {
    return JSON.stringify(a1) === JSON.stringify(a2);
  }

  rowSelected(args: any) {
    // this.toGlueID = args.data[0].id || 0
    this.selIndex = [args.rowIndex];
    if (!args.isInteracted && args.previousRowIndex) {
      this.selIndex = [args.previousRowIndex];
    }

    const data = args.data[0] || args.data;
    this.glueID = data.id;
    setTimeout(() => {
      this.loadPreview();
      this.partID = data.partID;
      this.treatmentID = data.treatmentWayID;
      this.isShow = true;
      this.partModify = data.part;
      this.getInkChemicalByglueID();
    }, 500);
  }

  setLocalStore(key: string, value: any) {
    localStorage.removeItem(key);
    let details = value || [];
    for (let key in details) {
      details[key].status = true;
    }
    const result = JSON.stringify(details);
    localStorage.setItem(key, result);
    this.loadPreview();
  }

  getLocalStore(key: string) {
    const data = JSON.parse(localStorage.getItem(key)) || [];
    return data;
  }

  getInkChemicalByglueID() {
    const details = this.getLocalStore("details") ;
    if (details.length === 0 || this.checkModify === false) {
      this.glueService
        .GetInkChemicalByGlueIDWithLocale(this.glueID, this.locale)
        .subscribe((res: any) => {
          this.chemicalData = res.map((item, index) => {
            return {
              index: index,
              status: true,
              id: item.id,
              name: item.name,
              subname: item.subname,
              percentage: item.percentage,
              modify: item.modify,
              code: item.code,
              consumption: item.consumption
            } 
          })
          this.chemicalData = this.chemicalData.sort((a, b) => (b.subname > a.subname) ? 1 : -1 && a.index - b.index)
          this.setLocalStore("details", this.chemicalData) 
          this.oldDetail = this.chemicalData 
        }) ;
    } else {
      const details = this.getLocalStore("details") ;
      this.chemicalData = details.sort((a, b) => (b.subname > a.subname) ? 1 : -1 && a.index - b.index) ;
    }
  }

  loadPreview() {
    this.previewData = [];
    this.textPreview = null;
    let details = this.getLocalStore("details") ;
    for (let i = 0; i < details.length; i++) {
      this.previewData.push(details[i].percentage + "%" + details[i].code) ;
      this.textPreview = this.previewData.join(" + ") ;
      this.textModify = this.partModify + " - " + this.textPreview ;
    }
  }


  actionBeginPart(args) {
    if (args.requestType === "beginEdit" ) {

      var tooltips = args.row.querySelectorAll('.e-tooltip');
      for (var i = 0; i < tooltips.length; i++) {
        tooltips[i].ej2_instances[0].destroy();
      }
      this.modalGlues.id = args.rowData.id;
      this.modalGlues.name = args.rowData.name;
      this.modalGlues.partID = args.rowData.partID;
      this.modalGlues.treatmentWayID = args.rowData.treatmentWayID;
      this.modalGlues.consumption = args.rowData.consumption;
      this.modalGlues.sequence = args.rowData.sequence;
      this.modalGlues.scheduleID = Number(this.ScheduleID);
    }

    if (args.requestType === "save" ) {
      if (args.action === "edit") {
        this.modalGlues.consumption = args.data.consumption;
        this.updateGlue(this.modalGlues);
      }
      if (args.action === "add") {
        this.modalGlues.id = 0
        // this.modalGlues.consumption = args.data.consumption;
        this.save();
        this.selectedRow = [this.glueData.length];
      }
    }
    if (args.requestType === "delete") {
      this.delete(args.data[0].id);
    }
  }

  tooltips(data) {
    if (data) {
      return data;
    } else {
      return "";
    }
  }
 
  actionBeginChemical(args) {
    if (args.requestType === "beginEdit") {
      
      if (args.rowData.status === false) {
        this.alertify.warning("Please select the ink/chemical ! <br> Vui lòng chọn mực/hóa chất", true);
        args.cancel = true;
        return;
      }
      if (args.rowData.status) {
        if (args.rowData.modify === false) {
          this.alertify.warning("Can not modify this chemical <br> Không thể sửa đổi hóa chất này", true);
          args.cancel = true;
          return;
        }
      }
    }

    if (args.requestType === "save" && args.action === "edit") {
      let details = this.getLocalStore("details");
      for (let i in this.chemicalData) {
        if (this.chemicalData[i].id == args.data.id && this.chemicalData[i].subname == args.data.subname) {
          this.chemicalData[i].percentage = args.data.percentage;
          this.chemicalData[i].consumption = args.data.consumption;
          break;
        }
      }
      for (let i in details) {
        if (details[i].id == args.data.id && details[i].subname == args.data.subname) {
          details[i].percentage = args.data.percentage;
          details[i].consumption = args.data.consumption;
          break;
        }
      }
      this.setLocalStore("details", details);
    }

  }

  delete(id) {
    this.alertify.delete(
      "Warning",
      'Are you sure you want to delete this Glues "' + id + '" ?'
    )
    .then((result) => {
      if (result) {
        this.glueService.delete(id, this.ScheduleID).subscribe(() => {
          this.getAllGlue();
          this.modalGlues.id = 0
          this.isShow = false;
          this.alertify.success("Glues has been deleted");
        })
      }
    })
    .catch((err) => {
      this.getAllGlue();
      this.gridPart.refresh();
    });

  }

  save() {
    this.partService.create(this.modalGlues).subscribe((res) => {
      this.alertify.success("Create successfully!");
      this.getAllGlue();
      // this.selIndex = [0];
    });
  }

  onChangePosition(args) {
    if (this.checkModify === true) {
      this.alertify
        .valid(
          "Warning",
          `Are you sure you want to discard these changes ? <br> Cảnh báo ! Bạn có chắc rằng muốn bỏ qua những thay đổi chưa được lưu của ${this.textModify}`
        )
        .then((result) => {
          if (result) {
            this.modalGlues.name = "1";
            this.modalGlues.scheduleID = this.ScheduleID;
            this.modalGlues.partID = args.value;
            this.checkModify = false;
          }
        })
        .catch((err) => {
          args.cancel = true;
      });
    } else {
      this.modalGlues.name = "1";
      this.modalGlues.scheduleID = this.ScheduleID;
      this.modalGlues.partID = args.value;
    }
  }

  onChangeTreatmentWay(args) {
    this.modalGlues.treatmentWayID = args.value;
  }

  updateGlue(modal) {
    this.partService.updateGlue(modal).subscribe((res: any) => {
      this.alertify.success("Update successfully!");
      this.getAllGlue();
    });
  }

  finished() {
    let DataSave = JSON.parse(localStorage.getItem("details"));
    const obj = {
      name: this.textPreview,
      glueID: this.glueID,
      partID: this.partID,
      scheduleID: this.ScheduleID,
      treatmentWayID: this.treatmentID,
      listAdd: DataSave,
    };
    this.selectedRow = this.gridPart.getSelectedRowIndexes();
    this.glueService.saveGlue(obj).subscribe((res: any) => {
      
      if (res.status) {
        this.alertify.success("successfully!");
        this.previewData = [];
        this.getAllGlue();
        this.isShow = false;
        this.checkModify = false;
        this.gridPart.refresh();
        this.removeLocalStore("details");
        this.oldDetail = [];
        // this.modified = false ;
      } else {
        this.alertify.error(res.message);
      }
    });
  }

  removeLocalStore(key: string) {
    localStorage.removeItem(key);
  }

  update(modal) {
    this.partService.update(modal).subscribe((res) => {
      this.alertify.success("Updated successfully!");
      this.GetDetailSchedule();
    });
  }

  rowDeselectedPart(args) {
    const localStoreDetails = this.getLocalStore("details");
    const check = this.compareArray(this.oldDetail, localStoreDetails);
    if (check === false) {
      if (args.isInteracted === true) {
        this.alertify
          .valid(
            "Warning",
            "Are you sure you want to discard these changes ? Cảnh báo ! Bạn có chắc rằng muốn bỏ qua những thay đổi chưa được lưu của " +
              this.textModify
          )
          .then((result) => {
            if (result) {
              this.checkModify = false ;
              this.removeLocalStore("details") ;
              this.modified = true ;
              this.loadPreview() ;
              this.isShow = true ;
              this.getInkChemicalByglueID() ;
              this.oldDetail = [] ;
            }
          })
          .catch((err) => {
            this.checkModify = true ;
            this.gridPart.selectRows(args.rowIndex) ;
        });
      }
    }
  }

  NO(index) {
    return (
      (this.gridPart.pageSettings.currentPage - 1) * this.gridPart.pageSettings.pageSize + Number(index) + 1
    );
  }

}
