import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-totalChemicalModal',
  templateUrl: './totalChemicalModal.component.html',
  styleUrls: ['./totalChemicalModal.component.css']
})
export class TotalChemicalModalComponent implements OnInit {

  data: any
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  @Input() result: any;
  constructor(
    public activeModal: NgbActiveModal,
  ) { }

  ngOnInit() {
    this.data = this.result
  }

}
