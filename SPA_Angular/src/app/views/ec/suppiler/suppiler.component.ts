import { ISupplier } from "./../../../_core/_model/Supplier";
import { IngredientService } from "./../../../_core/_service/ingredient.service";
import { Component, OnInit, ViewChild } from "@angular/core";
import { AlertifyService } from "src/app/_core/_service/alertify.service";
import { GridComponent } from "@syncfusion/ej2-angular-grids";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { TreatmentService } from "src/app/_core/_service/treatment.service";
@Component({
  selector: "app-suppiler",
  templateUrl: "./suppiler.component.html",
  styleUrls: ["./suppiler.component.css"],
})
export class SuppilerComponent implements OnInit {
  public pageSettings = {
    pageCount: 20,
    pageSizes: true,
    currentPage: 1,
    pageSize: 10,
  };
  public toolbarOptions = [ "Add", "Delete", "Cancel", "Search"];
  public editSettings = {
    showDeleteConfirmDialog: false,
    allowEditing: true,
    allowAdding: true,
    allowDeleting: true,
    mode: "Normal",
  };
  public data: object[];
  filterSettings = { type: "Excel" };
  modelSup: ISupplier = {
    id: 0,
    name: "",
    ProcessID: 0,
  };
  public ProcessData: any = [];
  public fieldsSup: object = { text: "name", value: "id" };
  @ViewChild("grid") grid: GridComponent;
  public textGlueLineName = "Select ";
  public supplier: object[];
  public setFocus: any;
  constructor(
    private alertify: AlertifyService,
    public modalService: NgbModal,
    private ingredientService: IngredientService,
    private treatmentWayService: TreatmentService
  ) {


  }

  ngOnInit(): void {
    this.getAllSupplier();
    this.getAllProcess();
  }
  dataBound() {
  }

  rowDeselected(args) {}

  onChangeSupplier(args) {
    this.modelSup.ProcessID = args.value;
  }

  getAllSupplier() {
    this.ingredientService.getAllSupplier().subscribe((res) => {
      this.supplier = res;
    });
  }
  getAllProcess() {
    this.treatmentWayService.getAllProcess().subscribe((res: any) => {
      this.ProcessData = res;
    });
  }

  actionBegin(args) {

    if (args.requestType === "beginEdit" ) {
      this.modelSup.id = args.rowData.id || 0 ;
      this.modelSup.name = args.rowData.name ;
      this.modelSup.ProcessID = args.rowData.processID ;
    }
    if (args.requestType === "save" ) {
      if (args.action === "edit") {
        this.modelSup.id = args.data.id || 0;
        this.modelSup.name = args.data.name;
        // this.modelSup.ProcessID = args.data.processID;
        this.update(this.modelSup);
      }
      if (args.action === "add") {
        const dataSource = this.grid.dataSource as any
        const exist = dataSource.filter(x => x.name === args.data.name && x.processID === this.modelSup.ProcessID)
        if (this.modelSup.ProcessID === 0) {
          this.alertify.error("Please select Treatment");
          args.cancel = true;
          return ;
        }
        if (exist.length > 0) {
          this.alertify.error("Data already exists");
          args.cancel = true;
          return ;
        }
        this.modelSup.id = 0;
        this.modelSup.name = args.data.name;
        if (args.data.name !== undefined && this.modelSup.ProcessID > 0) {
          this.add(this.modelSup);
        } else {
          this.getAllSupplier();
          this.grid.refresh();
        }
      }
    }
    if (args.requestType === "delete") {
      this.delete(args.data[0].id);
    }
  }

  toolbarClick(args): void {
    switch (args.item.text) {
      case "Excel Export":
        this.grid.excelExport();
        break;
      default:
        break;
    }
  }

  actionComplete(e: any): void {
    if (e.requestType === "add") {
      (e.form.elements.namedItem("name") as HTMLInputElement).focus();
      (e.form.elements.namedItem("id") as HTMLInputElement).disabled = true;
    }
  }

  onDoubleClick(args: any): void {
    this.setFocus = args.column; // Get the column from Double click event
  }

  delete(id) {
    this.alertify
      .delete(
        "Delete Supplier",
        'Are you sure you want to delete this Supplier "' + id + '" ?'
      )
      .then((result) => {
        if (result) {
          this.ingredientService.deleteSub(id).subscribe(() => {
            this.alertify.success("The supplier has been deleted");
            this.modelSup.name = "";
            this.modelSup.ProcessID = 0
            this.getAllSupplier();
          });
        }
      })
      .catch((err) => {
        this.getAllSupplier();
        this.grid.refresh();
      });

  }

  update(modelSup) {
    this.ingredientService.updateSub(modelSup).subscribe((res) => {
      this.alertify.success("Updated successfully!");
      this.getAllSupplier();
    });
  }

  add(modelSup) {
    this.ingredientService.createSub(modelSup).subscribe(() => {
      this.alertify.success("Add supplier successfully");
      this.getAllSupplier();
      this.modelSup.name = "";
      this.modelSup.ProcessID = 0
    });
  }

  save() {
    this.ingredientService.createSub(this.modelSup).subscribe(() => {
      this.alertify.success("Add supplier successfully");
      this.getAllSupplier();
      this.modelSup.name = "";
      this.modelSup.ProcessID = 0
    });
  }

  NO(index) {
    return ((this.grid.pageSettings.currentPage - 1) * this.grid.pageSettings.pageSize + Number(index) + 1);
  }
}
